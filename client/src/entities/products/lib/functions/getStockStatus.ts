export const getStockStatus = (stock: number, reserved: number) => {
	const available = stock - reserved;
	if (available <= 0)
		return {
			status: "out-of-stock",
			color: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300"
		};
	if (available <= 10)
		return {
			status: "low-stock",
			color: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300"
		};
	return {
		status: "in-stock",
		color: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300"
	};
};
