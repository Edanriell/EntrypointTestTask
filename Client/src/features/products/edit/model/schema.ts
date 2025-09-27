import { z } from "zod";

import { Currency } from "@entities/products";

export const editProductSchema = z.object({
	name: z.string().min(1, "Product name is required"),
	description: z.string().min(1, "Product description is required"),
	price: z.number({ message: "Price is required" }).gt(0, "Price must be positive"),
	currency: z.nativeEnum(Currency, { message: "Currency is required" }),
	totalStock: z
		.number({ message: "Stock is required" })
		.int({ message: "Stock must be a whole number" })
		.refine((val) => val !== 0, {
			message: "Stock cannot be zero"
		})
});
