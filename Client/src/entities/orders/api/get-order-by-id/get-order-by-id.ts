import { apiClient } from "@shared/api";

import { GetOrderByIdResponse } from "./get-order-by-id-response";
import { GetOrderByIdQuery } from "./get-order-by-id-query";

export const getOrderById = async ({
	orderId
}: GetOrderByIdQuery): Promise<GetOrderByIdResponse> => {
	return apiClient.get<GetOrderByIdResponse>(`/orders/${orderId}`);
};
