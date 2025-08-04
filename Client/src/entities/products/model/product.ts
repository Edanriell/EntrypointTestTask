import { ProductStatus } from "./product-status";
import { Currency } from "./currency";

export type Product = {
	id: string;
	name: string;
	description: string;
	price: number;
	currency: Currency;
	totalStock: number;
	reserved: number;
	available: number;
	isOutOfStock: boolean;
	hasReservations: boolean;
	isInStock: boolean;
	status: ProductStatus;
	createdAt: string;
	lastUpdatedAt: string;
	lastRestockedAt: string | null;
};
