#region Namespaces
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndreyevInterview.Models.API;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
#endregion

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        #region Constractor
        private readonly AIDbContext _context;

        public InvoicesController(AIDbContext context)
        {
            _context = context;
        }
        #endregion

        #region All Get API's
        [HttpGet]
        public InvoiceModel GetInvoices()
        {
            List<Invoices> Invoices = new List<Invoices>();
            var invoices = _context.Invoices.Include(i => i.Client).ToList();

            foreach (Invoice _invoice in invoices)
            {
                var lineItems = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == _invoice.Id);
                decimal totalValue = lineItems.Count() > 0 ? lineItems.Sum(x => x.Cost) : 0;
                decimal totalBillableValue = lineItems.Count() > 0 ? lineItems.Where(x => x.isBillable).Sum(x => x.Cost) : 0;
                totalBillableValue *= (1 - (_invoice.Discount / 100));

                Invoices invoice = new Invoices
                {
                    Id = _invoice.Id,
                    Description = _invoice.Description,
                    TotalBillableValue = totalBillableValue,
                    TotalNumberLineItems = lineItems.Count(),
                    TotalValue = totalValue,
                    Discount = _invoice.Discount,
                    Client = _invoice.Client != null ? new Client
                    {
                        Id = _invoice.Client.Id,
                        Name = _invoice.Client.Name
                    } : null
                };

                Invoices.Add(invoice);
            }

            return new InvoiceModel
            {
                Invoices = Invoices
            };
        }


        [HttpGet("{id}")]
        public LineItemModel GetInvoiceLineItems(int id)
        {
            var billableInvoices = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == id && x.isBillable)
                  .GroupBy(r => r.InvoiceId)
                  .Select(a => new
                  {
                      TotalBillableValue = a.Sum(r => r.Cost)
                  }).FirstOrDefault();

            var totalInvoices = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == id)
                  .GroupBy(r => r.InvoiceId)
                  .Select(a => new
                  {
                      GrandTotal = a.Sum(r => r.Cost)
                  }).FirstOrDefault();

            LineItemModel lineItemModel = new LineItemModel
            {
                LineItem = _context.LineItems.Where(x => x.InvoiceId == id).ToList(),
                GrandTotal = totalInvoices == null ? 0 : totalInvoices.GrandTotal,
                TotalBillableValue = billableInvoices == null ? 0 : billableInvoices.TotalBillableValue
            };

            return lineItemModel;
        }
        #endregion

        [HttpPut]
        public Invoice CreateInvoice(InvoiceInput input)
        {
            var invoice = new Invoice
            {
                Description = input.Description,
                ClientId = input.ClientId,
                Discount = input.Discount
            };

            var lineItems = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == invoice.Id);

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return invoice;
        }

        [HttpPost("{id}")]
        public async Task<LineItem> AddLineItemToInvoice(int id, LineItemInput input)
        {
            var lineItem = new LineItem();
            lineItem.InvoiceId = id;
            lineItem.Description = input.Description;
            lineItem.Quantity = input.Quantity;
            lineItem.Cost = input.Cost;
            lineItem.isBillable = input.isBillable;
            lineItem.Id = input.Id;

            if (input.Id == 0)
            {
                await AddLineItem(lineItem);
            }
            else
            {
                await UpdateLineItem(lineItem);
            }

            return lineItem;
        }

        [HttpPut("Update")]
        public async Task<bool> UpdateBillable(LineItemBillable lineItem)
        {
            return await UpdateLineItem(new LineItem
            {
                InvoiceId = lineItem.InvoiceId,
                isBillable = lineItem.isBillable,
                Id = lineItem.LineItemId
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            var lineItems = _context.LineItems.Where(li => li.InvoiceId == id);
            _context.LineItems.RemoveRange(lineItems);

            _context.Invoices.Remove(invoice);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("line-item/{id}")]
        public async Task<IActionResult> DeleteLineItem(int id)
        {
            var lineItem = await _context.LineItems.FindAsync(id);

            if (lineItem == null)
            {
                return NotFound();
            }

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        #region Repositories
        public async Task<bool> UpdateLineItem(LineItem lineItem)
        {
            bool done = false;

            await Task.Run(() =>
            {
                var dblineItem = _context.LineItems.Find(lineItem.Id);

                if (dblineItem != null)
                {
                    _context.Entry(dblineItem).CurrentValues.SetValues(lineItem);

                    foreach (var property in _context.Entry(lineItem).Properties)
                    {
                        if (property.IsModified)
                        {
                            _context.Entry(dblineItem).Property(property.Metadata.Name).IsModified = true;
                        }
                    }

                    if (_context.SaveChanges() > 0)
                    {
                        done = true;
                    }
                }
            });

            return done;
        }

        public async Task<bool> AddLineItem(LineItem lineItem)
        {
            bool done = false;
            await Task.Run(() =>
            {
                _context.LineItems.Add(lineItem);

                if (_context.SaveChanges() == 1)
                {
                    done = true;
                }
            });
            return done;
        }
        #endregion
    }
}