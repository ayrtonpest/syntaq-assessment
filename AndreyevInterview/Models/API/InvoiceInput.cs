namespace AndreyevInterview.Models.API
{
    public class InvoiceInput
    {
        public string Name { get; set; }
        public int? ClientId { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
    }
}
