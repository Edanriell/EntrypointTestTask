export type Payment = {
	id: string;
	orderId: string;
	amount: number;
	currency: string;
	paymentStatus: string;
	paymentMethod: string;
	createdAt: string;
	paymentCompletedAt?: string;
	paymentFailedAt?: string;
	paymentExpiredAt?: string;
	paymentFailureReason?: string;
};
