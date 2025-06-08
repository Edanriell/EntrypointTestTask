import { OrderStatus } from "@entities/orders/model";

import { getData } from "@shared/api";

type getPaginatedSortedAndFilteredOrdersParameters = {
	accessToken: string;
	pageParam: number;
	pageSize: number;
	sortColumn?: string;
	sortOrder?: string;
	orderStatus?: OrderStatus;
	customerName?: string | null;
	customerSurname?: string | null;
	customerEmail?: string | null;
	productName?: string | null;
	orderShipAddress?: string | null;
};

export const getAllOrders = async (accessToken: string) => {
	return getData(accessToken, "/Orders/all");
};

export const getRecentOrders = async (accessToken: string) => {
	return getData(accessToken, "/Orders", {
		PageIndex: 0,
		PageSize: 5,
		SortColumn: "CreatedAt",
		SortOrder: "DESC"
	});
};

export const getPaginatedSortedAndFilteredOrders = async ({
	accessToken,
	pageParam,
	pageSize,
	orderStatus,
	sortColumn = "CreatedAt",
	sortOrder = "DESC",
	customerName = null,
	customerSurname = null,
	customerEmail = null,
	productName = null,
	orderShipAddress = null
}: getPaginatedSortedAndFilteredOrdersParameters) => {
	const defaultRequestParams = {
		PageIndex: `${pageParam}`,
		PageSize: `${pageSize}`,
		SortColumn: `${sortColumn}`,
		SortOrder: `${sortOrder}`,
		OrderStatus: orderStatus
	};

	const filterParams = {
		OrderOrdererUserName: customerName,
		OrderOrdererUserSurname: customerSurname,
		OrderOrdererUserEmail: customerEmail,
		OrderProductName: productName,
		OrderShipAddress: orderShipAddress
	};

	const activeFilterParams = Object.fromEntries(
		Object.entries(filterParams).filter(([_, filter]) => filter !== null)
	);

	console.log(activeFilterParams);

	const requestParams = { ...defaultRequestParams, ...activeFilterParams };

	return getData(accessToken, "/Orders", requestParams);
};
