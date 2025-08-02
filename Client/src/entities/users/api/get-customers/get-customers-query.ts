export type GetCustomersQuery = {
	pageSize?: number;
	cursor?: string | null;
	sortBy?: string | null;
	sortDirection?: string | null;

	// Filtering properties
	nameFilter?: string | null;
	emailFilter?: string | null;
	countryFilter?: string | null;
	cityFilter?: string | null;
	minTotalSpent?: number | null;
	maxTotalSpent?: number | null;
	minTotalOrders?: number | null;
	maxTotalOrders?: number | null;
	createdAfter?: string | null;
	createdBefore?: string | null;
};
