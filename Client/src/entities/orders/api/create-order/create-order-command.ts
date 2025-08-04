export type CreateOrderCommand = {
	clientId: string;
	currency: string;
	shippingAddress: ShippingAddress;
	info?: string;
	orderItems: OrderItem[];
};

export type ShippingAddress = {
	country: string;
	city: string;
	zipCode: string;
	street: string;
};

export type OrderItem = {
	productId: string;
	quantity: number;
};
