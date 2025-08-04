import { apiClient } from "@shared/api";

import { GetPaymentsByOrderIdResponse } from "./get-payments-by-order-id-response";
import { GetPaymentsByOrderIdQuery } from "./get-payments-by-order-id-query";

export const getPaymentsByOrderId = async ({
	orderId
}: GetPaymentsByOrderIdQuery): Promise<GetPaymentsByOrderIdResponse> => {
	return apiClient.get<GetPaymentsByOrderIdResponse>(`/payments/order/${orderId}`);
};
