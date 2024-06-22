import type { Metadata } from "next";

import { MainLayout } from "@widgets/layouts";

export const metadata: Metadata = {
	title: "Access Denied",
	description: "You don't have access to application"
};

export default function RootLayout({
	children
}: Readonly<{
	children: React.ReactNode;
}>) {
	return <MainLayout>{children}</MainLayout>;
}
