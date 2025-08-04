import { useCallback, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";

import { getOrders, GetOrdersQuery } from "@entities/orders";

type OrdersListFilters = {
	orderNumberFilter?: string;
	statusFilter?: string;
	minTotalAmount?: number;
	maxTotalAmount?: number;
	trackingNumberFilter?: string;
	createdAfter?: string;
	createdBefore?: string;
	confirmedAfter?: string;
	confirmedBefore?: string;
	shippedAfter?: string;
	shippedBefore?: string;
	deliveredAfter?: string;
	deliveredBefore?: string;
	minOutstandingAmount?: number;
	maxOutstandingAmount?: number;
	estimatedDeliveryAfter?: string;
	estimatedDeliveryBefore?: string;
	hasPayment?: boolean;
	isFullyPaid?: boolean;
	hasOutstandingBalance?: boolean;
	productNameFilter?: string;
	productIdFilter?: string;
	clientEmailFilter?: string;
	clientNameFilter?: string;
	paymentStatusFilter?: string;
};

type UseOrdersListParams = {
	initialPageSize?: number;
	initialSortBy?: string;
	initialSortDirection?: "asc" | "desc";
};

export const useOrdersList = ({
	initialPageSize = 10,
	initialSortBy = "createdAt",
	initialSortDirection = "desc"
}: UseOrdersListParams = {}) => {
	const [queryParams, setQueryParams] = useState<GetOrdersQuery>({
		pageSize: initialPageSize,
		sortBy: initialSortBy,
		sortDirection: initialSortDirection
	});

	const {
		data: response,
		isLoading,
		error,
		refetch
	} = useQuery({
		queryKey: ["orders", "list", queryParams],
		queryFn: () => getOrders(queryParams),
		placeholderData: (previousData) => previousData
	});

	const orders = useMemo(() => response?.orders ?? [], [response]);
	const totalCount = response?.totalCount;
	const hasNextPage = response?.hasNextPage ?? false;
	const hasPreviousPage = response?.hasPreviousPage ?? false;

	const setSort = useCallback((sortBy: string, sortDirection: "asc" | "desc") => {
		setQueryParams((prev) => ({
			...prev,
			sortBy,
			sortDirection,
			cursor: undefined // Reset cursor when sorting changes
		}));
	}, []);

	const setFilters = useCallback((filters: OrdersListFilters) => {
		setQueryParams((prev) => ({
			...prev,
			...filters,
			cursor: undefined // Reset cursor when filters change
		}));
	}, []);

	const resetFilters = useCallback(() => {
		setQueryParams({
			pageSize: initialPageSize,
			sortBy: initialSortBy,
			sortDirection: initialSortDirection
		});
	}, [initialPageSize, initialSortBy, initialSortDirection]);

	const goToNextPage = useCallback(() => {
		if (hasNextPage && response?.nextCursor) {
			setQueryParams((prev) => ({
				...prev,
				cursor: response.nextCursor
			}));
		}
	}, [hasNextPage, response?.nextCursor]);

	const goToPreviousPage = useCallback(() => {
		if (hasPreviousPage && response?.previousCursor) {
			setQueryParams((prev) => ({
				...prev,
				cursor: response.previousCursor
			}));
		}
	}, [hasPreviousPage, response?.previousCursor]);

	return {
		orders,
		isLoading,
		error,
		hasNextPage,
		hasPreviousPage,
		goToNextPage,
		goToPreviousPage,
		setSort,
		setFilters,
		resetFilters,
		refetch,
		queryParams,
		totalCount
	};
};
