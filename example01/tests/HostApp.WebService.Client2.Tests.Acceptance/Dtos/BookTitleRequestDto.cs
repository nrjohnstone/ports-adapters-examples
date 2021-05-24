namespace HostApp.WebService.Client2.Tests.Acceptance.Dtos
{
    internal class BookTitleRequestDto
    {
        public string Supplier { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}