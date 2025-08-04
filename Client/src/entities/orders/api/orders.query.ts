import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { ApiError } from "@shared/lib/handlers/error";

import type { GetOrderByIdQuery, GetOrdersQuery } from "../api";
import { getOrderById, getOrders } from "../api";

export const ordersQueries = {
	// Base key
	all: () => ["orders"] as const,

	// Lists namespace
	lists: () => [...ordersQueries.all(), "list"] as const,

	// Details namespace
	orderDetails: () => [...ordersQueries.all(), "productDetail"] as const,

	// Customer list with pagination, sorting, and filtering
	ordersList: (query?: GetOrdersQuery) =>
		queryOptions({
			// Include all query parameters in the key for proper caching
			queryKey: [...ordersQueries.lists(), query],
			queryFn: () => getOrders(query),
			placeholderData: keepPreviousData,
			staleTime: 2 * 60 * 1000, // 2 minutes
			retry: (failureCount, error) => {
				// Type guard to check if error is ApiError
				const apiError = error as ApiError;

				// Don't retry on 4xx errors except 429 (rate limit)
				if (
					apiError.response?.status &&
					apiError.response.status >= 400 &&
					apiError.response.status < 500
				) {
					return apiError.response.status === 429 && failureCount < 3;
				}
				// Retry on 5xx errors up to 3 times
				return failureCount < 3;
			},
			retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000) // Exponential backoff
		}),

	// User detail by ID (works for both customers and users)
	orderDetail: (query?: GetOrderByIdQuery) =>
		queryOptions({
			queryKey: [...ordersQueries.orderDetails(), query?.orderId],
			queryFn: () => getOrderById({ orderId: query?.orderId! }),
			enabled: !!query?.orderId,
			staleTime: 5 * 60 * 1000, // 5 minutes
			retry: (failureCount, error) => {
				// Type guard to check if error is ApiError
				const apiError = error as ApiError;

				// Don't retry on 404 - user doesn't exist
				if (apiError.response?.status === 404) return false;
				return failureCount < 2;
			}
		})
};
