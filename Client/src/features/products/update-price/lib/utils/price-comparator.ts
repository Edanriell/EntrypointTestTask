export const priceComparator = {
	currency: (apiPrice: string, formPrice: string): boolean => {
		return apiPrice === formPrice;
	}
};
