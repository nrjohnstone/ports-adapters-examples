using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.UseCases;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Adapter.Trigger.Csv.Tests.Unit
{
    public class OrderBookUseCaseTriggerTests
    {
        internal class OrderBookUseCaseTriggerTestDouble : OrderBookUseCaseTrigger
        {
            private readonly Dictionary<string, string[]> _matchingFiles = 
                new Dictionary<string, string[]>();

            public readonly List<string> DeletedFiles = new List<string>();

            public OrderBookUseCaseTriggerTestDouble(AddBookTitleRequestUseCase addBookTitleRequestUseCase) : base(addBookTitleRequestUseCase)
            {
            }

            public new void CheckAndProcessCsvFiles()
            {
                base.CheckAndProcessCsvFiles();
            }

            public void AddBookOrdersFile(string filePath, string[] lines)
            {
                _matchingFiles.Add(filePath, lines);
            }

            protected override void DeleteFile(string filePath)
            {
                DeletedFiles.Add(filePath);
            }

            protected override bool FileExists(string filePath)
            {
                return _matchingFiles.ContainsKey(filePath);
            }

            protected override Stream GetFileStream(string filePath)
            {                   
                var ms = new MemoryStream();
                var ms2 = new MemoryStream();

                using (var streamWriter = new StreamWriter(ms))
                {
                    var lines = _matchingFiles[filePath];
                    foreach (var line in lines)
                    {
                        streamWriter.WriteLine(line);
                    }
                    streamWriter.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(ms2);
                }

                ms2.Seek(0, SeekOrigin.Begin);
                return ms2;
            }

            protected override IEnumerable<string> GetFilesMatchingBookOrderFileMask()
            {
                // NOTE: We are not trying to test Directory.EnumerateFiles pattern matching
                // so the test double just contains a list of files assumed to match the pattern
                return _matchingFiles.Keys;
            }
        }

        private readonly IBookOrderRepository _bookOrderRepository;

        public OrderBookUseCaseTriggerTests()
        {
            _bookOrderRepository = Substitute.For<IBookOrderRepository>();
        }

        private OrderBookUseCaseTriggerTestDouble CreateSut()
        {
            return new OrderBookUseCaseTriggerTestDouble(
                new AddBookTitleRequestUseCase(
                    _bookOrderRepository));
        }

        [Fact]
        public void BookOrdersFile_WithCorrectlyFormattedLine_ShouldStoreBookOrderInRepository()
        {
            var sut = CreateSut();
            sut.AddBookOrdersFile("Foo1.csv", new[] {"Title|Supplier|10.5|1"});

            sut.CheckAndProcessCsvFiles();

            _bookOrderRepository.Received(1).Store(Arg.Any<BookOrder>());
        }

        [Fact]
        public void Multiple_BookOrdersFiles_ShouldStoreBookOrdersInRepository()
        {
            var sut = CreateSut();
            sut.AddBookOrdersFile("Foo1.csv", new[] { "Title|Supplier|10.5|1" });
            sut.AddBookOrdersFile("Foo2.csv", new[] { "Title|Supplier|10.5|1" });

            sut.CheckAndProcessCsvFiles();

            _bookOrderRepository.Received(2).Store(Arg.Any<BookOrder>());
        }

        [Fact]
        public void BookOrdersFile_WithMultipleLines_ShouldStoreMultipleOrdersInRepository()
        {
            var sut = CreateSut();
            sut.AddBookOrdersFile("Foo1.csv", new[]
            {
                "Book1|Supplier1|10.5|1",
                "Book2|Supplier2|10.5|1"
            });

            sut.CheckAndProcessCsvFiles();

            _bookOrderRepository.Received(2).Store(Arg.Any<BookOrder>());
        }

        [Fact]
        public void BookOrdersFile_WhenProcessingIsFinished_ShouldBeDeleted()
        {
            var sut = CreateSut();
            sut.AddBookOrdersFile("Foo1.csv", new[]
            {
                "Book1|Supplier1|10.5|1",
                "Book2|Supplier2|10.5|1"
            });

            sut.CheckAndProcessCsvFiles();

            sut.DeletedFiles.Should().Contain("Foo1.csv");
        }
    }
}