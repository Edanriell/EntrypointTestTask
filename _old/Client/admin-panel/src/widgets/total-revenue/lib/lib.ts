import { formatNumberWithSeparators } from "@shared/lib";

export const calculateTotalRevenue = (ordersData: any) => {
	let total = 0;

	for (const order of ordersData) {
		if (order.products.length > 0) {
			for (const product of order.products) {
				let productOrderTotalPrice = 0;

				productOrderTotalPrice = product.quantity * product.unitPrice;

				total += productOrderTotalPrice;
			}
		}
	}

	return formatNumberWithSeparators(total, 3);
};
