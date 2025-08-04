import { EditOrderFormData } from "../model";

export const ORDER_UPDATABLE_FIELDS: (keyof EditOrderFormData)[] = [
	"street",
	"city",
	"zipCode",
	"country",
	"info"
];
