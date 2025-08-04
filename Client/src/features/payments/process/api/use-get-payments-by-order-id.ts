import { useQuery } from "@tanstack/react-query";

import { getPaymentsByOrderId } from "@entities/payments";

export const useGetPaymentsByOrderId = (orderId: string, enabled: boolean = true) => {
	return useQuery({
		queryKey: ["payments", "byOrderId", orderId],
		queryFn: () => getPaymentsByOrderId({ orderId }),
		enabled: enabled && !!orderId,
		staleTime: 30000, // 30 seconds
		gcTime: 300000 // 5 minutes
	});
};
