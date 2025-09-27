import { apiClient } from "@shared/api";

import { GetOrdersAndRevenueTrendResponse } from "./get-orders-and-revenue-trend-response";

export const getOrdersAndRevenueTrend = async (): Promise<GetOrdersAndRevenueTrendResponse> => {
	return await apiClient.get<GetOrdersAndRevenueTrendResponse>(
		"/statistics/orders/revenue-trend"
	);
};
