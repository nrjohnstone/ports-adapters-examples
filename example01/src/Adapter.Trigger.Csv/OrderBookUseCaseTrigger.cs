using System.Collections.Generic;
using System.IO;
using System.Threading;
using Domain.UseCases;
using Domain.ValueObjects;
using FileHelpers;

namespace Adapter.Trigger.Csv
{
    internal class OrderBookUseCaseTrigger
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private readonly Thread _threadPoll;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private bool _shutdown;

        protected const string BookOrderFileMask = "BookOrders*.csv";
        public string BookOrderFileFolder { get; }


        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            _orderBookUseCase = orderBookUseCase;
            _threadPoll = new Thread(PollLoopForCsv);
            BookOrderFileFolder = Path.GetTempPath();
        }

        private void PollLoopForCsv()
        {
            while (!_shutdown)
            {
                CheckAndProcessCsvFiles();
                _shutdownEvent.WaitOne(millisecondsTimeout: 2000);
            }
        }

        protected void CheckAndProcessCsvFiles()
        {            
            IEnumerable<string> filePaths = GetFilesMatchingBookOrderFileMask();

            foreach (string filePath in filePaths)
            {
                if (FileExists(filePath))
                {
                    List<BookTitleOrder> bookOrders = new List<BookTitleOrder>();

                    using (Stream stream = GetFileStream(filePath))
                    {
                        var reader = new StreamReader(stream);
                        var engine = new DelimitedFileEngine<BookTitleOrderModel>();
                        BookTitleOrderModel[] records = engine.ReadStream(reader);
                        
                        foreach (var record in records)
                        {
                            bookOrders.Add(new BookTitleOrder(
                                record.Title, record.Supplier, record.Price, record.Quantity));
                        }
                    }

                    // NOTE: Don't do this in production. There is no attempt here do even do this 
                    // in a transactional manner, and you should mark the file as processed once all 
                    // orders are placed correctly rather than delete it, and have some kind of support
                    // for resuming at the last processed line if a fault occurs
                    DeleteFile(filePath);

                    foreach (BookTitleOrder bookOrder in bookOrders)
                    {
                        _orderBookUseCase.Execute(bookOrder);
                    }
                }
            }                            
        }

        public void Start()
        {
            _threadPoll.Start();
        }

        public void Stop()
        {
            _shutdown = true;
            _shutdownEvent.Set();
        }

        #region Test Seams
        protected virtual void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        protected virtual Stream GetFileStream(string file)
        {
            var stream = new FileStream(file, FileMode.Open);
            return stream;
        }

        protected virtual bool FileExists(string file)
        {
            return File.Exists(file);
        }

        protected virtual IEnumerable<string> GetFilesMatchingBookOrderFileMask()
        {
            return Directory.EnumerateFiles(BookOrderFileFolder, BookOrderFileMask);
        }
        #endregion
    }
}