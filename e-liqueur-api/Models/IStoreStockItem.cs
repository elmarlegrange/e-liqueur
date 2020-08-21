namespace e_liqueur.Models
{
    public interface IStoreStockItem
    {
        public long Id { get; set; }
        public StockItem StockItem { get; set; }
        public double Quantity { get; set; }
    }
}