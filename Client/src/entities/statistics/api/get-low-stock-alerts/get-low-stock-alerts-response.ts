export type GetLowStockAlertsResponse = {
	lowStockProducts: Array<LowStockProduct>;
};

type LowStockProduct = {
	productName: string;
	unitsInStock: number;
};
