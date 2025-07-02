// // import { NextAuthOptions } from "next-auth";
// // import { JWT } from "next-auth/jwt";
// //
// // export const authConfig: NextAuthOptions = {
// // 	providers: [
// // 		{
// // 			id: "keycloak",
// // 			name: "Keycloak",
// // 			type: "oauth",
// // 			wellKnown: `${process.env.KEYCLOAK_ISSUER}/.well-known/openid-configuration`,
// // 			issuer: process.env.KEYCLOAK_ISSUER!,
// // 			clientId: process.env.KEYCLOAK_CLIENT_ID!,
// // 			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 			authorization: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/auth`,
// // 				params: {
// // 					scope: "openid email profile",
// // 					response_type: "code"
// // 				}
// // 			},
// // 			token: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`
// // 			},
// // 			userinfo: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`
// // 			},
// // 			checks: ["pkce", "state"],
// // 			idToken: true,
// // 			profile(profile: any) {
// // 				return {
// // 					id: profile.sub,
// // 					name: profile.name ?? profile.preferred_username,
// // 					email: profile.email,
// // 					image: profile.picture,
// // 					roles: profile.realm_access?.roles || [],
// // 					permissions: profile.permissions || profile.scope?.split(" ") || []
// // 				};
// // 			}
// // 		}
// // 	],
// // 	callbacks: {
// // 		async jwt({ token, account, user, profile }) {
// // 			// Initial sign in
// // 			if (account && user) {
// // 				token.accessToken = account.access_token;
// // 				token.refreshToken = account.refresh_token;
// // 				token.expiresAt = account.expires_at;
// // 				token.idToken = account.id_token;
// //
// // 				// Extract roles and permissions from Keycloak token
// // 				const keycloakToken = account.access_token;
// // 				if (keycloakToken) {
// // 					try {
// // 						// Decode JWT to extract roles and permissions
// // 						const payload = JSON.parse(
// // 							Buffer.from(keycloakToken.split(".")[1], "base64").toString()
// // 						);
// //
// // 						// Extract roles from different possible locations
// // 						const realmRoles = payload.realm_access?.roles || [];
// // 						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 							(client: any) => client.roles || []
// // 						);
// //
// // 						token.roles = [...realmRoles, ...clientRoles];
// //
// // 						// Extract custom permissions if configured in Keycloak
// // 						token.permissions = payload.permissions || payload.scope?.split(" ") || [];
// //
// // 						// Store user info
// // 						token.userId = payload.sub;
// // 						token.email = payload.email;
// // 						token.name = payload.name || payload.preferred_username;
// // 					} catch (error) {
// // 						console.error("Error parsing Keycloak token:", error);
// // 						token.roles = [];
// // 						token.permissions = [];
// // 					}
// // 				} else {
// // 					// Fallback to user object if available
// // 					token.roles = user.roles || [];
// // 					token.permissions = user.permissions || [];
// // 				}
// // 			}
// //
// // 			// Return previous token if the access token has not expired yet
// // 			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
// // 				return token;
// // 			}
// //
// // 			// Access token has expired, try to update it
// // 			return refreshAccessToken(token);
// // 		},
// //
// // 		async session({ session, token }) {
// // 			// Send properties to the client
// // 			session.accessToken = token.accessToken as string;
// // 			session.refreshToken = token.refreshToken as string;
// // 			session.error = token.error as string;
// //
// // 			if (session.user) {
// // 				session.user.id = token.userId as string;
// // 				session.user.roles = token.roles as string[];
// // 				session.user.permissions = token.permissions as string[];
// // 			}
// //
// // 			return session;
// // 		}
// // 	},
// // 	pages: {
// // 		signIn: "/sign-in",
// // 		error: "/auth/error"
// // 	},
// // 	session: {
// // 		strategy: "jwt",
// // 		maxAge: 30 * 24 * 60 * 60 // 30 days
// // 	},
// // 	secret: process.env.NEXTAUTH_SECRET,
// // 	debug: process.env.NODE_ENV === "development",
// // 	events: {
// // 		async signOut({ token }) {
// // 			// Optional: Logout from Keycloak when user signs out
// // 			if (token.idToken) {
// // 				try {
// // 					const logoutUrl = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/logout`;
// // 					const params = new URLSearchParams({
// // 						id_token_hint: token.idToken as string,
// // 						post_logout_redirect_uri:
// // 							process.env.NEXTAUTH_URL || "http://localhost:3000"
// // 					});
// //
// // 					await fetch(`${logoutUrl}?${params.toString()}`, { method: "GET" });
// // 				} catch (error) {
// // 					console.error("Error logging out from Keycloak:", error);
// // 				}
// // 			}
// // 		}
// // 	}
// // };
// //
// // async function refreshAccessToken(token: JWT) {
// // 	try {
// // 		const url = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`;
// //
// // 		const response = await fetch(url, {
// // 			headers: {
// // 				"Content-Type": "application/x-www-form-urlencoded"
// // 			},
// // 			method: "POST",
// // 			body: new URLSearchParams({
// // 				client_id: process.env.KEYCLOAK_CLIENT_ID!,
// // 				client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 				grant_type: "refresh_token",
// // 				refresh_token: token.refreshToken as string
// // 			})
// // 		});
// //
// // 		const refreshedTokens = await response.json();
// //
// // 		if (!response.ok) {
// // 			throw refreshedTokens;
// // 		}
// //
// // 		// Update roles and permissions from new token
// // 		let newRoles = token.roles;
// // 		let newPermissions = token.permissions;
// //
// // 		if (refreshedTokens.access_token) {
// // 			try {
// // 				const payload = JSON.parse(
// // 					Buffer.from(refreshedTokens.access_token.split(".")[1], "base64").toString()
// // 				);
// // 				const realmRoles = payload.realm_access?.roles || [];
// // 				const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 					(client: any) => client.roles || []
// // 				);
// //
// // 				newRoles = [...realmRoles, ...clientRoles];
// // 				newPermissions = payload.permissions || payload.scope?.split(" ") || [];
// // 			} catch (error) {
// // 				console.error("Error parsing refreshed token:", error);
// // 			}
// // 		}
// //
// // 		return {
// // 			...token,
// // 			accessToken: refreshedTokens.access_token,
// // 			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
// // 			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
// // 			roles: newRoles,
// // 			permissions: newPermissions,
// // 			error: undefined
// // 		};
// // 	} catch (error) {
// // 		console.error("Error refreshing access token:", error);
// //
// // 		return {
// // 			...token,
// // 			error: "RefreshAccessTokenError"
// // 		};
// // 	}
// // }
//
// // import { NextAuthOptions } from "next-auth";
// // import { JWT } from "next-auth/jwt";
// //
// // export const authConfig: NextAuthOptions = {
// // 	providers: [
// // 		{
// // 			id: "keycloak",
// // 			name: "Keycloak",
// // 			type: "oauth",
// // 			wellKnown: `${process.env.KEYCLOAK_ISSUER}/.well-known/openid-configuration`,
// // 			issuer: process.env.KEYCLOAK_ISSUER!,
// // 			clientId: process.env.KEYCLOAK_CLIENT_ID!,
// // 			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 			authorization: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/auth`,
// // 				params: {
// // 					scope: "openid email profile",
// // 					response_type: "code"
// // 				}
// // 			},
// // 			token: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`
// // 			},
// // 			userinfo: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`
// // 			},
// // 			checks: ["pkce", "state"],
// // 			idToken: true,
// // 			profile(profile: any) {
// // 				return {
// // 					id: profile.sub,
// // 					name: profile.name ?? profile.preferred_username,
// // 					email: profile.email,
// // 					image: profile.picture,
// // 					// Extract roles from the profile (this is from ID token)
// // 					roles: profile.realm_access?.roles || [],
// // 					permissions: profile.permissions || profile.scope?.split(" ") || []
// // 				};
// // 			}
// // 		}
// // 	],
// // 	callbacks: {
// // 		async jwt({ token, account, user }) {
// // 			// Initial sign in
// // 			if (account && user) {
// // 				token.accessToken = account.access_token;
// // 				token.refreshToken = account.refresh_token;
// // 				token.expiresAt = account.expires_at;
// // 				token.idToken = account.id_token;
// //
// // 				// Extract roles and permissions from Keycloak access token
// // 				if (account.access_token) {
// // 					try {
// // 						// Decode JWT to extract roles and permissions
// // 						const payload = JSON.parse(
// // 							Buffer.from(account.access_token.split(".")[1], "base64").toString()
// // 						);
// //
// // 						// Extract roles from different possible locations
// // 						const realmRoles = payload.realm_access?.roles || [];
// // 						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 							(client: any) => client.roles || []
// // 						);
// //
// // 						token.roles = [...realmRoles, ...clientRoles];
// //
// // 						// Extract custom permissions if configured in Keycloak
// // 						token.permissions = payload.permissions || payload.scope?.split(" ") || [];
// //
// // 						// Store user info
// // 						token.userId = payload.sub;
// // 						token.email = payload.email;
// // 						token.name = payload.name || payload.preferred_username;
// //
// // 						console.log("Extracted roles:", token.roles);
// // 						console.log("Extracted permissions:", token.permissions);
// // 					} catch (error) {
// // 						console.error("Error parsing Keycloak token:", error);
// // 						token.roles = [];
// // 						token.permissions = [];
// // 					}
// // 				} else {
// // 					// Fallback to user object if available
// // 					token.roles = user.roles || [];
// // 					token.permissions = user.permissions || [];
// // 				}
// // 			}
// //
// // 			// Return previous token if the access token has not expired yet
// // 			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
// // 				return token;
// // 			}
// //
// // 			// Access token has expired, try to update it
// // 			return refreshAccessToken(token);
// // 		},
// //
// // 		async session({ session, token }) {
// // 			// Send properties to the client
// // 			session.accessToken = token.accessToken as string;
// // 			session.refreshToken = token.refreshToken as string;
// // 			session.error = token.error as string;
// //
// // 			if (session.user) {
// // 				session.user.id = token.userId as string;
// // 				session.user.roles = token.roles as string[];
// // 				session.user.permissions = token.permissions as string[];
// // 			}
// //
// // 			return session;
// // 		}
// // 	},
// // 	pages: {
// // 		signIn: "/sign-in",
// // 		error: "/auth/error"
// // 	},
// // 	session: {
// // 		strategy: "jwt",
// // 		maxAge: 30 * 24 * 60 * 60 // 30 days
// // 	},
// // 	secret: process.env.NEXTAUTH_SECRET,
// // 	debug: process.env.NODE_ENV === "development",
// // 	events: {
// // 		async signOut({ token }) {
// // 			// Optional: Logout from Keycloak when user signs out
// // 			if (token.idToken) {
// // 				try {
// // 					const logoutUrl = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/logout`;
// // 					const params = new URLSearchParams({
// // 						id_token_hint: token.idToken as string,
// // 						post_logout_redirect_uri:
// // 							process.env.NEXTAUTH_URL || "http://localhost:3000"
// // 					});
// //
// // 					await fetch(`${logoutUrl}?${params.toString()}`, { method: "GET" });
// // 				} catch (error) {
// // 					console.error("Error logging out from Keycloak:", error);
// // 				}
// // 			}
// // 		}
// // 	}
// // };
// //
// // async function refreshAccessToken(token: JWT) {
// // 	try {
// // 		const url = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`;
// //
// // 		const response = await fetch(url, {
// // 			headers: {
// // 				"Content-Type": "application/x-www-form-urlencoded"
// // 			},
// // 			method: "POST",
// // 			body: new URLSearchParams({
// // 				client_id: process.env.KEYCLOAK_CLIENT_ID!,
// // 				client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 				grant_type: "refresh_token",
// // 				refresh_token: token.refreshToken as string
// // 			})
// // 		});
// //
// // 		const refreshedTokens = await response.json();
// //
// // 		if (!response.ok) {
// // 			throw refreshedTokens;
// // 		}
// //
// // 		// Update roles and permissions from new token
// // 		let newRoles = token.roles;
// // 		let newPermissions = token.permissions;
// //
// // 		if (refreshedTokens.access_token) {
// // 			try {
// // 				const payload = JSON.parse(
// // 					Buffer.from(refreshedTokens.access_token.split(".")[1], "base64").toString()
// // 				);
// // 				const realmRoles = payload.realm_access?.roles || [];
// // 				const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 					(client: any) => client.roles || []
// // 				);
// //
// // 				newRoles = [...realmRoles, ...clientRoles];
// // 				newPermissions = payload.permissions || payload.scope?.split(" ") || [];
// // 			} catch (error) {
// // 				console.error("Error parsing refreshed token:", error);
// // 			}
// // 		}
// //
// // 		return {
// // 			...token,
// // 			accessToken: refreshedTokens.access_token,
// // 			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
// // 			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
// // 			roles: newRoles,
// // 			permissions: newPermissions,
// // 			error: undefined
// // 		};
// // 	} catch (error) {
// // 		console.error("Error refreshing access token:", error);
// //
// // 		return {
// // 			...token,
// // 			error: "RefreshAccessTokenError"
// // 		};
// // 	}
// // }
//
// // WORKS
// // import { NextAuthOptions } from "next-auth";
// // import { JWT } from "next-auth/jwt";
// //
// // export const authConfig: NextAuthOptions = {
// // 	providers: [
// // 		{
// // 			id: "keycloak",
// // 			name: "Keycloak",
// // 			type: "oauth",
// // 			wellKnown: `${process.env.KEYCLOAK_ISSUER}/.well-known/openid-configuration`,
// // 			issuer: process.env.KEYCLOAK_ISSUER!,
// // 			clientId: process.env.KEYCLOAK_CLIENT_ID!,
// // 			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 			authorization: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/auth`,
// // 				params: {
// // 					scope: "openid email profile",
// // 					response_type: "code"
// // 				}
// // 			},
// // 			token: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`
// // 			},
// // 			userinfo: {
// // 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`
// // 			},
// // 			checks: ["pkce", "state"],
// // 			idToken: true,
// // 			profile(profile: any) {
// // 				return {
// // 					id: profile.sub,
// // 					name: profile.name ?? profile.preferred_username,
// // 					email: profile.email,
// // 					image: profile.picture,
// // 					roles: profile.realm_access?.roles || [],
// // 					permissions: profile.permissions || profile.scope?.split(" ") || []
// // 				};
// // 			}
// // 		}
// // 	],
// // 	callbacks: {
// // 		async jwt({ token, account, user }) {
// // 			// Initial sign in
// // 			if (account && user) {
// // 				// Store tokens but don't put full access token in session
// // 				token.accessTokenHash = account.access_token
// // 					? Buffer.from(account.access_token).toString("base64").slice(0, 32)
// // 					: null;
// // 				token.refreshToken = account.refresh_token;
// // 				token.expiresAt = account.expires_at;
// // 				token.idToken = account.id_token;
// //
// // 				// Extract and store only essential data
// // 				if (account.access_token) {
// // 					try {
// // 						const payload = JSON.parse(
// // 							Buffer.from(account.access_token.split(".")[1], "base64").toString()
// // 						);
// //
// // 						const realmRoles = payload.realm_access?.roles || [];
// // 						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 							(client: any) => client.roles || []
// // 						);
// //
// // 						// Store only essential user data
// // 						token.roles = [...realmRoles, ...clientRoles];
// // 						token.permissions = payload.permissions || payload.scope?.split(" ") || [];
// // 						token.userId = payload.sub;
// // 						token.email = payload.email;
// // 						token.name = payload.name || payload.preferred_username;
// //
// // 						// Don't store the full access token in session - retrieve it when needed
// // 					} catch (error) {
// // 						console.error("Error parsing Keycloak token:", error);
// // 						token.roles = [];
// // 						token.permissions = [];
// // 					}
// // 				}
// // 			}
// //
// // 			// Check if token needs refresh
// // 			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
// // 				return token;
// // 			}
// //
// // 			return refreshAccessToken(token);
// // 		},
// // 		async redirect({ url, baseUrl }) {
// // 			// Fixes redirect loop issues
// // 			if (url.startsWith("/")) {
// // 				return `${baseUrl}${url}`;
// // 			}
// // 			// Allow callback URLs on the same origin
// // 			if (new URL(url).origin === baseUrl) {
// // 				return url;
// // 			}
// // 			return baseUrl;
// // 		},
// //
// // 		async session({ session, token }) {
// // 			// Send only essential properties to client
// // 			session.error = token.error as string;
// //
// // 			if (session.user) {
// // 				session.user.id = token.userId as string;
// // 				session.user.roles = token.roles as string[];
// // 				session.user.permissions = token.permissions as string[];
// // 			}
// //
// // 			// Don't send tokens to client for security
// // 			return session;
// // 		}
// // 	},
// // 	pages: {
// // 		signIn: "/sign-in",
// // 		error: "/auth/error"
// // 	},
// // 	session: {
// // 		strategy: "jwt",
// // 		maxAge: 30 * 24 * 60 * 60, // 30 days
// // 		updateAge: 24 * 60 * 60 // 24 hours
// // 	},
// // 	cookies: {
// // 		sessionToken: {
// // 			name: `next-auth.session-token`,
// // 			options: {
// // 				httpOnly: true,
// // 				sameSite: "lax",
// // 				path: "/",
// // 				secure: process.env.NODE_ENV === "production",
// // 				maxAge: 30 * 24 * 60 * 60 // 30 days
// // 			}
// // 		}
// // 	},
// // 	secret: process.env.NEXTAUTH_SECRET,
// // 	debug: process.env.NODE_ENV === "development"
// // };
// //
// // async function refreshAccessToken(token: JWT) {
// // 	try {
// // 		const response = await fetch(
// // 			`${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`,
// // 			{
// // 				headers: {
// // 					"Content-Type": "application/x-www-form-urlencoded"
// // 				},
// // 				method: "POST",
// // 				body: new URLSearchParams({
// // 					client_id: process.env.KEYCLOAK_CLIENT_ID!,
// // 					client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
// // 					grant_type: "refresh_token",
// // 					refresh_token: token.refreshToken as string
// // 				})
// // 			}
// // 		);
// //
// // 		const refreshedTokens = await response.json();
// //
// // 		if (!response.ok) {
// // 			throw refreshedTokens;
// // 		}
// //
// // 		// Update with new tokens and extract roles again
// // 		let newRoles = token.roles;
// // 		let newPermissions = token.permissions;
// //
// // 		if (refreshedTokens.access_token) {
// // 			try {
// // 				const payload = JSON.parse(
// // 					Buffer.from(refreshedTokens.access_token.split(".")[1], "base64").toString()
// // 				);
// // 				const realmRoles = payload.realm_access?.roles || [];
// // 				const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// // 					(client: any) => client.roles || []
// // 				);
// //
// // 				newRoles = [...realmRoles, ...clientRoles];
// // 				newPermissions = payload.permissions || payload.scope?.split(" ") || [];
// // 			} catch (error) {
// // 				console.error("Error parsing refreshed token:", error);
// // 			}
// // 		}
// //
// // 		return {
// // 			...token,
// // 			accessTokenHash: refreshedTokens.access_token
// // 				? Buffer.from(refreshedTokens.access_token).toString("base64").slice(0, 32)
// // 				: token.accessTokenHash,
// // 			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
// // 			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
// // 			roles: newRoles,
// // 			permissions: newPermissions,
// // 			error: undefined
// // 		};
// // 	} catch (error) {
// // 		console.error("Error refreshing access token:", error);
// // 		return {
// // 			...token,
// // 			error: "RefreshAccessTokenError"
// // 		};
// // 	}
// // }
//
// import { NextAuthOptions } from "next-auth";
// import { JWT } from "next-auth/jwt";
// import CredentialsProvider from "next-auth/providers/credentials";
//
// export const authConfig: NextAuthOptions = {
// 	providers: [
// 		// Keep your existing Keycloak provider
// 		{
// 			id: "keycloak",
// 			name: "Keycloak",
// 			type: "oauth",
// 			wellKnown: `${process.env.KEYCLOAK_ISSUER}/.well-known/openid-configuration`,
// 			issuer: process.env.KEYCLOAK_ISSUER!,
// 			clientId: process.env.KEYCLOAK_CLIENT_ID!,
// 			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
// 			authorization: {
// 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/auth`,
// 				params: {
// 					scope: "openid email profile",
// 					response_type: "code"
// 				}
// 			},
// 			token: {
// 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`
// 			},
// 			userinfo: {
// 				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`
// 			},
// 			checks: ["pkce", "state"],
// 			idToken: true,
// 			profile(profile: any) {
// 				return {
// 					id: profile.sub,
// 					name: profile.name ?? profile.preferred_username,
// 					email: profile.email,
// 					image: profile.picture,
// 					roles: profile.realm_access?.roles || [],
// 					permissions: profile.permissions || profile.scope?.split(" ") || []
// 				};
// 			}
// 		},
// 		// Add credentials provider for direct login
// 		CredentialsProvider({
// 			id: "credentials",
// 			name: "credentials",
// 			credentials: {
// 				token: { label: "Token", type: "text" }
// 			},
// 			async authorize(credentials) {
// 				if (!credentials?.token) {
// 					return null;
// 				}
//
// 				try {
// 					// Verify the token with your backend or decode JWT
// 					const payload = JSON.parse(
// 						Buffer.from(credentials.token.split(".")[1], "base64").toString()
// 					);
//
// 					return {
// 						id: payload.sub || payload.userId,
// 						name: payload.name || payload.username,
// 						email: payload.email,
// 						roles: payload.roles || [],
// 						permissions: payload.permissions || []
// 					};
// 				} catch (error) {
// 					console.error("Token verification failed:", error);
// 					return null;
// 				}
// 			}
// 		})
// 	],
// 	callbacks: {
// 		async jwt({ token, account, user }) {
// 			// Handle credentials provider
// 			if (account?.provider === "credentials" && user) {
// 				token.userId = user.id;
// 				token.email = user.email;
// 				token.name = user.name;
// 				token.roles = user.roles;
// 				token.permissions = user.permissions;
// 				return token;
// 			}
//
// 			// Keep existing Keycloak logic
// 			if (account && user && account.provider === "keycloak") {
// 				token.accessTokenHash = account.access_token
// 					? Buffer.from(account.access_token).toString("base64").slice(0, 32)
// 					: null;
// 				token.refreshToken = account.refresh_token;
// 				token.expiresAt = account.expires_at;
// 				token.idToken = account.id_token;
//
// 				if (account.access_token) {
// 					try {
// 						const payload = JSON.parse(
// 							Buffer.from(account.access_token.split(".")[1], "base64").toString()
// 						);
//
// 						const realmRoles = payload.realm_access?.roles || [];
// 						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// 							(client: any) => client.roles || []
// 						);
//
// 						token.roles = [...realmRoles, ...clientRoles];
// 						token.permissions = payload.permissions || payload.scope?.split(" ") || [];
// 						token.userId = payload.sub;
// 						token.email = payload.email;
// 						token.name = payload.name || payload.preferred_username;
// 					} catch (error) {
// 						console.error("Error parsing Keycloak token:", error);
// 						token.roles = [];
// 						token.permissions = [];
// 					}
// 				}
// 			}
//
// 			// For credentials provider, don't refresh tokens
// 			if (!token.refreshToken) {
// 				return token;
// 			}
//
// 			// Check if token needs refresh (only for Keycloak)
// 			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
// 				return token;
// 			}
//
// 			return refreshAccessToken(token);
// 		},
// 		async redirect({ url, baseUrl }) {
// 			if (url.startsWith("/")) {
// 				return `${baseUrl}${url}`;
// 			}
// 			if (new URL(url).origin === baseUrl) {
// 				return url;
// 			}
// 			return baseUrl;
// 		},
// 		async session({ session, token }) {
// 			session.error = token.error as string;
//
// 			if (session.user) {
// 				session.user.id = token.userId as string;
// 				session.user.roles = token.roles as string[];
// 				session.user.permissions = token.permissions as string[];
// 			}
//
// 			return session;
// 		}
// 	},
// 	pages: {
// 		signIn: "/sign-in",
// 		error: "/auth/error"
// 	},
// 	session: {
// 		strategy: "jwt",
// 		maxAge: 30 * 24 * 60 * 60,
// 		updateAge: 24 * 60 * 60
// 	},
// 	cookies: {
// 		sessionToken: {
// 			name: `next-auth.session-token`,
// 			options: {
// 				httpOnly: true,
// 				sameSite: "lax",
// 				path: "/",
// 				secure: process.env.NODE_ENV === "production",
// 				maxAge: 30 * 24 * 60 * 60
// 			}
// 		}
// 	},
// 	secret: process.env.NEXTAUTH_SECRET,
// 	debug: process.env.NODE_ENV === "development"
// };
//
// // Keep existing refreshAccessToken function
// async function refreshAccessToken(token: JWT) {
// 	try {
// 		const response = await fetch(
// 			`${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`,
// 			{
// 				headers: {
// 					"Content-Type": "application/x-www-form-urlencoded"
// 				},
// 				method: "POST",
// 				body: new URLSearchParams({
// 					client_id: process.env.KEYCLOAK_CLIENT_ID!,
// 					client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
// 					grant_type: "refresh_token",
// 					refresh_token: token.refreshToken as string
// 				})
// 			}
// 		);
//
// 		const refreshedTokens = await response.json();
//
// 		if (!response.ok) {
// 			throw refreshedTokens;
// 		}
//
// 		let newRoles = token.roles;
// 		let newPermissions = token.permissions;
//
// 		if (refreshedTokens.access_token) {
// 			try {
// 				const payload = JSON.parse(
// 					Buffer.from(refreshedTokens.access_token.split(".")[1], "base64").toString()
// 				);
// 				const realmRoles = payload.realm_access?.roles || [];
// 				const clientRoles = Object.values(payload.resource_access || {}).flatMap(
// 					(client: any) => client.roles || []
// 				);
//
// 				newRoles = [...realmRoles, ...clientRoles];
// 				newPermissions = payload.permissions || payload.scope?.split(" ") || [];
// 			} catch (error) {
// 				console.error("Error parsing refreshed token:", error);
// 			}
// 		}
//
// 		return {
// 			...token,
// 			accessTokenHash: refreshedTokens.access_token
// 				? Buffer.from(refreshedTokens.access_token).toString("base64").slice(0, 32)
// 				: token.accessTokenHash,
// 			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
// 			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
// 			roles: newRoles,
// 			permissions: newPermissions,
// 			error: undefined
// 		};
// 	} catch (error) {
// 		console.error("Error refreshing access token:", error);
// 		return {
// 			...token,
// 			error: "RefreshAccessTokenError"
// 		};
// 	}
// }

// TODO
// Can remove
