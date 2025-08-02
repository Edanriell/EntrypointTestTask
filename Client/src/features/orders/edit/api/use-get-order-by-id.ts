import { useQuery } from "@tanstack/react-query";

import { ordersQueries } from "@entities/orders";

export const useGetOrderById = (orderId: string) => {
	return useQuery(ordersQueries.orderDetail({ orderId }));
};
