export type AddProductToOrderCommand = {
	orderId: string;
	products: readonly ProductItem[];
};

export type ProductItem = {
	productId: string;
	quantity: number;
};
