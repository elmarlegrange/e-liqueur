using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e_liqueur.Models;
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
            var stock = _context.Stock
                .OrderBy(s => s.Id)
                .ToArray();
            
            if (stock.Length == 0)
            {
                return NotFound();
            }

            return stock;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<StockItem>> GetStockItem(long id)
        {
            var stockItem = await _context.Stock.FindAsync(id);

            if (stockItem == null)
            {
                return NotFound();
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
        public async Task<IActionResult> UpdateStock(long id, StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(stockItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockItemExists(id))
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

            return stockItem;
        }
        
        private bool StockItemExists(long id)
        {
            return _context.Stock.Any(s => s.Id == id);
        }
    }
}