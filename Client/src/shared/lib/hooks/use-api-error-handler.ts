import { useCallback } from "react";

import { ErrorContext, ErrorHandler, ErrorHandlerConfig } from "@shared/lib/handlers/error";

export function useApiErrorHandler(context?: ErrorContext, config?: Partial<ErrorHandlerConfig>) {
	return useCallback(
		(error: any) => ErrorHandler.handleApiError(error, context, config),
		[context, config]
	);
}
