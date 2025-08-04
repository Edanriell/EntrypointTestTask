import { z } from "zod";

export const cancelOrderSchema = z.object({
	cancellationReason: z
		.string()
		.min(1, "Cancellation reason is required")
		.min(10, "Cancellation reason must be at least 10 characters")
		.max(500, "Cancellation reason must not exceed 500 characters")
});
