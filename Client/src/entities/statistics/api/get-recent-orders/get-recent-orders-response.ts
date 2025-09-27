export type GetRecentOrdersResponse = {
	recentOrders: Array<RecentOrder>;
};

type RecentOrder = {
	number: string;
	customer: string;
	status: string;
	total: number;
	date: string;
};
