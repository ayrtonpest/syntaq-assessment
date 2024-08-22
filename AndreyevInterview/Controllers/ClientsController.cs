using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndreyevInterview.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AIDbContext _context;

        public ClientsController(AIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<ClientModel> GetClients()
        {
            var clientModel = new ClientModel
            {
                Clients = _context.Clients.Select(client => new Clients
                {
                    Id = client.Id,
                    Name = client.Name,
                    Invoices = client.Invoices.Select(s => new Invoice
                    {
                        Id = s.Id,
                        Description = s.Description,
                    }).ToList()
                }).ToList()
            };

            return clientModel;
        }

        [HttpGet("{id}")]
        public ActionResult<Clients> GetClient(int id)
        {
            var client = _context.Clients.Select(c => new Clients
            {
                Id = c.Id,
                Name = c.Name,
                Invoices = c.Invoices.Select(s => new Invoice
                {
                    Id = s.Id,
                    Description = s.Description,
                }).ToList()
            }).FirstOrDefault(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpPost]
        public async Task<ActionResult<Clients>> CreateClient(Clients clientInput)
        {
            var client = new Client
            {
                Name = clientInput.Name
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            clientInput.Id = client.Id;

            return CreatedAtAction(nameof(GetClient), new { id = clientInput.Id }, clientInput);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            var invoices = _context.Invoices.Where(i => i.ClientId == id);
            _context.Invoices.RemoveRange(invoices); 

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Clients clientInput)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            client.Name = clientInput.Name;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{clientId}/add-invoice")]
        public async Task<ActionResult<Invoice>> AddInvoiceToClient(int clientId, InvoiceInput input)
        {
            var client = await _context.Clients.FindAsync(clientId);

            if (client == null)
            {
                return NotFound();
            }

            var invoice = new Invoice
            {
                Description = input.Description,
                ClientId = clientId
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(InvoicesController.GetInvoices), "Invoices", new { id = invoice.Id }, invoice);
        }
    }
}
