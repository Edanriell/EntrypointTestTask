export type UpdateOrderShippingAddressCommand = {
	orderId: string;
	orderShippingAddress: OrderShippingAddress;
};

type OrderShippingAddress = {
	address: string;
	city: string;
	state: string;
	zipCode: string;
};
