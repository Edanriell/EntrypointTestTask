export type UpdateOrderProductQuantityCommand = {
	productId: string;
	orderId: string;
	productData: ProductData;
};

export type ProductData = {
	quantity: number;
};
