import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { ApiError } from "@shared/lib/handlers/error";

import type { GetProductByIdQuery, GetProductsQuery } from "../api";
import { getProductById, getProducts } from "../api";

export const productsQueries = {
	// Base key
	all: () => ["products"] as const,

	// Lists namespace
	lists: () => [...productsQueries.all(), "list"] as const,

	// Details namespace
	productDetails: () => [...productsQueries.all(), "productDetail"] as const,

	// Customer list with pagination, sorting, and filtering
	productsList: (query?: GetProductsQuery) =>
		queryOptions({
			// Include all query parameters in the key for proper caching
			queryKey: [...productsQueries.lists(), query],
			queryFn: () => getProducts(query),
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
	productDetail: (query?: GetProductByIdQuery) =>
		queryOptions({
			queryKey: [...productsQueries.productDetails(), query?.productId],
			queryFn: () => getProductById({ productId: query?.productId! }),
			enabled: !!query?.productId,
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
