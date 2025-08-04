import { z } from "zod";

export const refundPaymentsSchema = z.object({
	refundReason: z
		.string()
		.min(1, "Refund reason is required")
		.min(10, "Refund reason must be at least 10 characters")
		.max(500, "Refund reason must not exceed 500 characters")
});
