import { FieldPath } from "react-hook-form";

import { EditOrderFormData } from "./edit-order-form-data";

export const EDIT_ORDER_FORM_FIELDS_MAPPING: Record<string, FieldPath<EditOrderFormData>> = {
	street: "street",
	city: "city",
	zipCode: "zipCode",
	country: "country",
	info: "info",
	"shippingAddress.street": "street",
	"shippingAddress.city": "city",
	"shippingAddress.zipCode": "zipCode",
	"shippingAddress.country": "country"
} as const;
