export const calculateIncrease = (current: number, previous: number) => {
	if (previous === 0) return 0;
	return ((current - previous) / previous) * 100;
};

export const processMonthData = (orders: any) => {
	if (!orders || orders.length === 0) {
		return { current: 0, previous: 0 };
	}

	const now = new Date();
	const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
	const startOfPreviousMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
	const endOfPreviousMonth = new Date(startOfMonth.getTime() - 1);

	let currentMonthTotal = 0;
	let previousMonthTotal = 0;

	orders.forEach((order: any) => {
		// console.log(order.createdAt);
		const createdAt = new Date(order.createdAt).getTime();
		// console.log(createdAt);
		let orderValue = 0;
		//parseFloat(order.OrderInformation);

		if (order.products.length === 0) {
			orderValue += 0;
		} else {
			for (const product of order.products) {
				orderValue += product.quantity * product.unitPrice;
				// console.log(orderValue);
			}
		}

		if (createdAt >= startOfMonth.getTime()) {
			currentMonthTotal += orderValue;
		} else if (
			createdAt >= startOfPreviousMonth.getTime() &&
			createdAt <= endOfPreviousMonth.getTime()
		) {
			previousMonthTotal += orderValue;
		}
	});

	return { current: currentMonthTotal, previous: previousMonthTotal };
};
