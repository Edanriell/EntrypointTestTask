namespace Domain.Entities;

public class Payment
{
	public int           Id            { get; set; }
	public decimal       Amount        { get; set; }
	public DateTime      PaymentDate   { get; set; }
	public PaymentMethod PaymentMethod { get; set; }
	public string        TransactionId { get; set; } = string.Empty;
	public int?          OrderId       { get; set; }
	public Order?        Order         { get; set; }
}