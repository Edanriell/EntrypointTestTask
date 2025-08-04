export type UpdateOrderCommand = {
	orderId: string;
	updatedOrderData: OrderData;
};

type OrderData = {
	street?: string;
	city?: string;
	zipCode?: string;
	country?: string;
	info?: string;
};
