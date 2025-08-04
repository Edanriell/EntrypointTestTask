import { Currency } from "../../model";

export type UpdateProductCommand = {
	productId: string;
	updatedProductData: ProductData;
};

export type ProductData = {
	name?: string | null;
	description?: string | null;
	currency?: Currency | null;
	price?: number | null;
	stockChange?: number | null;
};
