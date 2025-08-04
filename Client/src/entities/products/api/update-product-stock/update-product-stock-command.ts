export type UpdateProductStockCommand = {
	productId: string;
	updatedProductStockData: ProductData;
};

type ProductData = {
	totalStock?: number;
};
