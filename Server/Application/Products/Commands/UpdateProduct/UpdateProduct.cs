using Application.Interfaces;
using Application.Products.Dto;
using Domain.Entities;
using Domain.Events;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<IResult>
{
	public int                  Id                { get; set; }
	public string?              Code              { get; set; } = string.Empty;
	public string?              ProductName       { get; set; } = string.Empty;
	public string?              Description       { get; set; } = string.Empty;
	public decimal?             UnitPrice         { get; set; }
	public int?                 UnitsInStock      { get; set; }
	public int?                 UnitsOnOrder      { get; set; }
	public string?              Brand             { get; set; } = string.Empty;
	public double?              Rating            { get; set; }
	public string?              Photo             { get; set; }
	public List<CategoryIdDto>? UpdatedCategories { get; set; }
}

public class UpdateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand, IResult>
{
	public async Task<IResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
	{
//		var entity
//			= await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

		var entity = await context.Products
						.AsSplitQuery()
						.Include(product => product.ProductOrders)!
						.ThenInclude(productOrderLink => productOrderLink.Order)
						.ThenInclude(order => order!.User)
						.Include(product => product.Categories)
						.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Product with ID {request.Id} has not been found.");

		if (!string.IsNullOrWhiteSpace(request.Code))
			entity.Code = request.Code;

		if (!string.IsNullOrWhiteSpace(request.ProductName))
			entity.Name = request.ProductName;

		if (!string.IsNullOrWhiteSpace(request.Description))
			entity.Description = request.Description;

		if (request.UnitPrice.HasValue)
			entity.UnitPrice = request.UnitPrice.Value;

		if (request.UnitsInStock.HasValue)
			entity.UnitsInStock = request.UnitsInStock.Value;

		if (request.UnitsOnOrder.HasValue)
			entity.UnitsOnOrder = request.UnitsOnOrder.Value;

		if (!string.IsNullOrWhiteSpace(request.Brand))
			entity.Brand = request.Brand;

		if (request.Rating.HasValue)
			entity.Rating = request.Rating.Value;

		if (!string.IsNullOrWhiteSpace(request.Photo))
			entity.Photo = await ConvertBase64ToByteArray(request.Photo);

		if (request.UpdatedCategories is not null && request.UpdatedCategories.Any())
		{
			var oldCategories     = entity.Categories?.Select(c => c.Id).ToList();
			var updatedCategories = request.UpdatedCategories.Select(c => c.Id).ToList();

			var categoriesToAdd    = updatedCategories.Except(oldCategories!).ToList();
			var categoriesToRemove = oldCategories?.Except(updatedCategories).ToList();

			if (categoriesToAdd.Any())
				foreach (var categoryId in categoriesToAdd)
					entity.ProductCategories?.Add(new ProductCategoryLink { CategoryId = categoryId });

			if (categoriesToRemove!.Any())
				entity.ProductCategories?.RemoveAll(pc => categoriesToRemove!.Contains(pc.CategoryId));
		}

		context.Products.Update(entity);

		entity.AddDomainEvent(new ProductUpdatedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}

	private async Task<byte[]> ConvertBase64ToByteArray(string base64String)
	{
		const int maxBase64Length = 1398368;
		if (base64String.Length > maxBase64Length)
			throw new ArgumentException("The provided image is too large. The maximum allowed size is 1MB.");
		return await Task.FromResult(Convert.FromBase64String(base64String));
	}
}