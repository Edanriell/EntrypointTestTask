import { Courier } from "./courier";
import { OrderStatus } from "./order-status";

export type Order = {
	id: string;
	clientId: string;
	orderNumber: string;
	status: OrderStatus;
	totalAmount: number;
	paidAmount: number;
	outstandingAmount: number;
	shippingAddress?: string;
	createdAt: string;
	confirmedAt?: string;
	shippedAt?: string;
	deliveredAt?: string;
	cancelledAt?: string;
	estimatedDeliveryDate?: string;
	courier?: Courier;
	cancellationReason?: string;
	returnReason?: string;
	refundReason?: string;
	info?: string;
	trackingNumber?: string;
	currency: string;
};
