import { Currency } from "../../model";

export type CreateProductCommand = {
	name: string;
	description: string;
	price: number;
	currency: Currency;
	totalStock: number;
};
