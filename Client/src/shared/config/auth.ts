import { NextAuthOptions } from "next-auth";
import KeycloakProvider from "next-auth/providers/keycloak";
import { JWT } from "next-auth/jwt";

export const authConfig: NextAuthOptions = {
	providers: [
		KeycloakProvider({
			clientId: process.env.KEYCLOAK_CLIENT_ID!,
			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
			issuer: process.env.KEYCLOAK_ISSUER!
		})
	],
	callbacks: {
		async jwt({ token, account, user, profile }) {
			// Initial sign in
			if (account && user) {
				token.accessToken = account.access_token;
				token.refreshToken = account.refresh_token;
				token.expiresAt = account.expires_at;
				token.idToken = account.id_token;

				// Extract roles and permissions from Keycloak token
				// Keycloak typically stores roles in different places depending on configuration
				const keycloakToken = account.access_token;
				if (keycloakToken) {
					try {
						// Decode JWT to extract roles and permissions
						const payload = JSON.parse(
							Buffer.from(keycloakToken.split(".")[1], "base64").toString()
						);

						// Extract roles from different possible locations
						const realmRoles = payload.realm_access?.roles || [];
						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
							(client: any) => client.roles || []
						);

						token.roles = [...realmRoles, ...clientRoles];

						// Extract custom permissions if configured in Keycloak
						token.permissions = payload.permissions || payload.scope?.split(" ") || [];

						// Store user info
						token.userId = payload.sub;
						token.email = payload.email;
						token.name = payload.name || payload.preferred_username;
					} catch (error) {
						console.error("Error parsing Keycloak token:", error);
						token.roles = [];
						token.permissions = [];
					}
				} else {
					// Fallback to user object if available
					token.roles = user.roles || [];
					token.permissions = user.permissions || [];
				}
			}

			// Return previous token if the access token has not expired yet
			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
				return token;
			}

			// Access token has expired, try to update it
			return refreshAccessToken(token);
		},

		async session({ session, token }) {
			// Send properties to the client
			session.accessToken = token.accessToken as string;
			session.refreshToken = token.refreshToken as string;
			session.error = token.error as string;

			if (session.user) {
				session.user.id = token.userId as string;
				session.user.roles = token.roles as string[];
				session.user.permissions = token.permissions as string[];
			}

			return session;
		}
	},
	pages: {
		signIn: "/sign-in",
		error: "/auth/error"
	},
	session: {
		strategy: "jwt",
		maxAge: 30 * 24 * 60 * 60 // 30 days
	},
	secret: process.env.NEXTAUTH_SECRET,
	debug: process.env.NODE_ENV === "development",
	events: {
		async signOut({ token }) {
			// Optional: Logout from Keycloak when user signs out
			if (token.idToken) {
				try {
					const logoutUrl = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/logout`;
					const params = new URLSearchParams({
						id_token_hint: token.idToken as string,
						post_logout_redirect_uri:
							process.env.NEXTAUTH_URL || "http://localhost:3000"
					});

					await fetch(`${logoutUrl}?${params.toString()}`, { method: "GET" });
				} catch (error) {
					console.error("Error logging out from Keycloak:", error);
				}
			}
		}
	}
};

async function refreshAccessToken(token: JWT) {
	try {
		const url = `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`;

		const response = await fetch(url, {
			headers: {
				"Content-Type": "application/x-www-form-urlencoded"
			},
			method: "POST",
			body: new URLSearchParams({
				client_id: process.env.KEYCLOAK_CLIENT_ID!,
				client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
				grant_type: "refresh_token",
				refresh_token: token.refreshToken as string
			})
		});

		const refreshedTokens = await response.json();

		if (!response.ok) {
			throw refreshedTokens;
		}

		// Update roles and permissions from new token
		let newRoles = token.roles;
		let newPermissions = token.permissions;

		if (refreshedTokens.access_token) {
			try {
				const payload = JSON.parse(
					Buffer.from(refreshedTokens.access_token.split(".")[1], "base64").toString()
				);
				const realmRoles = payload.realm_access?.roles || [];
				const clientRoles = Object.values(payload.resource_access || {}).flatMap(
					(client: any) => client.roles || []
				);

				newRoles = [...realmRoles, ...clientRoles];
				newPermissions = payload.permissions || payload.scope?.split(" ") || [];
			} catch (error) {
				console.error("Error parsing refreshed token:", error);
			}
		}

		return {
			...token,
			accessToken: refreshedTokens.access_token,
			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken,
			roles: newRoles,
			permissions: newPermissions,
			error: undefined
		};
	} catch (error) {
		console.error("Error refreshing access token:", error);

		return {
			...token,
			error: "RefreshAccessTokenError"
		};
	}
}
