import { FC, ReactNode } from "react";
import type { Metadata } from "next";

import { generateStaticMetadata } from "@shared/lib/utils";

export const metadata: Metadata = generateStaticMetadata({
	title: "Sign In",
	description:
		"Sign in to your account to access your dashboard and manage your products, clients and orders efficiently.",
	ogTitle: "Sign In to Your Account",
	ogDescription: "Access your dashboard to manage products, clients and orders."
});

type AuthLayoutProps = {
	children: ReactNode;
};

export const AuthLayout: FC<AuthLayoutProps> = ({ children }) => {
	return (
		<main className="bg-muted flex min-h-svh flex-col items-center justify-center p-6 md:p-10">
			<div className="w-full max-w-sm md:max-w-3xl">{children}</div>
		</main>
	);
};
