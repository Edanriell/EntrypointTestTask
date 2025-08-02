import { z } from "zod";

export const createProductSchema = z.object({
	name: z
		.string()
		.min(1, "Product name is required")
		.max(100, "Product name cannot exceed 100 characters"),

	description: z
		.string()
		.min(1, "Description is required")
		.max(500, "Description cannot exceed 500 characters"),

	currency: z
		.string()
		.min(1, "Currency is required")
		.max(3, "Currency code cannot exceed 3 characters"),

	price: z
		.number()
		.min(0, "Price must be a positive number")
		.max(999999.99, "Price cannot exceed 999,999.99"),

	stock: z
		.number()
		.int("Stock must be a whole number")
		.min(0, "Stock cannot be negative")
		.max(999999, "Stock cannot exceed 999,999")
});

export type CreateProductFormData = z.infer<typeof createProductSchema>;