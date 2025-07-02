// import { withAuth } from "next-auth/middleware";
// import { NextResponse } from "next/server";
//
// export default withAuth(
// 	function middleware(req) {
// 		const token = req.nextauth.token;
// 		const { pathname } = req.nextUrl;
//
// 		// Handle different route protections
// 		// if (pathname.startsWith("/admin")) {
// 		// 	if (!token?.roles?.includes("admin")) {
// 		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
// 		// 	}
// 		// }
// 		//
// 		// if (pathname.startsWith("/dashboard")) {
// 		// 	const requiredRoles = ["user", "admin", "manager"];
// 		// 	const hasAccess = token?.roles?.some((role: string) => requiredRoles.includes(role));
// 		//
// 		// 	if (!hasAccess) {
// 		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
// 		// 	}
// 		// }
//
// 		return NextResponse.next();
// 	},
// 	{
// 		callbacks: {
// 			authorized: ({ token, req }) => {
// 				const { pathname } = req.nextUrl;
//
// 				// Always allow access to auth pages
// 				if (pathname.startsWith("/sign-in") || pathname.startsWith("/auth/")) {
// 					return true;
// 				}
//
// 				// For protected routes, check if user is authenticated
// 				return !!token;
// 			}
// 		}
// 	}
// );
//
// export const config = {
// 	matcher: [
// 		// Protected routes - customize based on your app structure
// 		"/"
// 	]
// };

// import { withAuth } from "next-auth/middleware";
// import { NextResponse } from "next/server";
//
// export default withAuth(
// 	function middleware(req) {
// 		// Only handle Keycloak realm requests that come to our domain
// 		if (req.nextUrl.pathname.startsWith("/realms/")) {
// 			// Redirect to the actual Keycloak server
// 			const keycloakUrl = new URL(
// 				req.nextUrl.pathname + req.nextUrl.search,
// 				"http://localhost:18080"
// 			);
// 			return NextResponse.redirect(keycloakUrl);
// 		}
//
// 		return NextResponse.next();
// 	},
// 	{
// 		callbacks: {
// 			authorized: ({ token, req }) => {
// 				// Always allow auth-related endpoints
// 				if (req.nextUrl.pathname.startsWith("/api/auth/")) {
// 					return true;
// 				}
//
// 				// Allow access to sign-in page
// 				if (req.nextUrl.pathname === "/sign-in") {
// 					return true;
// 				}
//
// 				// Allow access to public assets
// 				if (
// 					req.nextUrl.pathname.startsWith("/_next/") ||
// 					req.nextUrl.pathname.startsWith("/images/") ||
// 					req.nextUrl.pathname === "/favicon.ico"
// 				) {
// 					return true;
// 				}
//
// 				// For other routes, require authentication
// 				return !!token;
// 			}
// 		}
// 	}
// );
//
// export const config = {
// 	matcher: [
// 		"/",
// 		// Protect all routes except API auth, static files, and public assets
// 		"/((?!api/auth|_next/static|_next/image|favicon.ico|public/).*)",
// 		// Handle Keycloak realm redirects
// 		"/realms/:path*"
// 	]
// };

import { withAuth } from "next-auth/middleware";
import { NextResponse } from "next/server";

export default withAuth(
	function middleware(req) {
		// Handle Keycloak realm requests that come to our domain
		if (req.nextUrl.pathname.startsWith("/realms/")) {
			// Redirect to the actual Keycloak server
			const keycloakUrl = new URL(
				req.nextUrl.pathname + req.nextUrl.search,
				process.env.KEYCLOAK_ISSUER || "http://localhost:18080"
			);

			return NextResponse.redirect(keycloakUrl);
		}

		// Examples
		// Handle role-based access control
		// const token = req.nextauth.token;
		// const pathname = req.nextUrl.pathname;

		// Admin routes protection
		// if (pathname.startsWith("/admin")) {
		// 	const hasAdminRole = token?.roles?.includes("admin") || token?.roles?.includes("admin");
		//
		// 	if (!hasAdminRole) {
		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
		// 	}
		// }

		// User management routes protection
		// if (pathname.startsWith("/users")) {
		// 	const hasUserManagementPermission =
		// 		token?.permissions?.includes("users:read") ||
		// 		token?.roles?.includes("user-manager") ||
		// 		token?.roles?.includes("admin");
		//
		// 	if (!hasUserManagementPermission) {
		// 		return NextResponse.redirect(new URL("/unauthorized", req.url));
		// 	}
		// }

		const response = NextResponse.next();

		// Adding security headers, also could be configured in next.config.ts
		// Prevents website from being embedded in `<iframe>`, `<frame>`, `<embed>`, or `<object>` tags
		// Protects Against: Clickjacking attacks where malicious sites embed your page invisibly and trick users into clicking
		// DENY - Never allow framing
		// SAMEORIGIN - Only allow framing from same origin
		// ALLOW-FROM uri - Allow framing from specific URI
		response.headers.set("X-Frame-Options", "DENY");
		// Prevents browsers from MIME-type sniffing (guessing file types)
		// Protects Against: MIME confusion attacks where browsers interpret files differently than intended
		// Example: Prevents a `.txt` file from being executed as JavaScript if it contains JS code
		response.headers.set("X-Content-Type-Options", "nosniff");
		// Controls how much referrer information is sent with requests
		// Behavior:
		// Same-origin requests: Send full URL as referrer
		// Cross-origin HTTPS→HTTPS: Send only origin (domain)
		// Cross-origin HTTPS→HTTP: Send no referrer
		// Protects: Sensitive information in URLs from leaking to third parties
		response.headers.set("Referrer-Policy", "strict-origin-when-cross-origin");

		// CSP acts as a **whitelist** that tells the browser which sources are trusted for different types of content (scripts, styles, images, etc.).
		// Prevents:
		// XSS Attacks - Malicious scripts can't execute
		// Data Injection - Untrusted content blocked
		// Clickjacking - Frame embedding prevented
		// Mixed Content - HTTP resources blocked on HTTPS
		const keycloakUrl = process.env.KEYCLOAK_ISSUER || "http://localhost:18080";
		const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

		const cspDirectives = [
			"default-src 'self'",
			"script-src 'self' 'unsafe-eval' 'unsafe-inline'", // Add back 'unsafe-inline' for Next.js
			"style-src 'self' 'unsafe-inline'",
			"img-src 'self' data: blob: https:",
			"font-src 'self' data: https:",
			`connect-src 'self' ${keycloakUrl} ${apiUrl}`,
			"frame-ancestors 'none'",
			"base-uri 'self'",
			"form-action 'self'",
			"object-src 'none'",
			"upgrade-insecure-requests"
		];

		response.headers.set("Content-Security-Policy", cspDirectives.join("; "));

		return response;
	},
	{
		callbacks: {
			authorized: ({ token, req }) => {
				const pathname = req.nextUrl.pathname;

				// Always allow auth-related endpoints
				// if (pathname.startsWith("/api/auth/")) {
				// 	return true;
				// }
				if (
					pathname.startsWith(`/api/${process.env.NEXT_PUBLIC_API_VERSION}/users/login`)
				) {
					return true;
				}

				// Allow access to sign-in and auth error pages
				// if (pathname === "/sign-in" || pathname === "/auth/error") {
				// 	return true;
				// }
				if (pathname === "/sign-in") {
					return true;
				}

				// Allow access to unauthorized page
				// if (pathname === "/unauthorized") {
				// 	return true;
				// }

				// Allow access to public routes (if we have one)
				// const publicRoutes = ["/about", "/contact", "/privacy", "/terms", "/public"];

				// if (publicRoutes.some((route) => pathname.startsWith(route))) {
				// 	return true;
				// }

				// Allow access to public assets and Next.js internals
				if (
					pathname.startsWith("/_next/") ||
					pathname.startsWith("/images/") ||
					pathname.startsWith("/icons/") ||
					pathname.startsWith("/static/") ||
					pathname === "/favicon.ico" ||
					pathname === "/robots.txt" ||
					pathname === "/sitemap.xml"
				) {
					return true;
				}

				// For API routes (except auth), require authentication
				if (pathname.startsWith("/api/")) {
					return !!token;
				}

				// For all other routes, require authentication
				return !!token;
			}
		}
	}
);

export const config = {
	matcher: [
		"/",
		// Protect all routes except specified exclusions
		"/((?!api/auth|_next/static|_next/image|favicon.ico|robots.txt|sitemap.xml|public/).*)",
		// Handle Keycloak realm redirects
		"/realms/:path*",
		// Protect API routes
		"/api/((?!auth).*)"
	]
};
