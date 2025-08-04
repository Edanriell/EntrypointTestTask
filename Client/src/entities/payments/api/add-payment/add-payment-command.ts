import type { Currency, PaymentMethod } from "../../model";

export type AddPaymentCommand = {
	orderId: string;
	amount: number;
	currency: Currency;
	paymentMethod: PaymentMethod;
};
