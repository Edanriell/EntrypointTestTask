import { z } from "zod";

export const addPaymentSchema = z.object({
	amount: z
		.number({ required_error: "Amount is required" })
		.min(0.01, "Amount must be greater than 0")
		.max(1000000, "Amount must not exceed 1,000,000"),
	currency: z.string().min(1, "Currency is required"),
	paymentMethod: z.string().min(1, "Payment method is required")
});
