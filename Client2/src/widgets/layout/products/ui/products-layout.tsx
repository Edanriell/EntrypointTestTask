import { FC, ReactNode } from "react";
import type { Metadata } from "next";

import { generateStaticMetadata } from "@shared/lib/functions";

export const metadata: Metadata = generateStaticMetadata({
	title: "Products - Business Management",
	description:
		"Organize and manage your product catalog. Add, edit-user, and maintain product information, inventory levels, and pricing in your centralized product database.",
	ogTitle: "Product Management Dashboard",
	ogDescription:
		"Complete product catalog management solution. Maintain inventory, update product details, and organize your entire product portfolio efficiently."
});

type ProductsLayoutProps = {
	children: ReactNode;
};

export const ProductsLayout: FC<ProductsLayoutProps> = ({ children }) => {
	return <>{children}</>;
};
