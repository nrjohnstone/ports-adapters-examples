using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adapter.Persistence.CouchDb.Repositories;
using FluentAssertions;
using Xunit;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public class BookOrderRepositoryTests
    {
        private BookOrderRepository CreateSut()
        {
            return new BookOrderRepository();
        }

        [Fact]
        public void CanCreateInstance()
        {
            var sut = CreateSut();
            sut.Should().NotBeNull();
        }

        [Fact]
        public void Get_WhenOrderDoesNotExist_ShouldReturnNull()
        {
            var sut = CreateSut();

            var bookOrder = sut.Get(Guid.Parse("D5153312-1BFF-4529-99DA-A189BB050F48"));

            bookOrder.Should().BeNull();
        }
    }
}
