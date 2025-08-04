export type Customer = {
	id: string;
	firstName: string;
	lastName: string;
	email: string;
	phoneNumber: string;
	gender: string;
	country: string;
	city: string;
	zipCode: string;
	street: string;
	createdOnUtc: string;
	totalOrders: number;
	completedOrders: number;
	pendingOrders: number;
	cancelledOrders: number;
	totalSpent: number;
	lastOrderDate: string | null;
	recentOrders: RecentOrder[];
	fullName: string;
	fullAddress: string;
	hasOrders: boolean;
	hasRecentActivity: boolean;
	averageOrderValue: number;
	customerStatus: string;
};

export type RecentOrder = {
	userId: string;
	orderId: string;
	totalAmount: number;
	status: string;
	createdOnUtc: string;
};
