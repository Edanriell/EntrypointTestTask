export type GetOrdersQuery = {
	pageSize?: number;
	cursor?: string;
	sortBy?: string;
	sortDirection?: string;

	// Order filtering properties
	orderNumberFilter?: string;
	statusFilter?: string;
	minTotalAmount?: number;
	maxTotalAmount?: number;
	trackingNumberFilter?: string;
	createdAfter?: string;
	createdBefore?: string;
	confirmedAfter?: string;
	confirmedBefore?: string;
	shippedAfter?: string;
	shippedBefore?: string;
	deliveredAfter?: string;
	deliveredBefore?: string;
	minOutstandingAmount?: number;
	maxOutstandingAmount?: number;
	estimatedDeliveryAfter?: string;
	estimatedDeliveryBefore?: string;
	hasPayment?: boolean;
	isFullyPaid?: boolean;
	hasOutstandingBalance?: boolean;

	// Product filtering properties
	productNameFilter?: string;
	productIdFilter?: string;

	// Client filtering properties
	clientEmailFilter?: string;
	clientNameFilter?: string;

	// Payment filtering properties
	paymentStatusFilter?: string;
};
