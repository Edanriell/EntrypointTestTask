import { EditProductFormData } from "../model";

export const PRODUCT_UPDATABLE_FIELDS: (keyof EditProductFormData)[] = [
	"name",
	"description",
	"price",
	"currency",
	"stock"
];
