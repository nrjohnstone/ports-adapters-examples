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

        private const string BookOrderFileMask = "BookOrders*.csv";
        public string BookOrderFileFolder { get; }


        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            _orderBookUseCase = orderBookUseCase;
            _threadPoll = new Thread(PollForNewCsv);
            BookOrderFileFolder = Path.GetTempPath();
        }

        private void PollForNewCsv()
        {
            while (!_shutdown)
            {
                IEnumerable<string> filePaths = GetFilesMatchingBookOrderFileMask();

                foreach (string filePath in filePaths)
                {
                    if (FileExists(filePath))
                    {
                        Stream stream = GetFileStream(filePath);
                        var reader = new StreamReader(stream);
                        var engine = new DelimitedFileEngine<BookTitleOrderModel>();
                        BookTitleOrderModel[] records = engine.ReadStream(reader);

                        List<BookTitleOrder> bookOrders = new List<BookTitleOrder>();

                        foreach (var record in records)
                        {
                            bookOrders.Add(new BookTitleOrder(
                                record.Title, record.Supplier, record.Price, record.Quantity));
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

                _shutdownEvent.WaitOne(millisecondsTimeout: 2000);
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

        protected virtual void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        protected virtual FileStream GetFileStream(string file)
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

    }
}