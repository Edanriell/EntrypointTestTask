export type DiscountProductCommand = {
	productId: string;
	updatedProductPriceData: ProductData;
};

type ProductData = {
	newPrice?: number;
};
