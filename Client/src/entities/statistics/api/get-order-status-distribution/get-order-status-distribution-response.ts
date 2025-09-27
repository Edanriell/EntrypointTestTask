import { OrderStatus } from "../../model";

export type GetOrderStatusDistributionResponse = {
	orderStatusDistributions: Array<OrderStatusDistribution>;
};

type OrderStatusDistribution = {
	status: OrderStatus;
	count: number;
};
