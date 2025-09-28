import { z } from "zod";

export const createOrderSchema = z.object({
	customerId: z.string({ message: "Please select a customer" }),
	country: z.string().min(1, "Country is required"),
	city: z.string().min(1, "City is required"),
	zipCode: z.string().min(1, "Zip code is required"),
	street: z.string().min(1, "Street is required"),
	currency: z.string().min(1, "Currency is required"),
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
