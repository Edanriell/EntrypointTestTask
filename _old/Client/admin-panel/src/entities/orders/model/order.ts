import { User } from "@entities/users";
import { Product } from "@entities/products";

export enum OrderStatus {
	Created,
	PendingForPayment,
	Paid,
	InTransit,
	Delivered,
	Cancelled
}

export type Order = {
	id: number;
	createdAt: string;
	updatedAt: string;
	status: OrderStatus;
	shipAddress: string;
	orderInformation: string;
	customer: User;
	products?: Product[] | null;
};
