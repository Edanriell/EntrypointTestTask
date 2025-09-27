"use client";

import { FC } from "react";

import { useCredentialsAuth } from "../../general";

export const SignOut: FC = () => {
	const { logout } = useCredentialsAuth();

	return (
		<span className="w-full h-full" onClick={logout}>
			Sign Out
		</span>
	);
};
