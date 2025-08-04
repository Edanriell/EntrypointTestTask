export type UpdateProductPriceCommand = {
	productId: string;
	updatedProductPriceData: ProductData;
};

type ProductData = {
	price?: number;
};
