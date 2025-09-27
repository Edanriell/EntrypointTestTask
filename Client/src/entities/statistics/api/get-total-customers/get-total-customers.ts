import { apiClient } from "@shared/api";

import { GetTotalCustomersResponse } from "./get-total-customers-response";

export const getTotalCustomers = async (): Promise<GetTotalCustomersResponse> => {
	return await apiClient.get<GetTotalCustomersResponse>("/statistics/customers/total");
};
