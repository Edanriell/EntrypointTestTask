export type GetOrdersAndRevenueTrendResponse = {
	trend: Array<MonthlyOrdersRevenue>;
};

type MonthlyOrdersRevenue = {
	month: string;
	orders: number;
	revenue: number;
};
