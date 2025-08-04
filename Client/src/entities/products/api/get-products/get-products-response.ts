import { Product } from "../../model";

export type GetProductsResponse = {
	products: Array<Product>;
	nextCursor: string | null;
	previousCursor: string | null;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
	totalCount: number;
	currentPageSize: number;
	pageNumber: number;
};
