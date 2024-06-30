import { FC, ReactNode } from "react";

import { NextAuthSessionProvider, TanStackQueryProvider } from "@app/_providers";
import { Toaster } from "@/components/ui/sonner";

import "./_styles/styles.scss";

type RootLayoutProps = {
	children: ReactNode;
};

const RootLayout: FC<RootLayoutProps> = ({ children }) => {
	return (
		<html lang="en">
			<TanStackQueryProvider>
				<NextAuthSessionProvider>
					<body>{children}</body>
				</NextAuthSessionProvider>
			</TanStackQueryProvider>
			<Toaster />
		</html>
	);
};

export default RootLayout;
