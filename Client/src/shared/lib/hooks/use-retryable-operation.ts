import { useMemo } from "react";

import { ErrorHandler } from "@shared/lib/handlers/error";

export function useRetryableOperation<T>(
	operation: () => Promise<T>,
	maxRetries: number = 3,
	delayMs: number = 1000,
	retryCondition?: (error: any) => boolean
) {
	return useMemo(
		() =>
			ErrorHandler.createRetryableErrorHandler(
				operation,
				maxRetries,
				delayMs,
				retryCondition
			),
		[operation, maxRetries, delayMs, retryCondition]
	);
}
