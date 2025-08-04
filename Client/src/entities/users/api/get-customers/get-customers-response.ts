import { Customer } from "../../model";

export type GetCustomersResponse = {
	customers: Customer[];
	nextCursor: string | null;
	previousCursor: string | null;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
	totalCount: number;
	currentPageSize: number;
	pageNumber: number;
};
