import { ReactNode } from "react";

import { AuthProvider, QueryProvider, ToasterProvider } from "@app/_providers";

import { DefaultLayout, metadata } from "@widgets/layout/default";

export { metadata };

export default async ({ children }: { children: ReactNode }) => {
	return (
		<AuthProvider>
			<QueryProvider>
				<ToasterProvider>
					<DefaultLayout>{children}</DefaultLayout>
				</ToasterProvider>
			</QueryProvider>
		</AuthProvider>
	);
};
