export const calculateIncrease = (current: number, previous: number) => {
	if (previous === 0) return 0;
	return ((current - previous) / previous) * 100;
};

export const processWeekData = (orders: any) => {
	if (!orders || orders.length === 0) {
		return { current: 0, previous: 0 };
	}
	// console.log(orders);
	const now = new Date();
	const startOfWeek = new Date(now.setDate(now.getDate() - now.getDay()));
	// console.log(startOfWeek);
	const startOfPreviousWeek = new Date(startOfWeek.getTime() - 7 * 24 * 60 * 60 * 1000);
	// console.log(startOfPreviousWeek);

	let currentWeekTotal = 0;
	let previousWeekTotal = 0;

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

		if (createdAt >= startOfWeek.getTime()) {
			currentWeekTotal += orderValue;
		} else if (createdAt >= startOfPreviousWeek.getTime() && createdAt < startOfWeek.getTime()) {
			previousWeekTotal += orderValue;
		}
	});

	// console.log(`${currentWeekTotal} currentWeekTotal`);
	// console.log(`${previousWeekTotal} previousWeekTotal`);

	return { current: currentWeekTotal, previous: previousWeekTotal };
};
