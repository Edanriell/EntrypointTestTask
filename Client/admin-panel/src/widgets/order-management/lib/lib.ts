import { OrderStatus } from "@entities/orders";

export const filterOrdersByStatus = (orders: any, orderStatus: OrderStatus) => {
	return orders?.filter((order: any) => order.status === orderStatus);
};
