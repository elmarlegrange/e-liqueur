using System.Collections.Generic;

namespace e_liqueur.Models
{
    public class Store : IStore
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<StoreStockItem> Stock { get; } = new List<StoreStockItem>();
    }
}