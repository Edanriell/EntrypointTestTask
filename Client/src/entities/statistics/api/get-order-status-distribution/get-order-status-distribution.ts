import { apiClient } from "@shared/api";

import { GetOrderStatusDistributionResponse } from "./get-order-status-distribution-response";

export const getOrderStatusDistribution = async (): Promise<GetOrderStatusDistributionResponse> => {
	return await apiClient.get<GetOrderStatusDistributionResponse>("/statistics/orders/status");
};
