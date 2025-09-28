import { Order } from "../../model";

export type GetOrdersResponse = {
	orders: OrdersResponse[];
	nextCursor?: string;
	previousCursor?: string;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
	totalCount: number;
	currentPageSize: number;
	pageNumber: number;
};

export type OrdersResponse = {
	client?: ClientResponse;
	payments?: PaymentResponse[];
	orderProducts: OrderProductResponse[];
} & Order;

export type OrderProductResponse = {
	productId: string;
	productName: string;
	quantity: number;
	unitPriceCurrency: string;
	unitPriceAmount: number;
	totalPriceAmount: number;
};

export type ClientResponse = {
	clientId: string;
	clientFirstName: string;
	clientLastName: string;
	clientEmail: string;
	clientPhoneNumber?: string;
};

export type PaymentResponse = {
	paymentId: string;
	paymentTotalAmount: number;
	paymentStatus: string;
};
