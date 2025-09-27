export type GetTopSellingProductsResponse = {
	bestSellingProducts: Array<BestSellingProduct>;
};

type BestSellingProduct = {
	productName: string;
	unitsSold: number;
	revenue: number;
};
