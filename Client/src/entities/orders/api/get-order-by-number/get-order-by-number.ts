import { apiClient } from "@shared/api";

import { GetOrderByNumberResponse } from "./get-order-by-number-response";
import { GetOrderByNumberQuery } from "./get-order-by-number-query";

export const getOrderByNumber = async ({
	orderNumber
}: GetOrderByNumberQuery): Promise<GetOrderByNumberResponse> => {
	return apiClient.get<GetOrderByNumberResponse>(`/orders/number/${orderNumber}`);
};
