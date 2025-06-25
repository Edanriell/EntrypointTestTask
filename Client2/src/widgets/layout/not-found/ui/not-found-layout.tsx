import { FC, ReactNode } from "react";
import type { Metadata } from "next";

import { generateStaticMetadata } from "@shared/lib/functions";

export const metadata: Metadata = generateStaticMetadata({
	title: "404 - Page Not Found",
	description:
		"The page you are looking for could not be found. Return to the homepage or try again.",
	ogTitle: "Page Not Found",
	ogDescription: "This page does not exist. Navigate back to continue browsing.",
	ogImagePath: "/images/banner.jpg"
});

type AuthLayoutProps = {
	children: ReactNode;
};

export const NotFoundLayout: FC<AuthLayoutProps> = ({ children }) => {
	return (
		<main className="bg-muted flex min-h-svh flex-col items-center justify-center p-6 md:p-10">
			<div className="w-full max-w-sm md:max-w-3xl">{children}</div>
		</main>
	);
};
