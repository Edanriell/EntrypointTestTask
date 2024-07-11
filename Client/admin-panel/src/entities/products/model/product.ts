import { ProductOrderLink } from "@entities/product-order-link";

export type Product = {
	id: number;
	code: string;
	productName: string;
	description: string;
	unitPrice: number;
	unitsInStock: number;
	unitsOnOrder: number;
	productOrders: ProductOrderLink[];
};
