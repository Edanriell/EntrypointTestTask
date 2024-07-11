import { getData } from "@shared/api";

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
