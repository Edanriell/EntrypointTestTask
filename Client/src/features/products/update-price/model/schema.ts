import { z } from "zod";

export const updateProductPriceSchema = z.object({
	price: z
		.number({ required_error: "Price is required" })
		.min(0.01, "Price must be greater than 0")
		.max(999999.99, "Price cannot exceed 999,999.99")
});
