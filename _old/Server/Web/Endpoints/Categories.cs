using Application.Categories.Commands.CreateCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Categories.Commands.UpdateCategoryProducts;
using Application.Categories.Commands.WipeOutAllCategories;
using Application.Categories.Queries.GetAllCategories;
using Application.Categories.Queries.GetCategory;
using Application.Categories.Queries.GetPaginatedAndSortedCategories;
using Domain.Constants;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Categories : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup(this);

		routeGroupBuilder
		   .MapGet(GetPaginatedAndSortedCategories)
		   .MapGet(GetAllCategories, "All")
		   .MapGet(GetCategory,      "{id}")
		   .MapPost(CreateCategory)
		   .MapPut(UpdateCategory,         "{id}")
		   .MapPut(UpdateCategoryProducts, "/Products/{id}")
		   .MapDelete(DeleteCategory, "{id}")
		   .MapDelete(WipeOutAllCategories);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult>
		GetPaginatedAndSortedCategories(ISender                                             sender,
										[AsParameters] GetPaginatedAndSortedCategoriesQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> GetAllCategories(ISender sender, [AsParameters] GetAllCategoriesQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> GetCategory(ISender sender, int id, [AsParameters] GetCategoryQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> CreateCategory(ISender sender, [FromBody] CreateCategoryCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> UpdateCategory(ISender sender, int id, [FromBody] UpdateCategoryCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> UpdateCategoryProducts(ISender                                  sender, int id,
												 [FromBody] UpdateCategoryProductsCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> DeleteCategory(ISender sender, int id, [FromBody] DeleteCategoryCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageCategories)]
	private Task<IResult> WipeOutAllCategories(ISender sender, [AsParameters] WipeOutAllCategoriesCommand command)
	{
		return sender.Send(command);
	}
}