import { OrderStatus } from "@entities/orders";

export const getStatusInfo = (status: OrderStatus) => {
	switch (status) {
		case OrderStatus.Pending:
			return {
				color: "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300",
				label: "Pending"
			};
		case OrderStatus.Confirmed:
			return {
				color: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300",
				label: "Confirmed"
			};
		case OrderStatus.Processing:
			return {
				color: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300",
				label: "Processing"
			};
		case OrderStatus.ReadyForShipment:
			return {
				color: "bg-cyan-100 text-cyan-800 dark:bg-cyan-900 dark:text-cyan-300",
				label: "Ready for Shipment"
			};
		case OrderStatus.Shipped:
			return {
				color: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300",
				label: "Shipped"
			};
		case OrderStatus.OutForDelivery:
			return {
				color: "bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-300",
				label: "Out for Delivery"
			};
		case OrderStatus.Delivered:
			return {
				color: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300",
				label: "Delivered"
			};
		case OrderStatus.Completed:
			return {
				color: "bg-emerald-100 text-emerald-800 dark:bg-emerald-900 dark:text-emerald-300",
				label: "Completed"
			};
		case OrderStatus.Cancelled:
			return {
				color: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300",
				label: "Cancelled"
			};
		case OrderStatus.Returned:
			return {
				color: "bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300",
				label: "Returned"
			};
		case OrderStatus.Failed:
			return {
				color: "bg-red-200 text-red-900 dark:bg-red-800 dark:text-red-200",
				label: "Failed"
			};
		case OrderStatus.UnderReview:
			return {
				color: "bg-amber-100 text-amber-800 dark:bg-amber-900 dark:text-amber-300",
				label: "Under Review"
			};
		default:
			return {
				color: "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300",
				label: "Unknown"
			};
	}
};
