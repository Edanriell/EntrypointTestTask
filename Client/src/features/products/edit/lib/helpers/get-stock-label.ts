export const getStockLabel = (
	initialStock: number | undefined,
	currentStock: number | undefined
) => {
	if (initialStock === currentStock) return "Stock";
	if (currentStock !== undefined && currentStock > 0) return "Increase stock by";
	if (currentStock !== undefined && currentStock < 0) return "Decrease stock by";
	return "Stock";
};
