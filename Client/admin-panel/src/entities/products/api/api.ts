import { getData } from "@shared/api";

export const getAllProducts = async (accessToken: string) => {
	return getData(accessToken, "/Products/all");
};
