export type CreateProductRequest = {
	name: string;
	description: string;
	price: number;
	stock: number;
};

export type ProductsListParams = {
	// Pagination
	pageSize?: number;
	cursor?: string;

	// Sorting
	sortBy?: string;
	sortDirection?: string;

	// Filtering
	nameFilter?: string;
	descriptionFilter?: string;
	minPrice?: number;
	maxPrice?: number;
	minStock?: number;
	maxStock?: number;
	statusFilter?: string;
	createdAfter?: string;
	createdBefore?: string;
	lastUpdatedAfter?: string;
	lastUpdatedBefore?: string;
	lastRestockedAfter?: string;
	lastRestockedBefore?: string;
	hasStock?: boolean;
	isReserved?: boolean;
};

export type ProductsListResponse = {
	products: Product[];
	nextCursor: string | null;
	previousCursor: string | null;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
	totalCount: number;
	currentPageSize: number;
	pageNumber: number;
};

export type Product = {
	id: string;
	name: string;
	description: string;
	price: number;
	reserved: number;
	stock: number;
	status: string;
	createdAt: string;
	lastUpdatedAt: string;
	lastRestockedAt: string | null;
	availableQuantity: number;
};

export type UpdateProductRequest = {
	name?: string;
	description?: string;
	price?: number;
	stockChange?: number;
	reservedChange?: number;
};

export type ProductsListQuery = {} & ProductsListParams;

export type ProductDetailQuery = {
	id?: string;
};

export type UpdateProductPriceRequest = {
	newPrice: number;
};

export type UpdateProductStockRequest = {
	stock: number;
};

export type UpdateProductReservedStockRequest = {
	reservedStock: number;
};

export type DiscountProductRequest = {
	newPrice: number;
};

export type RestoreProductRequest = {};
