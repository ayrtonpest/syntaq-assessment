using System.Collections.Generic;

namespace AndreyevInterview.Models.API
{
    public class ClientModel
    {
        public List<Clients> Clients { get; set; }
    }

    public class Clients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}
