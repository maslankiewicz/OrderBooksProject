namespace OrderBookProject.Models
{
    public class Order
    {
        public long SourceTime { get; set; }
        public char? Side { get; set; }
        public char? Action { get; set; }
        public long OrderId { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
    }
}
