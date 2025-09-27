import { z } from "zod";

export const discountProductSchema = z.object({
	newPrice: z
		.number({ message: "Price is required" })
		.gt(0, "Price must be greater than 0")
		.refine((val) => !isNaN(val), "Please enter a valid price")
});
