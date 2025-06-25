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
		async jwt({ token, account, profile }) {
			// Persist the OAuth access_token and refresh_token to the token right after signin
			if (account) {
				token.accessToken = account.access_token;
				token.refreshToken = account.refresh_token;
				token.expiresAt = account.expires_at;
				token.idToken = account.id_token;
			}

			// Return previous token if the access token has not expired yet
			if (Date.now() < (token.expiresAt as number) * 1000) {
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

			return session;
		}
	},
	// TODO
	// FIX URL
	pages: {
		signIn: "/auth/signin",
		error: "/auth/error"
	},
	session: {
		strategy: "jwt"
	},
	secret: process.env.NEXTAUTH_SECRET
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

		return {
			...token,
			accessToken: refreshedTokens.access_token,
			expiresAt: Math.floor(Date.now() / 1000 + refreshedTokens.expires_in),
			refreshToken: refreshedTokens.refresh_token ?? token.refreshToken
		};
	} catch (error) {
		console.error("Error refreshing access token:", error);

		return {
			...token,
			error: "RefreshAccessTokenError"
		};
	}
}
