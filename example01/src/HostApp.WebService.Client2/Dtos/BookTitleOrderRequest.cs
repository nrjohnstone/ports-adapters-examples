namespace HostApp.WebService.Client2.Dtos
{
    internal class BookTitleOrderRequest
    {
        public string Title { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}