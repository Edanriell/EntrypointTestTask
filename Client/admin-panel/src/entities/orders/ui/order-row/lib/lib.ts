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
