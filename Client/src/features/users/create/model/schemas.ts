import { z } from "zod";

import { GenderEnum } from "./enums";

export const createCustomerSchema = z
	.object({
		firstName: z
			.string()
			.min(1, "First name is required")
			.max(50, "First name cannot exceed 50 characters"),

		lastName: z
			.string()
			.min(1, "Last name is required")
			.max(50, "Last name cannot exceed 50 characters"),

		email: z
			.string()
			.min(1, "Email is required")
			.email("Please enter a valid email address")
			.max(255, "Email cannot exceed 255 characters"),

		phoneNumber: z
			.string()
			.min(1, "Phone number is required")
			.regex(/^\+?[1-9]\d{1,14}$/, "Phone number must be in valid international format"),

		gender: GenderEnum,

		country: z
			.string()
			.min(1, "Country is required")
			.max(100, "Country cannot exceed 100 characters"),

		city: z.string().min(1, "City is required").max(100, "City cannot exceed 100 characters"),

		zipCode: z
			.string()
			.min(1, "Zip code is required")
			.max(20, "Zip code cannot exceed 20 characters"),

		street: z
			.string()
			.min(1, "Street is required")
			.max(200, "Street cannot exceed 200 characters"),

		password: z
			.string()
			.min(8, "Password must be at least 8 characters long")
			.max(128, "Password cannot exceed 128 characters")
			.regex(/[A-Z]/, "Password must contain at least one uppercase letter")
			.regex(/[a-z]/, "Password must contain at least one lowercase letter")
			.regex(/\d/, "Password must contain at least one digit")
			.regex(
				/[!@#$%^&*()_+\-=\[\]{}|;:,.<>?]/,
				"Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)"
			),

		confirmPassword: z.string().min(1, "Please confirm your password")
	})
	.refine((data) => data.password === data.confirmPassword, {
		message: "Passwords do not match",
		path: ["confirmPassword"]
	});
