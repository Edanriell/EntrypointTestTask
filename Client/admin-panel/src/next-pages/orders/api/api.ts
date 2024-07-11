import { getData } from "@shared/lib";

export const getAllOrders = async (accessToken: string) => {
	return getData(accessToken, "/Orders/all");
};

// export const getAllUsers = async (accessToken: string) => {
// 	return getData(accessToken, "/Users/all");
// };
//
// export const getAllProducts = async (accessToken: string) => {
// 	return getData(accessToken, "/Products/all");
// };
//
// export const getRecentOrders = async (accessToken: string) => {
// 	return getData(accessToken, "/Orders", {
// 		PageIndex: 0,
// 		PageSize: 5,
// 		SortColumn: "CreatedAt",
// 		SortOrder: "DESC"
// 	});
// };

export const getRecentUsers = async (accessToken: string) => {
	return getData(accessToken, "/Users", {
		PageIndex: 0,
		PageSize: 10,
		SortColumn: "CreatedAt",
		SortOrder: "DESC"
	});
};
