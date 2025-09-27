import { apiClient } from "@shared/api";

import { GetTotalOrdersResponse } from "./get-total-orders-response";

export const getTotalOrders = async (): Promise<GetTotalOrdersResponse> => {
	return await apiClient.get<GetTotalOrdersResponse>("/statistics/orders/total");
};
