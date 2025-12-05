namespace PaymentsService.DTOs
{
    public class CreatePaymentDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}
