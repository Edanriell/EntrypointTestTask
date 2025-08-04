export const stockComparator = {
	// Fields from API and Client must match!
	totalStock: (apiStock: string, formStock: string): boolean => {
		console.log(apiStock, formStock);

		return apiStock === formStock;
	}
};
