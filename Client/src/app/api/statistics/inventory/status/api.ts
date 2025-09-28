import { GetInventoryStatusResponse } from "@entities/statistics";

export async function getInventoryStatus(): Promise<GetInventoryStatusResponse> {
	try {
		const res = await fetch("/api/statistics/inventory/status", {
			method: "GET",
			cache: "no-store",
			next: { revalidate: 0 },
			headers: { "Content-Type": "application/json" }
		});

		if (!res.ok) {
			console.error("Failed to fetch inventory status:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});

			throw new Error(`Request failed with status ${res.status}`);
		}

		return await res.json();
	} catch (error) {
		console.error("Error fetching inventory status:", error);
		throw error;
	}
}
