import { z } from "zod";

export const shipOrderSchema = z.object({
	trackingNumber: z
		.string()
		.min(1, "Tracking number is required")
		.min(3, "Tracking number must be at least 3 characters")
		.max(50, "Tracking number must not exceed 50 characters"),
	courier: z.number().min(0, "Please select a courier"),
	estimatedDeliveryDate: z
		.string()
		.min(1, "Estimated delivery date is required")
		.refine((date) => {
			const selectedDate = new Date(date);
			const today = new Date();
			today.setHours(0, 0, 0, 0);
			return selectedDate >= today;
		}, "Estimated delivery date cannot be in the past")
});
