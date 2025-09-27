import { apiClient } from "@shared/api";

import { GetLowStockAlertsResponse } from "./get-low-stock-alerts-response";

export const getLowStockAlerts = async (): Promise<GetLowStockAlertsResponse> => {
	return await apiClient.get<GetLowStockAlertsResponse>("/statistics/products/low-stock-alerts");
};
