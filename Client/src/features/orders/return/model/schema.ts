import { z } from "zod";

export const returnOrderSchema = z.object({
	returnReason: z
		.string()
		.min(1, "Return reason is required")
		.min(10, "Return reason must be at least 10 characters")
		.max(500, "Return reason must not exceed 500 characters")
});
