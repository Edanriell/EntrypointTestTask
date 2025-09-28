import { GetTotalOrdersResponse } from "@entities/statistics";

export async function getTotalOrders(): Promise<GetTotalOrdersResponse> {
	try {
		const res = await fetch("/api/statistics/orders/total", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch total orders:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching total orders:", error);
		throw error;
	}
}
