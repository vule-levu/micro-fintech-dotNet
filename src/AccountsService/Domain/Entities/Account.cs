namespace AccountsService.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
