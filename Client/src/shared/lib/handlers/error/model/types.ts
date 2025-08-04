export type ValidationError = {
	propertyName: string;
	errorMessage: string;
};

export type ApiErrorResponse = {
	type: string;
	title: string;
	status: number;
	detail: string;
	errors?: ValidationError[];
	traceId?: string;
	code?: string;
	name?: string;
};

export type ApiError = {
	response?: {
		status: number;
		statusText: string;
		data: ApiErrorResponse;
	};
	config?: {
		url?: string;
		method?: string;
	};
} & Error;

export type ErrorContext = {
	action?: string;
	resource?: string;
	userId?: string;
	timestamp?: Date;
	metadata?: Record<string, unknown>;
	retryable?: boolean;
};
