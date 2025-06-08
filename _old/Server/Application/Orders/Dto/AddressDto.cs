namespace Application.Orders.Dto;

public class AddressDto
{
	public string  Street     { get; set; } = string.Empty;
	public string  City       { get; set; } = string.Empty;
	public string? Region     { get; set; }
	public string? PostalCode { get; set; }
	public string  Country    { get; set; } = string.Empty;
}