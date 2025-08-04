export type RemoveProductFromOrderCommand = {
	orderId: string;
	productRemovals: ProductRemovalRequest[];
};

type ProductRemovalRequest = {
	productId: string;
	quantity?: number;
};
