export type ShipOrderCommand = {
	orderId: string;
	orderData: OrderData;
};

type OrderData = {
	trackingNumber: string;
	courier: number;
	estimatedDeliveryDate: string;
};
