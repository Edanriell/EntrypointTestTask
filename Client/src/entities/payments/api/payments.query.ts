import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { ApiError } from "@shared/lib/handlers/error";

import type { GetPaymentByIdQuery, GetPaymentsByOrderIdQuery } from "../api";
import { getPaymentById, getPaymentsByOrderId } from "../api";

export const paymentsQueries = {
	// Base key
	all: () => ["payments"] as const,

	// Lists namespace
	lists: () => [...paymentsQueries.all(), "list"] as const,

	// Details namespace
	paymentDetails: () => [...paymentsQueries.all(), "paymentDetail"] as const,

	// Payments by order ID
	paymentsByOrderId: (query?: GetPaymentsByOrderIdQuery) =>
		queryOptions({
			// Include all query parameters in the key for proper caching
			queryKey: [...paymentsQueries.lists(), "byOrderId", query?.orderId],
			queryFn: () => getPaymentsByOrderId({ orderId: query?.orderId! }),
			enabled: !!query?.orderId,
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

	// Payment detail by ID
	paymentDetail: (query?: GetPaymentByIdQuery) =>
		queryOptions({
			queryKey: [...paymentsQueries.paymentDetails(), query?.paymentId],
			queryFn: () => getPaymentById({ paymentId: query?.paymentId! }),
			enabled: !!query?.paymentId,
			staleTime: 5 * 60 * 1000, // 5 minutes
			retry: (failureCount, error) => {
				// Type guard to check if error is ApiError
				const apiError = error as ApiError;

				// Don't retry on 404 - payment doesn't exist
				if (apiError.response?.status === 404) return false;
				return failureCount < 2;
			}
		})
};
