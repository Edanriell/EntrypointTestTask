import { apiClient } from "@shared/api";

import { GetCustomerGrowthAndOrderVolumeResponse } from "./get-customer-growth-and-order-volume-response";

export const getCustomerGrowthAndOrderVolume =
	async (): Promise<GetCustomerGrowthAndOrderVolumeResponse> => {
		return await apiClient.get<GetCustomerGrowthAndOrderVolumeResponse>(
			"/statistics/customers/growth-orders"
		);
	};
