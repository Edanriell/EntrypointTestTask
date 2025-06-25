import "@app/_styles/styles.css";

import { FC, ReactNode } from "react";
import type { Metadata } from "next";
import { Roboto } from "next/font/google";

import { generateStaticMetadata } from "@shared/lib/functions";

const roboto = Roboto({
	subsets: ["latin"]
});

export const metadata: Metadata = generateStaticMetadata({
	title: "Dashboard - Business Management",
	description:
		"Manage your business operations efficiently. Handle orders, clients, and products with comprehensive management tools and real-time insights.",
	ogTitle: "Business Management Dashboard",
	ogDescription:
		"Streamline your business operations with a complete management platform. Order processing, client relationship tools, and product catalog administration in one place.",
	ogImagePath: "/images/banner.jpg"
});

type DefaultLayoutProps = {
	children: ReactNode;
};

export const DefaultLayout: FC<DefaultLayoutProps> = ({ children }) => {
	return (
		<html lang="en" className={roboto.className}>
			<body>{children}</body>
		</html>
	);
};
