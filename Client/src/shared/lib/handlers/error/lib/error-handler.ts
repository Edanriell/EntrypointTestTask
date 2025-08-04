// import { FieldPath, UseFormSetError } from "react-hook-form";
// import { toast } from "sonner";
//
// import { DEFAULT_CONFIG, ErrorHandlerConfig } from "../config";
// import { ApiError, ErrorContext, ErrorType, ValidationError } from "../model";
//
// export class ErrorHandler {
// 	private static config: ErrorHandlerConfig = DEFAULT_CONFIG;
// 	private static activeToasts = 0;
//
// 	// Allow configuration updates
// 	static configure(config: Partial<ErrorHandlerConfig>): void {
// 		this.config = { ...this.config, ...config };
// 	}
//
// 	// Handle form validation errors from API
// 	// static handleFormValidationErrors<T extends Record<string, any>>(
// 	// 	error: ApiError,
// 	// 	setError: UseFormSetError<T>,
// 	// 	fieldMapping: Record<string, FieldPath<T>>,
// 	// 	context?: ErrorContext
// 	// ): boolean {
// 	// 	if (error.response?.status === 400 && error.response?.data?.errors) {
// 	// 		const validationErrors = error.response.data.errors;
// 	// 		let hasValidationErrors = false;
// 	// 		let unmappedErrors: ValidationError[] = [];
// 	//
// 	// 		validationErrors.forEach((validationError: ValidationError) => {
// 	// 			const formFieldName = fieldMapping[validationError.propertyName];
// 	// 			if (formFieldName) {
// 	// 				setError(formFieldName, {
// 	// 					type: "server",
// 	// 					message: validationError.errorMessage
// 	// 				});
// 	// 				hasValidationErrors = true;
// 	// 			} else {
// 	// 				// Collect errors that don't have form field mappings
// 	// 				unmappedErrors.push(validationError);
// 	// 			}
// 	// 		});
// 	//
// 	// 		// Show validation errors in toast
// 	// 		if (hasValidationErrors || unmappedErrors.length > 0) {
// 	// 			this.showValidationErrorsToast(validationErrors);
// 	// 			this.logError(error, { ...context, action: "validation_failed" });
// 	// 			return true;
// 	// 		} else {
// 	// 			toast.error(error.response.data.detail || "Validation failed");
// 	// 			return true;
// 	// 		}
// 	// 	}
// 	//
// 	// 	return false;
// 	// }
//
// 	static handleFormValidationErrors<T extends Record<string, any>>(
// 		error: ApiError,
// 		setError: UseFormSetError<T>,
// 		fieldMapping: Record<string, FieldPath<T>>,
// 		context?: ErrorContext
// 	): boolean {
// 		if (error.response?.status === 400 && error.response?.data?.errors) {
// 			const validationErrors = error.response.data.errors;
// 			let hasValidationErrors = false;
// 			const unmappedErrors: ValidationError[] = [];
//
// 			validationErrors.forEach((validationError: ValidationError) => {
// 				const formFieldName = fieldMapping[validationError.propertyName];
// 				if (formFieldName) {
// 					setError(formFieldName, {
// 						type: "server",
// 						message: validationError.errorMessage
// 					});
// 					hasValidationErrors = true;
// 				} else {
// 					unmappedErrors.push(validationError);
// 				}
// 			});
//
// 			// Show validation errors in toast if enabled
// 			if (this.config.showToasts && (hasValidationErrors || unmappedErrors.length > 0)) {
// 				this.showValidationErrorsToast(validationErrors);
// 			}
//
// 			if (this.config.logErrors) {
// 				this.logError(error, { ...context, action: "validation_failed" });
// 			}
//
// 			return true;
// 		}
//
// 		return false;
// 	}
//
// 	// Handle general API errors
// 	// static handleApiError(error: ApiError, context?: ErrorContext): void {
// 	// 	const status = error.response?.status;
// 	// 	const errorType = error.response?.data?.type;
// 	// 	const errorCode = error.response?.data?.code; // Your backend sends 'code' field
// 	//
// 	// 	switch (status) {
// 	// 		case 400:
// 	// 			if (errorType === ErrorType.VALIDATION) {
// 	// 				toast.error("Please check your input and try again");
// 	// 			} else {
// 	// 				toast.error(error.response?.data?.detail || "Bad request");
// 	// 			}
// 	// 			break;
// 	//
// 	// 		case 401:
// 	// 			if (context?.action?.includes("login") || context?.action?.includes("sign_in")) {
// 	// 				// Handle login credential errors
// 	// 				switch (errorCode) {
// 	// 					case ErrorType.INVALID_CREDENTIALS:
// 	// 						toast.error("Invalid email or password. Please try again");
// 	// 						break;
// 	// 					default:
// 	// 						toast.error(
// 	// 							"Invalid credentials. Please check your email and password"
// 	// 						);
// 	// 				}
// 	// 			} else {
// 	// 				// Handle session expiration for other actions
// 	// 				toast.error("Your session has expired. Please log in again");
// 	// 			}
// 	//
// 	// 			break;
// 	//
// 	// 		case 403:
// 	// 			toast.error("You don't have permission to perform this action");
// 	// 			break;
// 	//
// 	// 		case 404:
// 	// 			toast.error("The requested resource was not found");
// 	// 			break;
// 	//
// 	// 		case 409:
// 	// 			toast.error(
// 	// 				error.response?.data?.detail || "This action conflicts with existing data"
// 	// 			);
// 	// 			break;
// 	//
// 	// 		case 422:
// 	// 			toast.error("Unable to process your request. Please check your input");
// 	// 			break;
// 	//
// 	// 		case 429:
// 	// 			toast.error("Too many requests. Please try again later");
// 	// 			break;
// 	//
// 	// 		case 500:
// 	// 		case 502:
// 	// 		case 503:
// 	// 		case 504:
// 	// 			toast.error("Server error. Please try again later");
// 	// 			break;
// 	//
// 	// 		default:
// 	// 			if (error.message?.includes("Network Error") || !error.response) {
// 	// 				toast.error("Network error. Please check your connection");
// 	// 			} else {
// 	// 				toast.error("An unexpected error occurred");
// 	// 			}
// 	// 	}
// 	//
// 	// 	this.logError(error, context);
// 	// }
//
// 	static handleApiError(
// 		error: ApiError,
// 		context?: ErrorContext,
// 		config?: Partial<ErrorHandlerConfig>
// 	): void {
// 		const effectiveConfig = { ...this.config, ...config };
// 		const status = error.response?.status;
// 		const errorType = error.response?.data?.type;
// 		const errorCode = error.response?.data?.code;
//
// 		const message = this.getErrorMessage(status, errorType, errorCode, error, effectiveConfig);
//
// 		if (effectiveConfig.showToasts) {
// 			toast.error(message);
// 		}
//
// 		if (effectiveConfig.logErrors) {
// 			this.logError(error, context);
// 		}
// 	}
//
// 	// Create standardized error messages for mutations
// 	static createMutationErrorHandler<T extends Record<string, any>>(
// 		setError?: UseFormSetError<T>,
// 		fieldMapping?: Record<string, FieldPath<T>>,
// 		context?: ErrorContext,
// 		config?: Partial<ErrorHandlerConfig>
// 	) {
// 		return (error: ApiError) => {
// 			// Remove console.log from production code
// 			if (process.env.NODE_ENV === "development") {
// 				console.log(error);
// 			}
//
// 			// Try form validation first if we have form methods
// 			if (setError && fieldMapping) {
// 				const wasValidationError = this.handleFormValidationErrors(
// 					error,
// 					setError,
// 					fieldMapping,
// 					context
// 				);
//
// 				if (wasValidationError) return;
// 			}
//
// 			// Handle other errors
// 			this.handleApiError(error, context, config);
// 		};
// 	}
//
// 	static createQueryErrorHandler(context?: ErrorContext, config?: Partial<ErrorHandlerConfig>) {
// 		return (error: ApiError) => {
// 			this.handleApiError(error, context, config);
// 		};
// 	}
//
// 	static createSilentQueryErrorHandler(context?: ErrorContext) {
// 		return (error: ApiError) => {
// 			// Only log errors, don't show toasts for background queries
// 			this.logError(error, context);
// 		};
// 	}
//
// 	static createRetryableQueryErrorHandler(
// 		context?: ErrorContext,
// 		shouldRetry?: (error: ApiError, retryCount: number) => boolean
// 	) {
// 		return (error: ApiError) => {
// 			const isRetryable = shouldRetry ? shouldRetry(error, 0) : this.isRetryableError(error);
//
// 			if (!isRetryable) {
// 				this.handleApiError(error, context);
// 			}
//
// 			// Log all errors regardless of retry status
// 			this.logError(error, { ...context, retryable: isRetryable });
// 		};
// 	}
//
// 	// Create standardized error messages for mutations
// 	// static createMutationErrorHandler<T extends Record<string, any>>(
// 	// 	setError?: UseFormSetError<T>,
// 	// 	fieldMapping?: Record<string, FieldPath<T>>,
// 	// 	context?: ErrorContext
// 	// ) {
// 	// 	return (error: ApiError) => {
// 	// 		// Try form validation first if we have form methods
// 	// 		console.log(error);
// 	// 		if (setError && fieldMapping) {
// 	// 			const wasValidationError = this.handleFormValidationErrors(
// 	// 				error,
// 	// 				setError,
// 	// 				fieldMapping,
// 	// 				context
// 	// 			);
// 	//
// 	// 			if (wasValidationError) return;
// 	// 		}
// 	//
// 	// 		// Handle other errors
// 	// 		this.handleApiError(error, context);
// 	// 	};
// 	// }
//
// 	// Add retry functionality
// 	static createRetryableErrorHandler<T>(
// 		originalFn: () => Promise<T>,
// 		maxRetries: number = 3,
// 		delayMs: number = 1000,
// 		retryCondition?: (error: ApiError) => boolean
// 	) {
// 		return async (): Promise<T> => {
// 			let lastError: ApiError;
//
// 			for (let attempt = 0; attempt <= maxRetries; attempt++) {
// 				try {
// 					return await originalFn();
// 				} catch (error) {
// 					lastError = error as ApiError;
//
// 					const shouldRetry = retryCondition
// 						? retryCondition(lastError)
// 						: this.isRetryableError(lastError);
//
// 					if (attempt === maxRetries || !shouldRetry) {
// 						throw lastError;
// 					}
//
// 					await new Promise((resolve) =>
// 						setTimeout(resolve, delayMs * Math.pow(2, attempt))
// 					);
// 				}
// 			}
//
// 			throw lastError!;
// 		};
// 	}
//
// 	// private static showValidationErrorsToast(errors: ValidationError[]): void {
// 	// 	errors.forEach((error, index) => {
// 	// 		setTimeout(() => {
// 	// 			toast.error("Validation error", {
// 	// 				description: error.errorMessage,
// 	// 				duration: 4000
// 	// 			});
// 	// 		}, index * 250);
// 	// 	});
// 	// }
//
// 	// Add method to clear active toast counter (useful for testing)
// 	static clearActiveToasts(): void {
// 		this.activeToasts = 0;
// 	}
//
// 	// Log errors for debugging/monitoring
// 	// private static logError(error: ApiError, context?: ErrorContext): void {
// 	// 	const errorLog = {
// 	// 		message: error.message,
// 	// 		status: error.response?.status,
// 	// 		url: error.config?.url,
// 	// 		method: error.config?.method,
// 	// 		data: error.response?.data,
// 	// 		context,
// 	// 		timestamp: new Date().toISOString()
// 	// 	};
// 	//
// 	// 	console.error("API Error:", errorLog);
// 	//
// 	// 	// Here we could send our error to monitoring service Sentry, LogRocket, etc.
// 	// 	// this.sendToErrorTracking(errorLog);
// 	// 	// Sentry.captureException(error, {
// 	// 	// 	tags: {
// 	// 	// 		action: errorLog.context?.action,
// 	// 	// 		resource: errorLog.context?.resource
// 	// 	// 	}
// 	// 	// });
// 	// }
//
// 	// Extract error message logic to separate method
// 	private static getErrorMessage(
// 		status: number | undefined,
// 		errorType: string | undefined,
// 		errorCode: string | undefined,
// 		error: ApiError,
// 		config: ErrorHandlerConfig
// 	): string {
// 		// Check for custom messages first
// 		if (status && config.customMessages?.[status]) {
// 			return config.customMessages[status]!;
// 		}
//
// 		switch (status) {
// 			case 400:
// 				if (errorType === ErrorType.VALIDATION) {
// 					return "Please check your input and try again";
// 				}
// 				return error.response?.data?.detail || "Bad request";
//
// 			case 401:
// 				if (errorCode === ErrorType.INVALID_CREDENTIALS) {
// 					return "Invalid email or password. Please try again";
// 				}
// 				return "Your session has expired. Please log in again";
//
// 			case 403:
// 				return "You don't have permission to perform this action";
//
// 			case 404:
// 				return "The requested resource was not found";
//
// 			case 409:
// 				return error.response?.data?.detail || "This action conflicts with existing data";
//
// 			case 422:
// 				return "Unable to process your request. Please check your input";
//
// 			case 429:
// 				return "Too many requests. Please try again later";
//
// 			case 500:
// 			case 502:
// 			case 503:
// 			case 504:
// 				return "Server error. Please try again later";
//
// 			default:
// 				if (error.message?.includes("Network Error") || !error.response) {
// 					return "Network error. Please check your connection";
// 				}
// 				return "An unexpected error occurred";
// 		}
// 	}
//
// 	private static showValidationErrorsToast(errors: ValidationError[]): void {
// 		if (this.activeToasts >= this.config.maxToasts!) return;
//
// 		errors.slice(0, this.config.maxToasts! - this.activeToasts).forEach((error, index) => {
// 			setTimeout(() => {
// 				this.activeToasts++;
// 				toast.error("Validation error", {
// 					description: error.errorMessage,
// 					duration: 4000,
// 					onDismiss: () => this.activeToasts--,
// 					onAutoClose: () => this.activeToasts--
// 				});
// 			}, index * this.config.toastDelay!);
// 		});
// 	}
//
// 	private static logError(error: ApiError, context?: ErrorContext): void {
// 		const errorLog = {
// 			timestamp: new Date().toISOString(),
// 			level: "error",
// 			message: error.message,
// 			status: error.response?.status,
// 			statusText: error.response?.statusText,
// 			url: error.config?.url,
// 			method: error.config?.method,
// 			data: error.response?.data,
// 			context: {
// 				...context,
// 				userAgent: typeof window !== "undefined" ? window.navigator.userAgent : undefined,
// 				url: typeof window !== "undefined" ? window.location.href : undefined
// 			},
// 			stack: error.stack
// 		};
//
// 		if (process.env.NODE_ENV === "development") {
// 			console.error("API Error:", errorLog);
// 		}
//
// 		// Here you could send to monitoring service
// 		// this.sendToErrorTracking(errorLog);
// 	}
//
// 	private static isRetryableError(error: ApiError): boolean {
// 		const status = error.response?.status;
// 		return (
// 			status === 429 || status === 500 || status === 502 || status === 503 || status === 504
// 		);
// 	}
// }

import { FieldPath, UseFormSetError } from "react-hook-form";
import { toast } from "sonner";

import { DEFAULT_CONFIG, ErrorHandlerConfig } from "../config";
import { ApiError, ErrorContext, ErrorType, ValidationError } from "../model";

export class ErrorHandler {
	private static config: ErrorHandlerConfig = DEFAULT_CONFIG;
	private static activeToasts = 0;

	// Allow configuration updates
	static configure(config: Partial<ErrorHandlerConfig>): void {
		this.config = { ...this.config, ...config };
	}

	static handleFormValidationErrors<T extends Record<string, any>>(
		error: ApiError,
		setError: UseFormSetError<T>,
		fieldMapping: Record<string, FieldPath<T>>,
		context?: ErrorContext
	): boolean {
		if (error.response?.status === 400 && error.response?.data?.errors) {
			const validationErrors = error.response.data.errors;
			let hasValidationErrors = false;
			const unmappedErrors: ValidationError[] = [];

			validationErrors.forEach((validationError: ValidationError) => {
				const formFieldName = fieldMapping[validationError.propertyName];
				if (formFieldName) {
					setError(formFieldName, {
						type: "server",
						message: validationError.errorMessage
					});
					hasValidationErrors = true;
				} else {
					unmappedErrors.push(validationError);
				}
			});

			// Show validation errors in toast if enabled
			if (this.config.showToasts && (hasValidationErrors || unmappedErrors.length > 0)) {
				this.showValidationErrorsToast(validationErrors);
			}

			if (this.config.logErrors) {
				this.logError(error, { ...context, action: "validation_failed" });
			}

			return true;
		}

		return false;
	}

	static handleApiError(
		error: ApiError,
		context?: ErrorContext,
		config?: Partial<ErrorHandlerConfig>
	): void {
		const effectiveConfig = { ...this.config, ...config };
		const status = error.response?.status;
		const errorType = error.response?.data?.type;
		const errorCode = error.response?.data?.code;
		const errorName = error.response?.data?.name; // Business rule error name

		const message = this.getErrorMessage(
			status,
			errorType,
			errorCode,
			errorName,
			error,
			effectiveConfig
		);

		if (effectiveConfig.showToasts) {
			toast.error(message);
		}

		if (effectiveConfig.logErrors) {
			this.logError(error, context);
		}
	}

	// Create standardized error messages for mutations
	static createMutationErrorHandler<T extends Record<string, any>>(
		setError?: UseFormSetError<T>,
		fieldMapping?: Record<string, FieldPath<T>>,
		context?: ErrorContext,
		config?: Partial<ErrorHandlerConfig>
	) {
		return (error: ApiError) => {
			// Remove console.log from production code
			if (process.env.NODE_ENV === "development") {
				console.log(error);
			}

			// Try form validation first if we have form methods
			if (setError && fieldMapping) {
				const wasValidationError = this.handleFormValidationErrors(
					error,
					setError,
					fieldMapping,
					context
				);

				if (wasValidationError) return;
			}

			// Handle other errors (including business rule violations)
			this.handleApiError(error, context, config);
		};
	}

	static createQueryErrorHandler(context?: ErrorContext, config?: Partial<ErrorHandlerConfig>) {
		return (error: ApiError) => {
			this.handleApiError(error, context, config);
		};
	}

	static createSilentQueryErrorHandler(context?: ErrorContext) {
		return (error: ApiError) => {
			// Only log errors, don't show toasts for background queries
			this.logError(error, context);
		};
	}

	static createRetryableQueryErrorHandler(
		context?: ErrorContext,
		shouldRetry?: (error: ApiError, retryCount: number) => boolean
	) {
		return (error: ApiError) => {
			const isRetryable = shouldRetry ? shouldRetry(error, 0) : this.isRetryableError(error);

			if (!isRetryable) {
				this.handleApiError(error, context);
			}

			// Log all errors regardless of retry status
			this.logError(error, { ...context, retryable: isRetryable });
		};
	}

	// Add retry functionality
	static createRetryableErrorHandler<T>(
		originalFn: () => Promise<T>,
		maxRetries: number = 3,
		delayMs: number = 1000,
		retryCondition?: (error: ApiError) => boolean
	) {
		return async (): Promise<T> => {
			let lastError: ApiError;

			for (let attempt = 0; attempt <= maxRetries; attempt++) {
				try {
					return await originalFn();
				} catch (error) {
					lastError = error as ApiError;

					const shouldRetry = retryCondition
						? retryCondition(lastError)
						: this.isRetryableError(lastError);

					if (attempt === maxRetries || !shouldRetry) {
						throw lastError;
					}

					await new Promise((resolve) =>
						setTimeout(resolve, delayMs * Math.pow(2, attempt))
					);
				}
			}

			throw lastError!;
		};
	}

	// Add method to clear active toast counter (useful for testing)
	static clearActiveToasts(): void {
		this.activeToasts = 0;
	}

	// Extract error message logic to separate method
	private static getErrorMessage(
		status: number | undefined,
		errorType: string | undefined,
		errorCode: string | undefined,
		errorName: string | undefined, // Add errorName parameter
		error: ApiError,
		config: ErrorHandlerConfig
	): string {
		// Check for custom messages first
		if (status && config.customMessages?.[status]) {
			return config.customMessages[status]!;
		}

		switch (status) {
			case 400:
				// Prioritize business rule error name if available
				if (errorName) {
					return errorName;
				}

				// Then check for validation errors
				if (errorType === ErrorType.VALIDATION) {
					return "Please check your input and try again";
				}

				// Fall back to detail or generic message
				return error.response?.data?.detail || "Bad request";

			case 401:
				if (errorCode === ErrorType.INVALID_CREDENTIALS) {
					return "Invalid email or password. Please try again";
				}
				return "Your session has expired. Please log in again";

			case 403:
				return "You don't have permission to perform this action";

			case 404:
				return "The requested resource was not found";

			case 409:
				return error.response?.data?.detail || "This action conflicts with existing data";

			case 422:
				return "Unable to process your request. Please check your input";

			case 429:
				return "Too many requests. Please try again later";

			case 500:
			case 502:
			case 503:
			case 504:
				return "Server error. Please try again later";

			default:
				if (error.message?.includes("Network Error") || !error.response) {
					return "Network error. Please check your connection";
				}
				return "An unexpected error occurred";
		}
	}

	private static showValidationErrorsToast(errors: ValidationError[]): void {
		if (this.activeToasts >= this.config.maxToasts!) return;

		errors.slice(0, this.config.maxToasts! - this.activeToasts).forEach((error, index) => {
			setTimeout(() => {
				this.activeToasts++;
				toast.error("Validation error", {
					description: error.errorMessage,
					duration: 4000,
					onDismiss: () => this.activeToasts--,
					onAutoClose: () => this.activeToasts--
				});
			}, index * this.config.toastDelay!);
		});
	}

	private static logError(error: ApiError, context?: ErrorContext): void {
		const errorLog = {
			timestamp: new Date().toISOString(),
			level: "error",
			message: error.message,
			status: error.response?.status,
			statusText: error.response?.statusText,
			url: error.config?.url,
			method: error.config?.method,
			data: error.response?.data,
			context: {
				...context,
				userAgent: typeof window !== "undefined" ? window.navigator.userAgent : undefined,
				url: typeof window !== "undefined" ? window.location.href : undefined
			},
			stack: error.stack
		};

		if (process.env.NODE_ENV === "development") {
			console.error("API Error:", errorLog);
		}

		// Here you could send to monitoring service
		// this.sendToErrorTracking(errorLog);
	}

	private static isRetryableError(error: ApiError): boolean {
		const status = error.response?.status;
		return (
			status === 429 || status === 500 || status === 502 || status === 503 || status === 504
		);
	}
}
