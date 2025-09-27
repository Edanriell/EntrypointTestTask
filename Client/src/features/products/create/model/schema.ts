import { z } from "zod";

import { Currency } from "@entities/products";

export const createProductSchema = z.object({
	name: z
		.string()
		.min(1, "Product name is required")
		.max(100, "Product name cannot exceed 100 characters"),

	description: z
		.string()
		.min(1, "Description is required")
		.max(500, "Description cannot exceed 500 characters"),

	currency: z.nativeEnum(Currency, { required_error: "Currency is required" }),

	price: z
		.number({ message: "Price is required" })
		.gt(0, "Price must be a positive number")
		.max(999999.99, "Price cannot exceed 999,999.99"),

	totalStock: z
		.number({ message: "Stock is required" })
		.int("Stock must be a whole number")
		.gt(0, "Stock cannot be 0")
		.max(999999, "Stock cannot exceed 999,999")
});
