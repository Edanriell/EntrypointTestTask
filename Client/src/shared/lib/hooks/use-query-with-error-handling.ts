import { useQuery, UseQueryOptions, UseQueryResult } from "@tanstack/react-query";
import { useEffect } from "react";

import {
	ApiError,
	ErrorContext,
	ErrorHandler,
	ErrorHandlerConfig
} from "@shared/lib/handlers/error";

export function useQueryWithErrorHandling<
	TQueryFnData = unknown,
	TError extends Error = ApiError, // Constrain TError to extend Error
	TData = TQueryFnData,
	TQueryKey extends readonly unknown[] = readonly unknown[]
>(
	options: UseQueryOptions<TQueryFnData, TError, TData, TQueryKey>,
	errorContext?: ErrorContext,
	errorConfig?: Partial<ErrorHandlerConfig>
): UseQueryResult<TData, TError> {
	const result = useQuery(options);

	// Handle errors when they occur
	useEffect(() => {
		if (result.isError && result.error) {
			// Now the cast is safer since TError extends Error
			ErrorHandler.handleApiError(
				result.error as unknown as ApiError,
				errorContext,
				errorConfig
			);
		}
	}, [result.isError, result.error, errorContext, errorConfig]);

	return result;
}
