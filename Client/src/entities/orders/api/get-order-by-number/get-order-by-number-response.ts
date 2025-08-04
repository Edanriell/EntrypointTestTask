import { Order } from "../../model";

export type GetOrderByNumberResponse = {
	client?: ClientResponse;
	payment?: readonly PaymentResponse[];
	orderProducts: readonly OrderProductResponse[];
} & Order;

type OrderProductResponse = {
	productId: string;
	productName: string;
	quantity: number;
	unitPriceCurrency: string;
	unitPriceAmount: number;
	totalPriceAmount: number;
};

type ClientResponse = {
	clientId: string;
	clientFirstName: string;
	clientLastName: string;
	clientEmail: string;
	clientPhoneNumber?: string;
};

type PaymentResponse = {
	paymentId: string;
	paymentTotalAmount: number;
	paymentStatus: string;
};
