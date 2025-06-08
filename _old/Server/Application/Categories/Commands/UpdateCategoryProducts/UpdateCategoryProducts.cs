using Application.Categories.Dto;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.Categories.Commands.UpdateCategoryProducts;

public class UpdateCategoryProductsCommand : IRequest<IResult>
{
	public int                      Id                      { get; set; }
	public List<CategoryProductDto> UpdatedCategoryProducts { get; set; }
}

public class UpdateCategoryProductsCommandHandler(IApplicationDbContext context)
	: IRequestHandler<UpdateCategoryProductsCommand, IResult>
{
	public async Task<IResult> Handle(UpdateCategoryProductsCommand request, CancellationToken cancellationToken)
	{
		var entity = await context.Categories
						.AsSplitQuery()
						.Include(category => category.CategoryProducts)
						.FirstOrDefaultAsync(category => category.Id == request.Id, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Category with ID {request.Id} has not been found.");

		if (request.UpdatedCategoryProducts.Any())
		{
			var oldProducts = entity.CategoryProducts?.Select(productCategoryLink => productCategoryLink.ProductId)
			   .ToList();
			var updatedProducts =
				request.UpdatedCategoryProducts.Select(categoryProduct => categoryProduct.Id).ToList();

			var productsToAdd    = updatedProducts.Except(oldProducts!).ToList();
			var productsToRemove = oldProducts?.Except(updatedProducts).ToList();

			if (productsToAdd.Any())
				foreach (var productId in productsToAdd)
					entity.CategoryProducts?.Add(new ProductCategoryLink { ProductId = productId });

			if (productsToRemove!.Any())
				entity.CategoryProducts?.RemoveAll(product => productsToRemove!.Contains(product.ProductId));
		}

		context.Categories.Update(entity);

		entity.AddDomainEvent(new CategoryProductsUpdatedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}