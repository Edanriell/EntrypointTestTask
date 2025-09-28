import { GetActiveProductsResponse } from "@entities/statistics";

export async function getActiveProducts(): Promise<GetActiveProductsResponse> {
	try {
		const res = await fetch("/api/statistics/products/active", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch active products:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching active products:", error);
		throw error;
	}
}
