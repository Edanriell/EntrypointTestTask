import { NextResponse } from "next/server";

export const runtime = "nodejs";

if (process.env.NODE_ENV === "development") {
	process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL!;
const API_VERSION = process.env.NEXT_PUBLIC_API_VERSION!;

export async function GET() {
	try {
		const res = await fetch(`${API_BASE_URL}/${API_VERSION}/statistics/orders/recent`, {
			method: "GET",
			headers: { "Content-Type": "application/json" },
			cache: "no-store",
			next: { revalidate: 0 }
		});

		if (!res.ok) {
			console.error("Backend error:", {
				status: res.status,
				statusText: res.statusText,
				url: res.url
			});
			return NextResponse.json(
				{ error: `Backend returned ${res.status}` },
				{ status: res.status }
			);
		}

		const data = await res.json();
		return NextResponse.json(data);
	} catch (error) {
		console.error("Error fetching recent orders:", error);
		return NextResponse.json({ error: "Failed to fetch recent orders" }, { status: 500 });
	}
}
