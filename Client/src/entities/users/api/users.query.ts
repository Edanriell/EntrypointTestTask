import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { ApiError } from "@shared/lib/handlers/error";

import { getLoggedInUser } from "./get-users";
import { getCustomerById, getCustomers } from "./get-customers";
import { CustomersListQuery, UserDetailQuery } from "../model";

export const usersQueries = {
	// Base key
	all: () => ["users"] as const,

	// Lists namespace
	lists: () => [...usersQueries.all(), "list"] as const,

	// Details namespace
	customerDetails: () => [...usersQueries.all(), "customerDetail"] as const,

	// Customer list with pagination, sorting, and filtering
	customersList: (query?: CustomersListQuery) =>
		queryOptions({
			// Include all query parameters in the key for proper caching
			queryKey: [...usersQueries.lists(), "customers", query],
			queryFn: () => getCustomers(query),
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
	customerDetail: (query?: UserDetailQuery) =>
		queryOptions({
			queryKey: [...usersQueries.customerDetails(), query?.id],
			queryFn: () => getCustomerById(query?.id!),
			enabled: !!query?.id,
			staleTime: 5 * 60 * 1000, // 5 minutes
			retry: (failureCount, error) => {
				// Type guard to check if error is ApiError
				const apiError = error as ApiError;

				// Don't retry on 404 - user doesn't exist
				if (apiError.response?.status === 404) return false;
				return failureCount < 2;
			}
		}),

	// Current logged-in user
	me: () =>
		queryOptions({
			queryKey: [...usersQueries.all(), "me"],
			queryFn: () => getLoggedInUser(),
			staleTime: 10 * 60 * 1000, // 10 minutes
			retry: (failureCount, error) => {
				// Type guard to check if error is ApiError
				const apiError = error as ApiError;

				// Don't retry on 401 - user is not authenticated
				if (apiError.response?.status === 401) return false;
				return failureCount < 2;
			}
		})
};
