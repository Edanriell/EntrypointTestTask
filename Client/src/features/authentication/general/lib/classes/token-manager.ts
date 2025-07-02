import { getServerSession } from "next-auth";

import { authConfig } from "@features/authentication/general/config";

type TokenResponse = {
	access_token: string;
	refresh_token?: string;
	expires_in: number;
};

export class TokenManager {
	public static async getAccessToken(): Promise<string | null> {
		const session = await getServerSession(authConfig);

		if (!session?.refreshToken) {
			return null;
		}

		const tokenResponse = await this.getTokenFromRefresh(session.refreshToken as string);
		return tokenResponse?.access_token || null;
	}

	private static async getTokenFromRefresh(refreshToken: string): Promise<TokenResponse | null> {
		try {
			const response = await fetch(
				`${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`,
				{
					method: "POST",
					headers: {
						"Content-Type": "application/x-www-form-urlencoded"
					},
					body: new URLSearchParams({
						client_id: process.env.KEYCLOAK_CLIENT_ID!,
						client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
						grant_type: "refresh_token",
						refresh_token: refreshToken
					})
				}
			);

			if (!response.ok) {
				return null;
			}

			return await response.json();
		} catch (error) {
			console.error("Error refreshing token:", error);
			return null;
		}
	}
}
