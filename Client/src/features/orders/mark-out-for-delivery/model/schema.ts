import { z } from "zod";

export const markOutForDeliverySchema = z.object({
	estimatedDeliveryDate: z.string().min(1, "Estimated delivery date is required")
});
