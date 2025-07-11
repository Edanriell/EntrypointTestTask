import z from "zod";

import { GenderEnum } from "../model";

export const editUserSchema = z.object({
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

	street: z.string().min(1, "Street is required").max(200, "Street cannot exceed 200 characters")
});
