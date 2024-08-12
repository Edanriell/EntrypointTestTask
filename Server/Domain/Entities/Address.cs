namespace Domain.Entities;

public class Address
{
	public string  Street     { get; set; } = string.Empty;
	public string  City       { get; set; } = string.Empty;
	public string? Region     { get; set; }
	public string? PostalCode { get; set; }
	public string  Country    { get; set; } = string.Empty;
}