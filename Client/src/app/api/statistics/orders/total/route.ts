import { NextResponse } from "next/server";

export async function GET() {
	try {
		// Build the backend URL using env variables
		const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
		const version = process.env.NEXT_PUBLIC_API_VERSION;

		if (!baseUrl || !version) {
			return NextResponse.json({ error: "API configuration missing" }, { status: 500 });
		}

		const url = `${baseUrl}/${version}/statistics/orders/total`;

		// Fetch data from backend API
		const res = await fetch(url, {
			method: "GET",
			headers: {
				"Content-Type": "application/json"
			}
			// If calling your own backend with self-signed cert (like https://localhost:5001),
			// you may need to disable strict SSL for dev OR use http instead.
		});

		if (!res.ok) {
			return NextResponse.json(
				{ error: `Failed to fetch data: ${res.statusText}` },
				{ status: res.status }
			);
		}

		const data = await res.json();

		return NextResponse.json(data, { status: 200 });
	} catch (error: any) {
		return NextResponse.json({ error: error.message || "Unknown error" }, { status: 500 });
	}
}
