import type { Metadata } from "next";

import { MainLayout } from "@widgets/layouts";

export const metadata: Metadata = {
	title: "Login",
	description: "Login to Admin Panel"
};

export default function RootLayout({
	children
}: Readonly<{
	children: React.ReactNode;
}>) {
	return <MainLayout>{children}</MainLayout>;
}
