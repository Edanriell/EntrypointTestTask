export type CustomersListParams = {
	// Pagination
	cursor?: string;
	pageSize?: number;

	// Sorting
	sortBy?: string;
	sortDirection?: "asc" | "desc";

	// Filtering
	nameFilter?: string;
	emailFilter?: string;
	countryFilter?: string;
	cityFilter?: string;
	minTotalSpent?: number;
	maxTotalSpent?: number;
	minTotalOrders?: number;
	maxTotalOrders?: number;
	createdAfter?: string;
	createdBefore?: string;
};

export type UserDetailQuery = {
	id?: string;
};

export type CustomersListQuery = {} & CustomersListParams;

export type RegisterCustomerRequest = {
	firstName: string;
	lastName: string;
	email: string;
	phoneNumber: string;
	gender: number;
	country: string;
	city: string;
	zipCode: string;
	street: string;
	password: string;
};

export type UpdateUserRequest = {
	firstName: string;
	lastName: string;
	email: string;
	phoneNumber: string;
	gender: number;
	country: string;
	city: string;
	zipCode: string;
	street: string;
};

export type UserResponse = {
	firstName: string;
	lastName: string;
	email: string;
	phoneNumber: string;
	gender: string;
	country: string;
	city: string;
	zipCode: string;
	street: string;
};
