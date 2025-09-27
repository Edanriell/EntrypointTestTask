import { apiClient } from "@shared/api";

import { GetMonthlyRevenueResponse } from "./get-monthly-revenue-response";

export const getMonthlyRevenue = async (): Promise<GetMonthlyRevenueResponse> => {
	return await apiClient.get<GetMonthlyRevenueResponse>("/statistics/revenue/monthly");
};
