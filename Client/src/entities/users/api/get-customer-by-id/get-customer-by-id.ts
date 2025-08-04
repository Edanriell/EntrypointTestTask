import { apiClient } from "@shared/api";

import { GetCustomerByIdResponse } from "./get-customer-by-id-response";
import { GetCustomerByIdQuery } from "./get-customer-by-id-query";

export const getCustomerById = async ({
	userId
}: GetCustomerByIdQuery): Promise<GetCustomerByIdResponse> => {
	return apiClient.get<GetCustomerByIdResponse>(`/customers/${userId}`);
};
