import { withAuth } from "next-auth/middleware";
import { NextResponse } from "next/server";

export default withAuth(
	function middleware(req) {
		const token = req.nextauth.token;
		const { pathname } = req.nextUrl;

		// Handle different route protections
		// if (pathname.startsWith("/admin")) {
		// 	if (!token?.roles?.includes("admin")) {
		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
		// 	}
		// }
		//
		// if (pathname.startsWith("/dashboard")) {
		// 	const requiredRoles = ["user", "admin", "manager"];
		// 	const hasAccess = token?.roles?.some((role: string) => requiredRoles.includes(role));
		//
		// 	if (!hasAccess) {
		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
		// 	}
		// }

		return NextResponse.next();
	},
	{
		callbacks: {
			authorized: ({ token, req }) => {
				const { pathname } = req.nextUrl;

				// Always allow access to auth pages
				if (pathname.startsWith("/sign-in") || pathname.startsWith("/auth/")) {
					return true;
				}

				// For protected routes, check if user is authenticated
				return !!token;
			}
		}
	}
);

export const config = {
	matcher: [
		// Protected routes - customize based on your app structure
		"/"
	]
};
