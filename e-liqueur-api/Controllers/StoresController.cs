using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e_liqueur.Models;
using Microsoft.EntityFrameworkCore;
using e_liqueur.Classes.Requests;
using Microsoft.AspNetCore.Http;

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
            var stores = await _context.Stores
                .Include(i => i.Stock)
                .ThenInclude(s => s.StockItem)
                .OrderBy(s => s.Id)
                .ToArrayAsync();

            if (stores.Length == 0)
            {
                return NotFound(new { message = "No stores found" });
            }

            return stores;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(long id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound(new { message = "Store not found" });
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
        public async Task<IActionResult> UpdateStore(long id, UpdateStoreRequest storeRequest)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return NotFound(new { message = "Store not found" });
            
            store.Name = storeRequest.Name;

            _context.Entry(store).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Database update failure when changing store details");
            }

            return Ok(new { message = "Store item updated successfully" });
        }

        [HttpPut("{id}/add-stock")]
        public async Task<ActionResult<Store>> AddStock(long id, PostStoreStockItemRequest stockItemRequest)
        {
            var store = await _context.Stores.FindAsync(id);
            var stockItem = await _context.Stock.FirstOrDefaultAsync(s => s.Name == stockItemRequest.StockItem);
            
            if (store == null)
                return NotFound(new { message = "Store not found" });

            if (stockItem == null)
                return NotFound(new {message = "Stock item not found"});

            store.Stock.Add(
                new StoreStockItem
                {
                    Quantity = stockItemRequest.Quantity, 
                    StockItem = stockItem
                });
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Database update failure when adding stock to store");
            }

            return Ok(new { message = "Stock added to store" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Store>> DeleteStore(long id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return NotFound(new { message = "Store not found" });

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return store;
        }
    }
}