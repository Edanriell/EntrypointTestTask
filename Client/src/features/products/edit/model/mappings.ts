import { Currency } from "@entities/payments";

export const EDIT_PRODUCT_FORM_FIELDS_MAPPING = {
	name: "name",
	description: "description",
	price: "price",
	currency: "currency",
	stock: "stock"
} as const;

export const CURRENCY_MAPPING: Record<string, Currency> = {
	Eur: Currency.EUR,
	Usd: Currency.USD
};
