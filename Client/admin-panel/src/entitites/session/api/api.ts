type LoginParameters = {
	username: string;
	password: string;
};

type RefreshTokensParameters = {
	accessToken: string;
	refreshToken: string;
};

export const login = async ({ username, password }: LoginParameters) => {
	return await fetch(
		"https://localhost:5001/api/Users/login?UseCookies=false&UseSessionCookies=false",
		{
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({
				userName: username,
				password: password
			})
		}
	);
};

export const getUserInfo = async (accessToken: string) => {
	return await fetch("https://localhost:5001/api/Users/userInfo", {
		method: "GET",
		headers: {
			"Content-Type": "application/json",
			Authorization: `Bearer ${accessToken}` // Use the existing access token
		}
	});
};

export const refreshTokens = async ({ accessToken, refreshToken }: RefreshTokensParameters) => {
	return await fetch("https://localhost:5001/api/Users/refresh", {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
			Authorization: `Bearer ${accessToken}` // Use the existing access token
		},
		body: JSON.stringify({
			refreshToken: refreshToken
		})
	});
};
