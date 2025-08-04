import { useQuery } from "@tanstack/react-query";
import { useMemo, useState } from "react";

import type { GetProductsQuery } from "@entities/products";
import { productsQueries } from "@entities/products";

export type UseProductsListOptions = {
	initialPageSize?: number;
	initialSortBy?: string;
	initialSortDirection?: "asc" | "desc";
};

export const useProductsList = (options: UseProductsListOptions = {}) => {
	const {
		initialPageSize = 10,
		initialSortBy = "createdAt",
		initialSortDirection = "desc"
	} = options;

	// State for query parameters
	const [queryParams, setQueryParams] = useState<GetProductsQuery>({
		pageSize: initialPageSize,
		sortBy: initialSortBy,
		sortDirection: initialSortDirection
	});

	// Memoize query to prevent unnecessary re-renders
	const queryOptions = useMemo(() => productsQueries.productsList(queryParams), [queryParams]);

	const query = useQuery(queryOptions);

	// Helper functions for updating query parameters
	const updateQuery = (updates: Partial<GetProductsQuery>) => {
		setQueryParams((prev) => ({ ...prev, ...updates }));
	};

	const resetFilters = () => {
		setQueryParams({
			pageSize: initialPageSize,
			sortBy: initialSortBy,
			sortDirection: initialSortDirection
		});
	};

	const goToNextPage = () => {
		if (query.data?.nextCursor) {
			updateQuery({ cursor: query.data.nextCursor });
		}
	};

	const goToPreviousPage = () => {
		if (query.data?.previousCursor) {
			updateQuery({ cursor: query.data.previousCursor });
		}
	};

	const setSort = (sortBy: string, sortDirection: "asc" | "desc" = "asc") => {
		// Reset cursor when sorting changes
		updateQuery({ sortBy, sortDirection, cursor: undefined });
	};

	const setFilters = (filters: Partial<GetProductsQuery>) => {
		// Reset cursor when filters change
		updateQuery({ ...filters, cursor: undefined });
	};

	return {
		// Query state
		...query,

		// Current query parameters
		queryParams,

		// Helper functions
		updateQuery,
		resetFilters,
		goToNextPage,
		goToPreviousPage,
		setSort,
		setFilters,

		// Pagination helpers
		hasNextPage: query.data?.hasNextPage ?? false,
		hasPreviousPage: query.data?.hasPreviousPage ?? false,
		isFirstPage: !queryParams.cursor,

		// Data helpers
		products: query.data?.products ?? [],
		totalCount: query.data?.totalCount
	};
};
