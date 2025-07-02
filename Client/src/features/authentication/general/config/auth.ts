import { NextAuthOptions } from "next-auth";
import { JWT } from "next-auth/jwt";

import CredentialsProvider from "next-auth/providers/credentials";

export const authConfig: NextAuthOptions = {
	providers: [
		// Keep your existing Keycloak provider
		{
			id: "keycloak",
			name: "Keycloak",
			type: "oauth",
			wellKnown: `${process.env.KEYCLOAK_ISSUER}/.well-known/openid-configuration`,
			issuer: process.env.KEYCLOAK_ISSUER!,
			clientId: process.env.KEYCLOAK_CLIENT_ID!,
			clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
			authorization: {
				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/auth`,
				params: {
					scope: "openid email profile",
					response_type: "code"
				}
			},
			token: {
				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`
			},
			userinfo: {
				url: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`
			},
			checks: ["pkce", "state"],
			idToken: true,
			profile(profile: any) {
				return {
					id: profile.sub,
					name: profile.name ?? profile.preferred_username,
					email: profile.email,
					image: profile.picture,
					roles: profile.realm_access?.roles || [],
					permissions: profile.permissions || profile.scope?.split(" ") || []
				};
			}
		},
		// Add credentials provider for direct login
		CredentialsProvider({
			id: "credentials",
			name: "credentials",
			credentials: {
				token: { label: "Token", type: "text" }
			},
			async authorize(credentials) {
				if (!credentials?.token) {
					return null;
				}

				try {
					// Verify the token with your backend or decode JWT
					const payload = JSON.parse(
						Buffer.from(credentials.token.split(".")[1], "base64").toString()
					);

					return {
						id: payload.sub || payload.userId,
						name: payload.name || payload.username,
						email: payload.email,
						roles: payload.roles || [],
						permissions: payload.permissions || []
					};
				} catch (error) {
					console.error("Token verification failed:", error);
					return null;
				}
			}
		})
	],
	callbacks: {
		async jwt({ token, account, user }) {
			// Handle credentials provider
			if (account?.provider === "credentials" && user) {
				token.userId = user.id;
				token.email = user.email;
				token.name = user.name;
				token.roles = user.roles;
				token.permissions = user.permissions;
				return token;
			}

			// Keep existing Keycloak logic
			if (account && user && account.provider === "keycloak") {
				token.accessTokenHash = account.access_token
					? Buffer.from(account.access_token).toString("base64").slice(0, 32)
					: null;
				token.refreshToken = account.refresh_token;
				token.expiresAt = account.expires_at;
				token.idToken = account.id_token;

				if (account.access_token) {
					try {
						const payload = JSON.parse(
							Buffer.from(account.access_token.split(".")[1], "base64").toString()
						);

						const realmRoles = payload.realm_access?.roles || [];
						const clientRoles = Object.values(payload.resource_access || {}).flatMap(
							(client: any) => client.roles || []
						);

						token.roles = [...realmRoles, ...clientRoles];
						token.permissions = payload.permissions || payload.scope?.split(" ") || [];
						token.userId = payload.sub;
						token.email = payload.email;
						token.name = payload.name || payload.preferred_username;
					} catch (error) {
						console.error("Error parsing Keycloak token:", error);
						token.roles = [];
						token.permissions = [];
					}
				}
			}

			// For credentials provider, don't refresh tokens
			if (!token.refreshToken) {
				return token;
			}

			// Check if token needs refresh (only for Keycloak)
			if (token.expiresAt && Date.now() < (token.expiresAt as number) * 1000) {
				return token;
			}

			return refreshAccessToken(token);
		},
		async redirect({ url, baseUrl }) {
			if (url.startsWith("/")) {
				return `${baseUrl}${url}`;
			}
			if (new URL(url).origin === baseUrl) {
				return url;
			}
			return baseUrl;
		},
		async session({ session, token }) {
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
		maxAge: 30 * 24 * 60 * 60,
		updateAge: 24 * 60 * 60
	},
	cookies: {
		sessionToken: {
			name: `next-auth.session-token`,
			options: {
				httpOnly: true,
				sameSite: "lax",
				path: "/",
				secure: process.env.NODE_ENV === "production",
				maxAge: 30 * 24 * 60 * 60
			}
		}
	},
	secret: process.env.NEXTAUTH_SECRET,
	debug: process.env.NODE_ENV === "development"
};

// Keep existing refreshAccessToken function
async function refreshAccessToken(token: JWT) {
	try {
		const response = await fetch(
			`${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`,
			{
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
			}
		);

		const refreshedTokens = await response.json();

		if (!response.ok) {
			throw refreshedTokens;
		}

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
			accessTokenHash: refreshedTokens.access_token
				? Buffer.from(refreshedTokens.access_token).toString("base64").slice(0, 32)
				: token.accessTokenHash,
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
