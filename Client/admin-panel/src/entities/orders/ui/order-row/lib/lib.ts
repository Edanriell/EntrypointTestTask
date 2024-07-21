import { OrderStatus } from "@entities/orders/model";

export const calculateTotalPrice = (products: any) => {
	let totalPrice = 0;

	if (products && products.length === 0) {
		return totalPrice;
	}

	for (const product of products) {
		totalPrice += product.quantity * product.unitPrice;
	}

	return totalPrice;
};

export const displayOrderStatus = (orderStatus: OrderStatus) => {
	switch (orderStatus) {
		case OrderStatus.Created:
			return "Created";
		case OrderStatus.PendingForPayment:
			return "PendingForPayment";
		case OrderStatus.Paid:
			return "Paid";
		case OrderStatus.InTransit:
			return "InTransit";
		case OrderStatus.Delivered:
			return "Delivered";
		case OrderStatus.Cancelled:
			return "Cancelled";
		default:
			return "Unknown status";
	}
};
