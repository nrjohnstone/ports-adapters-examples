using FileHelpers;

namespace Adapter.Trigger.Csv
{
    [DelimitedRecord("|")]
    internal class BookTitleOrderModel
    {
        public string Title { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}