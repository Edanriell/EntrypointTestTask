import { OrderStatus } from "../../../model";
import { OrdersResponse } from "../../../api";

type IsStatusCompletedParameters = {
	order: OrdersResponse;
	targetStatus: OrderStatus;
};

export const isStatusCompleted = ({
	order,
	targetStatus
}: IsStatusCompletedParameters): boolean => {
	const statusOrder = [
		OrderStatus.Pending,
		OrderStatus.Confirmed,
		OrderStatus.Processing,
		OrderStatus.ReadyForShipment,
		OrderStatus.Shipped,
		OrderStatus.OutForDelivery,
		OrderStatus.Delivered,
		OrderStatus.Completed
	];

	const currentIndex = statusOrder.indexOf(order.status as OrderStatus);
	const targetIndex = statusOrder.indexOf(targetStatus);

	return currentIndex >= targetIndex;
};
