import { apiClient } from "@shared/api";

import type { CreateOrderCommand } from "./create-order-command";
import type { CreateOrderResponse } from "./create-order-response";

export const createOrder = async (data: CreateOrderCommand): Promise<CreateOrderResponse> => {
	return apiClient.post<CreateOrderResponse>("/orders", data);
};
