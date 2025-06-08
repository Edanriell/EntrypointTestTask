import { Order } from "@entities/orders";
import { Product } from "@entities/products";

export type ProductOrderLink = {
	order: Order;
	orderId: number;
	product: Product;
	productId: number;
	quantity: number;
};
