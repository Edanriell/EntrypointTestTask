export type GetProductByIdResponse = {
	id: string;
	name: string;
	description: string;
	price: number;
	currency: string;
	totalStock: number;
	reserved: number;
	available: number;
	isOutOfStock: boolean;
	hasReservations: boolean;
	isInStock: boolean;
	status: string;
	createdAt: string;
	lastUpdatedAt: string;
	lastRestockedAt: string | null;
};
