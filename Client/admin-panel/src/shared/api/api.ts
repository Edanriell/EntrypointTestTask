import axios from "axios";

// Should come from ENV Variables
const baseURL = "https://localhost:5001/api";

const axiosInstance = axios.create({
	baseURL,
	headers: {
		"Content-Type": "application/json",
		Accept: "application/json"
	}
});

const handleRequestResponse = (response: any) => response.data;

const handleRequestError = (error: any) => {
	if (axios.isAxiosError(error)) {
		if (error.response) {
			console.error("Error response:", {
				data: error.response.data,
				status: error.response.status,
				headers: error.response.headers
			});
			throw new Error(`${error.message}`);
		} else if (error.request) {
			console.error("Error request:", error.request);
			throw new Error("No response received from the server");
		} else {
			console.error("Error message:", error.message);
			throw new Error(`Error setting up request: ${error.message}`);
		}
	} else {
		console.error("Unexpected error:", error);
		throw new Error(`Unexpected error: ${(error as any).message}`);
	}
};

export const getData = async (accessToken: string, endpoint: string, params = {}) => {
	try {
		const { data } = await axiosInstance({
			method: "GET",
			url: endpoint,
			params,
			headers: {
				Authorization: `Bearer ${accessToken}`
			}
		});
		return handleRequestResponse(data);
	} catch (error) {
		return handleRequestError(error);
	}
};
