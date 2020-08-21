using System.Collections.Generic;

namespace e_liqueur.Models
{
    public interface IStore
    {
        long Id { get; set; }
        string Name { get; set; }
        public List<StoreStockItem> Stock { get; }
    }
}