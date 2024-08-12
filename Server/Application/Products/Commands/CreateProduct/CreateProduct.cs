using Application.Interfaces;
using Application.Products.Dto;
using Domain.Entities;
using Domain.Events;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<IResult>
{
	public string               Code         { get; set; } = string.Empty;
	public string               Name         { get; set; } = string.Empty;
	public string               Description  { get; set; } = string.Empty;
	public decimal              UnitPrice    { get; set; }
	public int                  UnitsInStock { get; set; }
	public int?                 UnitsOnOrder { get; set; } = 0;
	public string               Brand        { get; set; } = string.Empty;
	public double?              Rating       { get; set; } = 0;
	public List<CategoryIdDto>? Categories   { get; set; } = new();
}

public class CreateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
	: IRequestHandler<CreateProductCommand, IResult>
{
	public async Task<IResult> Handle(CreateProductCommand request
									, CancellationToken    cancellationToken)
	{
		var entity = new Product
					 {
						 Code         = request.Code,
						 Name         = request.Name,
						 Description  = request.Description,
						 UnitPrice    = request.UnitPrice,
						 UnitsInStock = request.UnitsInStock,
						 UnitsOnOrder = request.UnitsOnOrder ?? 0,
						 Brand        = request.Brand,
						 Rating       = request.Rating ?? 0,
						 ProductCategories = request.Categories?.Select(category => new ProductCategoryLink
							 {
								 CategoryId = category.Id
							 }).ToList()
					 };

		entity.AddDomainEvent(new ProductCreatedEvent(entity));

		context.Products.Add(entity);
		await context.SaveChangesAsync(cancellationToken);

		var savedProduct = await context.Products
							  .AsNoTracking()
							  .Where(product => product.Id == entity.Id)
							  .AsSplitQuery()
							  .Include(product => product.ProductOrders)!
							  .ThenInclude(productOrderLink => productOrderLink.Order)
							  .Include(product => product.Categories)
							  .ProjectTo<CreateProductCommandResponseDto>(mapper.ConfigurationProvider)
							  .FirstOrDefaultAsync(cancellationToken);

		if (savedProduct is null) return TypedResults.Problem("Could not find created product in database");

		return TypedResults.Ok(savedProduct);
	}
}