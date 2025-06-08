namespace Application.Orders.Dto;

public class CustomerDto
{
	public string Id      { get; init; } = string.Empty;
	public string Name    { get; init; } = string.Empty;
	public string Surname { get; init; } = string.Empty;
	public string Address { get; init; } = string.Empty;
	public string Email   { get; init; } = string.Empty;
}