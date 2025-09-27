import { z } from "zod";

export const editOrderSchema = z.object({
	street: z.string().min(1, "Street is required"),
	city: z.string().min(1, "City is required"),
	zipCode: z.string().min(1, "Zip code is required"),
	country: z.string().min(1, "Country is required"),
	info: z.string().optional()
}) as any;
