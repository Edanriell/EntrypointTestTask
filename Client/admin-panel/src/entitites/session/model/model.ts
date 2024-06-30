import { getServerSession, NextAuthOptions } from "next-auth";
import CredentialsProvider from "next-auth/providers/credentials";

import { validateObjectFields } from "@shared/lib";

import { getUserInfo, login, refreshTokens } from "@/entitites/session";

type AuthData = {
	accessToken: string;
	expiresIn: number;
	refreshToken: string;
	errors?: Array<string>;
};

type UserData = {
	id: string;
	username: string;
	name: string;
	surname: string;
	photo: string;
	roles: Array<string>;
	email: string;
	isEmailConfirmed: boolean;
};

type Token = {
	accessToken: string;
	accessTokenExpires: number;
	refreshToken: string;
	roles: Array<string>;
};

const requiredUserFields = [
	"id",
	"username",
	"name",
	"surname",
	"photo",
	"roles",
	"email",
	"isEmailConfirmed"
];
const requiredAuthFields = ["accessToken", "expiresIn", "refreshToken"];

export const authOptions: NextAuthOptions = {
	session: {
		strategy: "jwt"
	},
	pages: {
		signIn: "/login"
	},
	providers: [
		CredentialsProvider({
			name: "credentials",
			credentials: {
				username: { label: "username", type: "text" },
				password: { label: "password", type: "password" }
			},
			async authorize(credentials) {
				const loginRes = await login({
					username: credentials!.username,
					password: credentials!.password
				});

				const authData: AuthData = await loginRes.json();

				if (!loginRes.ok) {
					if (loginRes.headers.get("Content-Type")?.includes("application/json")) {
						throw new Error(JSON.stringify(authData.errors));
					} else {
						throw new Error("Unexpected server response");
					}
				}

				const userInfoRes = await getUserInfo(authData.accessToken);

				const userData: UserData = await userInfoRes.json();

				if (!userInfoRes.ok) {
					if (loginRes.headers.get("Content-Type")?.includes("application/json")) {
						throw new Error("Failed to fetch user information");
					} else {
						throw new Error("Unexpected server response");
					}
				}

				const isUserDataValid = validateObjectFields({
					object: userData,
					requiredFields: requiredUserFields
				});
				const isAuthDataValid = validateObjectFields({
					object: authData,
					requiredFields: requiredAuthFields
				});

				// console.log(userData);
				// console.log(authData);
				// console.log(isUserDataValid);
				// console.log(isAuthDataValid);

				if (isUserDataValid && isAuthDataValid) {
					return {
						...userData,
						...authData
					};
				} else {
					console.error("Failed to authenticate: Received invalid user or auth data");
					return null;
				}
			}
		})
	],
	callbacks: {
		async jwt({ token, user }: { token: any; user: any }) {
			// console.log(`${token} tooookeeeeeen`);
			// console.log(token);
			// console.log(`${user} uuuuuuuussssseeeeer`);
			// console.log(user);
			if (user) {
				console.log(user);
				// console.log(user.accessToken);
				token.id = user.id;
				token.username = user.username;
				token.name = user.name;
				token.surname = user.surname;
				token.email = user.email;
				token.isEmailConfirmed = user.isEmailConfirmed;
				token.photo = user.photo;

				token.accessToken = user.accessToken;
				token.refreshToken = user.refreshToken;
				token.accessTokenExpires = Date.now() + user.expiresIn * 1000;
				token.roles = user.roles;
			}

			if (Date.now() < (token as Token).accessTokenExpires) {
				return token;
			}

			return refreshAccessToken(token as Token);
		},
		async session({ session, token }: { session: any; token: any }) {
			// console.log("asdasdsadsadsadaasdasdsdasda");
			// console.log(session);
			// console.log("1232133231");
			// console.log(token);
			if (token) {
				session.user.id = token.id;
				session.user.username = token.username;
				session.user.name = token.name;
				session.user.surname = token.surname;
				session.user.email = token.email;
				session.user.isEmailConfirmed = token.isEmailConfirmed;
				session.user.photo = token.photo;

				session.accessToken = token.accessToken;
				session.roles = token.roles;
			}

			return session;
		}
	},
	secret: process.env.NEXTAUTH_SECRET
};

export const refreshAccessToken = async (token: Token) => {
	const refreshTokensResponse = await refreshTokens({
		accessToken: token.accessToken,
		refreshToken: token.refreshToken
	});
	const refreshedTokens = await refreshTokensResponse.json();

	if (!refreshTokensResponse.ok) {
		if (!refreshTokensResponse.headers.get("Content-Type")?.includes("application/json")) {
			console.error("Unexpected server response");
			return null;
		} else {
			console.error("Could not retrieve new access token");
			return null;
		}
	}

	return {
		...token,
		accessToken: refreshedTokens.accessToken,
		accessTokenExpires: Date.now() + refreshedTokens.expiresIn * 1000,
		refreshToken: refreshedTokens.refreshToken ?? token.refreshToken
	};
};

export const getSession = async () => await getServerSession(authOptions);
