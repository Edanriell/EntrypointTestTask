"use client";

import { FC } from "react";

import { useCredentialsAuth } from "@features/authentication/general";

export const SignOut: FC = () => {
	const { logout } = useCredentialsAuth();

	return (
		<span className="w-full h-full" onClick={logout}>
			Sign Out
		</span>
	);
};
