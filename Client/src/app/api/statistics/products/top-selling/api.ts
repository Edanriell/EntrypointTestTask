import { GetTopSellingProductsResponse } from "@entities/statistics";

export async function getTopSellingProducts(): Promise<GetTopSellingProductsResponse> {
	try {
		const res = await fetch("/api/statistics/products/top-selling", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch top selling products:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching top selling products:", error);
		throw error;
	}
}
