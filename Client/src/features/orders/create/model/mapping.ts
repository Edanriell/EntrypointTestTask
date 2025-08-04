import { FieldPath } from "react-hook-form";

import { CreateOrderFormData } from "./create-order-form-data";

export const CREATE_ORDER_FORM_FIELD_MAPPING: Record<string, FieldPath<CreateOrderFormData>> = {
	clientId: "customerId",
	"shippingAddress.country": "country",
	"shippingAddress.city": "city",
	"shippingAddress.zipCode": "zipCode",
	"shippingAddress.street": "street",
	currency: "currency",
	info: "orderInfo",
	orderItems: "orderProducts"
} as const;
