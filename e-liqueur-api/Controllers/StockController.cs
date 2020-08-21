using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e_liqueur.Models;
using e_liqueur.Classes.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace e_liqueur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly LiquorContext _context;

        public StockController(ILogger<StockController> logger, LiquorContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockItem>>> GetStock()
        {
            var stock = await _context.Stock
                .OrderBy(s => s.Id)
                .ToArrayAsync();

            if (stock.Length == 0)
            {
                return NotFound(new { message = "No stock found" });
            }

            return stock;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<StockItem>> GetStockItem(long id)
        {
            var stockItem = await _context.Stock.FindAsync(id);

            if (stockItem == null)
            {
                return NotFound(new { message = "Stock item not found" });
            }

            return stockItem;
        }
        
        [HttpPost]
        public async Task<ActionResult<StockItem>> CreateStock(StockItem stockItem)
        {
            _context.Add(stockItem);
            await _context.SaveChangesAsync();
            
            var stock = _context.Stock
                .OrderBy(s => s.Id)
                .ToArray();
            
            return CreatedAtAction(nameof(GetStock), new { id = stockItem.Id }, stock);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(long id, UpdateStockItemRequest stockItemRequest)
        {
            var stock = await _context.Stock.FindAsync(id);
            
            if (stock == null)
                return NotFound(new {message = "Stock item not found"});
            
            stock.Name = stockItemRequest.Name;

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Database update failure when changing stock details");
            }

            return Ok(new { message = "Stock item updated successfully" });
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<StockItem>> DeleteStock(long id)
        {
            var stockItem = await _context.Stock.FindAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }

            _context.Stock.Remove(stockItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stock item deleted successfully" });
        }
    }
}