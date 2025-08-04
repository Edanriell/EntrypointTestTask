import { Payment } from "../../model";

export type GetPaymentByIdResponse = {
	refund?: RefundResponse;
} & Payment;

export type RefundResponse = {
	refundId: string;
	refundAmount: number;
	refundCurrency: string;
	refundReason: string;
	refundCreatedAt: string;
	refundProcessedAt?: string;
};
