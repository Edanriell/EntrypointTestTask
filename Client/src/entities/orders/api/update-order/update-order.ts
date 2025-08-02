import { apiClient } from "@shared/api";

import { UpdateOrderShippingAddressCommand } from "./update-order-shipping-address-command";

export const updateOrderShippingAddress = async ({
	orderId,
	orderShippingAddress
}: UpdateOrderShippingAddressCommand): Promise<void> => {
	return apiClient.patch<void>(`orders/${orderId}/shipping-address`, orderShippingAddress);
};
