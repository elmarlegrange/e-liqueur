namespace e_liqueur.Models
{
    public class StoreStockItem : IStoreStockItem
    {
        public long Id { get; set; }
        public StockItem StockItem { get; set; }
        public double Quantity { get; set; }
    }
}