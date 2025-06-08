namespace Domain.Entities;

public class User : IdentityUser
{
	public string              Name      { get; set; } = string.Empty;
	public string              Surname   { get; set; } = string.Empty;
	public string              Password  { get; set; } = string.Empty;
	public string              Address   { get; set; } = string.Empty;
	public DateTime            BirthDate { get; set; }
	public Gender              Gender    { get; set; }
	public DateTime            CreatedAt { get; set; }
	public DateTime?           LastLogin { get; set; }
	public byte[]?             Photo     { get; set; }
	public ICollection<Order>? Orders    { get; set; }
}