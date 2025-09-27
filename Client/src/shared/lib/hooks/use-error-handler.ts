import { FieldPath, UseFormSetError } from "react-hook-form";
import { useMemo } from "react";

import { ErrorContext, ErrorHandler, ErrorHandlerConfig } from "@shared/lib/handlers/error";

export function useErrorHandler<T extends Record<string, any>>(
	setError?: UseFormSetError<T>,
	fieldMapping?: Record<string, FieldPath<T>>,
	context?: ErrorContext,
	config?: Partial<ErrorHandlerConfig>
) {
	return useMemo(
		() => ErrorHandler.createMutationErrorHandler(setError, fieldMapping, context, config),
		[setError, fieldMapping, context, config]
	);
}
