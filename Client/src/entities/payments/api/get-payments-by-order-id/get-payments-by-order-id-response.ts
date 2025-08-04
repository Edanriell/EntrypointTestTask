import { Payment } from "../../model";

export type GetPaymentsByOrderIdResponse = {
	orderId: string;
	payments: readonly PaymentResponse[];
};

export type PaymentResponse = {
	refund?: RefundResponse;
} & Payment;

type RefundResponse = {
	refundId: string;
	refundAmount: number;
	refundCurrency: string;
	refundReason: string;
	refundCreatedAt: string;
	refundProcessedAt?: string;
};
