using Domain.Builders;

namespace Host.WebService.Client1.Tests.Unit
{
    internal static class a
    {
        public static BookTitleRequestBuilder BookTitleOrder => new BookTitleRequestBuilder();
        public static BookOrderBuilder BookOrder => new BookOrderBuilder();
    }
}