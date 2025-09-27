import { z } from "zod";

export const updateProductStockSchema = z.object({
	totalStock: z
		.number({ message: "Stock is required" })
		.int("Stock must be a whole number")
		.max(999999, "Stock cannot exceed 999,999")
});
