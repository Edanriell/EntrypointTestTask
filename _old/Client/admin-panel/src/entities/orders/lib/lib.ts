import { formatNumberWithSeparators } from "@shared/lib";

export const calculateOrderTotal = (orderData: any) => {
	let orderTotal = 0;

	if (orderData.products.length > 0) {
		for (const product of orderData.products) {
			orderTotal += product.quantity * product.unitPrice;
		}
	}

	return formatNumberWithSeparators(orderTotal, 3);
};

export const calculateIncrease = (current: number, previous: number) => {
	if (previous === 0) return 0;
	return ((current - previous) / previous) * 100;
};
