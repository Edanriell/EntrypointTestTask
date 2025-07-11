export type ErrorHandlerConfig = {
	showToasts?: boolean;
	logErrors?: boolean;
	toastDelay?: number;
	maxToasts?: number;
	retryable?: boolean;
	customMessages?: Partial<Record<number, string>>;
};

export const DEFAULT_CONFIG: ErrorHandlerConfig = {
	showToasts: true,
	logErrors: true,
	toastDelay: 250,
	maxToasts: 3,
	retryable: false,
	customMessages: {}
};
