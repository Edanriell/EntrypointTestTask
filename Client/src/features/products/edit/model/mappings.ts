import { Currency } from "@entities/products";

export const EDIT_PRODUCT_FORM_FIELDS_MAPPING = {
	name: "name",
	description: "description",
	price: "price",
	currency: "currency",
	totalStock: "totalStock"
} as const;

export const CURRENCY_MAPPING: Record<string, Currency> = {
	EUR: Currency.Eur,
	USD: Currency.Usd
};
