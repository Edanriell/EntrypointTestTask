import { OrderStatus } from "@entities/orders";

export const getStatusInfo = (status: OrderStatus) => {
	switch (status) {
		case OrderStatus.PENDING:
			return {
				color: "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300",
				label: "Pending"
			};
		case OrderStatus.CONFIRMED:
			return {
				color: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300",
				label: "Confirmed"
			};
		case OrderStatus.PROCESSING:
			return {
				color: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300",
				label: "Processing"
			};
		case OrderStatus.SHIPPED:
			return {
				color: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300",
				label: "Shipped"
			};
		case OrderStatus.DELIVERED:
			return {
				color: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300",
				label: "Delivered"
			};
		case OrderStatus.CANCELLED:
			return {
				color: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300",
				label: "Cancelled"
			};
		case OrderStatus.RETURNED:
			return {
				color: "bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300",
				label: "Returned"
			};
		case OrderStatus.REFUNDED:
			return {
				color: "bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-300",
				label: "Refunded"
			};
		default:
			return {
				color: "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300",
				label: "Unknown"
			};
	}
};
