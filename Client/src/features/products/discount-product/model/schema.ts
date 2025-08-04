import { z } from "zod";

export const discountProductSchema = z.object({
	newPrice: z
		.number()
		.min(0, "Price must be greater than or equal to 0")
		.refine((val) => !isNaN(val), "Please enter a valid price")
});
