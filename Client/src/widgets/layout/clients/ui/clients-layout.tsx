import { FC, ReactNode } from "react";
import type { Metadata } from "next";

import { generateStaticMetadata } from "@shared/lib/utils";

export const metadata: Metadata = generateStaticMetadata({
	title: "Clients - Business Management",
	description:
		"Manage your client relationships effectively. View, organize, and maintain your customer database with comprehensive client management tools.",
	ogTitle: "Client Management Dashboard",
	ogDescription:
		"Streamline your client relationships with powerful management tools. Access customer information, track interactions, and maintain your client database efficiently."
});

type ClientsLayoutProps = {
	children: ReactNode;
};

export const ClientsLayout: FC<ClientsLayoutProps> = ({ children }) => {
	return <>{children}</>;
};
