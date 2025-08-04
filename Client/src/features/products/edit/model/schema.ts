import { z } from "zod";

import { Currency } from "@entities/products";

export const editProductSchema = z.object({
	name: z.string().min(1, "Product name is required"),
	description: z.string().min(1, "Product description is required"),
	price: z.number().min(0, "Price must be positive"),
	currency: z.nativeEnum(Currency, { required_error: "Currency is required" }),
	stock: z.number().int().min(0, "Stock must be a non-negative integer")
});
