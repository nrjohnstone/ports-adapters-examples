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

    }
}
