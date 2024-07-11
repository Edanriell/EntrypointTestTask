import { getData } from "@shared/api";

export const getAllUsers = async (accessToken: string) => {
	return getData(accessToken, "/Users/all");
};

export const getRecentUsers = async (accessToken: string) => {
	return getData(accessToken, "/Users", {
		PageIndex: 0,
		PageSize: 10,
		SortColumn: "CreatedAt",
		SortOrder: "DESC"
	});
};
