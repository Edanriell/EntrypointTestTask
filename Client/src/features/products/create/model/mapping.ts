import { FieldPath } from "react-hook-form";

import { CreateProductFormData } from "./";

export const CREATE_PRODUCT_FORM_FIELD_MAPPING: Record<string, FieldPath<CreateProductFormData>> = {
	name: "name",
	description: "description",
	currency: "currency",
	price: "price",
	totalStock: "totalStock"
} as const;
