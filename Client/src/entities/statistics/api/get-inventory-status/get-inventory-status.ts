import { apiClient } from "@shared/api";

import { GetInventoryStatusResponse } from "./get-inventory-status-response";

export const getInventoryStatus = async (): Promise<GetInventoryStatusResponse> => {
	return await apiClient.get<GetInventoryStatusResponse>("/statistics/inventory/status");
};
