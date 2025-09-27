export type GetProductsQuery = {
	pageSize?: number;
	cursor?: string | null;
	sortBy?: string | null;
	sortDirection?: string | null;

	// Filtering properties
	nameFilter?: string | null;
	descriptionFilter?: string | null;
	minPrice?: number | null;
	maxPrice?: number | null;
	minStock?: number | null;
	maxStock?: number | null;
	statusFilter?: string | null;
	createdAfter?: string | null;
	createdBefore?: string | null;
	lastUpdatedAfter?: string | null;
	lastUpdatedBefore?: string | null;
	lastRestockedAfter?: string | null;
	lastRestockedBefore?: string | null;
	hasStock?: boolean | null;
	hasLowStock?: boolean | null;
	isReserved?: boolean | null;
};
