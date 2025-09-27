import { z } from "zod";

export const createOrderSchema = z.object({
	customerId: z.string().min(1, "Please select a customer"),
	country: z.string().min(1, "Country is required"),
	city: z.string().min(1, "City is required"),
	zipCode: z.string().min(1, "Zip code is required"),
	street: z.string().min(1, "Street is required"),
	currency: z.enum(["Usd", "Eur"]).optional().default("Eur"),
	orderInfo: z.string().optional(),
	orderProducts: z
		.array(
			z.object({
				productId: z.string(),
				quantity: z.number().min(1)
			})
		)
		.min(1, "At least one product is required")
});
