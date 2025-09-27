import { apiClient } from "@shared/api";

import { GetRecentOrdersResponse } from "./get-recent-orders-response";

export const getRecentOrders = async (): Promise<GetRecentOrdersResponse> => {
	return await apiClient.get<GetRecentOrdersResponse>("/statistics/orders/recent");
};
