using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e_liqueur.Models;
using Microsoft.EntityFrameworkCore;
using e_liqueur.Classes.Requests;

namespace e_liqueur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly ILogger<StoresController> _logger;
        private readonly LiquorContext _context;

        public StoresController(ILogger<StoresController> logger, LiquorContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetAllStores()
        {
            var stores = _context.Stores
                .Include(i => i.Stock)
                .OrderBy(s => s.Id)
                .ToArray();

            if (stores.Length == 0)
            {
                return NotFound();
            }

            return stores;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(long id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }
        
        [HttpPost]
        public async Task<ActionResult<Store>> CreateStore(Store store)
        {
            _context.Add(store);
            await _context.SaveChangesAsync();
            
            var stores = _context.Stores
                .OrderBy(s => s.Id)
                .ToArray();
            
            return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(long id, Store store)
        {
            if (id != store.Id)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("{id}/add-stock")]
        public async Task<ActionResult<Store>> AddStock(long id, PostStockItemRequest stockItemRequest)
        {
            var store = await _context.Stores.FindAsync(id);
            var stockItem = await _context.Stock.SingleOrDefaultAsync(s => s.Name == stockItemRequest.StockItem);

            if (store == null || stockItem == null)
            {
                return NotFound();
            }

            store.Stock.Add(
                new StoreStockItem
                {
                    Quantity = stockItemRequest.Quantity, 
                    StockItem = new StockItem
                    {
                        Name = stockItem.Name
                    }
                });
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Store>> DeleteStore(long id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return store;
        }

        private bool StoreExists(long id)
        {
            return _context.Stores.Any(s => s.Id == id);
        }
    }
}