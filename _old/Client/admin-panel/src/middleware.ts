import { getToken } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";
import { NextResponse } from "next/server";

export default withAuth(
	async function middleware(req) {
		const token = await getToken({ req, secret: process.env.NEXTAUTH_SECRET });
		const { pathname } = req.nextUrl;
		// console.log(`${token}  token`);
		// console.log(token);
		// Define protected routes
		const sensitiveRoutes = ["/dashboard", "/access-denied"];
		const allowedRoles = ["Moderator", "Administrator", "SuperAdministrator"];

		// Check if the current route requires authentication
		const isAccessingSensitiveRoute = sensitiveRoutes.some((route) => pathname.startsWith(route));

		// Handle redirection for login page
		if (pathname.startsWith("/login")) {
			if (token) {
				return NextResponse.redirect(new URL("/dashboard", req.url));
			}
			return NextResponse.next();
		}

		// Handle redirection for protected routes
		if (isAccessingSensitiveRoute) {
			if (!token) {
				return NextResponse.redirect(new URL("/login", req.url));
			}

			const userHasAllowedRole = (token.roles as Array<string>)?.some((role) =>
				allowedRoles.includes(role)
			);

			if (pathname.startsWith("/dashboard")) {
				if (!userHasAllowedRole) {
					return NextResponse.redirect(new URL("/access-denied", req.url));
				}
			}

			if (pathname.startsWith("/access-denied")) {
				if (userHasAllowedRole) {
					return NextResponse.redirect(new URL("/dashboard", req.url));
				}
			}
		}

		// Redirect root to dashboard
		if (pathname === "/") {
			return NextResponse.redirect(new URL("/dashboard", req.url));
		}

		// Allow access if authenticated or not accessing sensitive routes
		return NextResponse.next();
	},
	{
		callbacks: {
			async authorized() {
				return true;
			}
		}
	}
);

export const config = {
	matcher: ["/", "/login", "/dashboard/:path*", "/access-denied"]
};
