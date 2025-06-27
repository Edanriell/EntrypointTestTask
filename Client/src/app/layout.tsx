import { ReactNode } from "react";

import { AuthProvider, QueryProvider } from "@app/_providers";

import { DefaultLayout, metadata } from "@widgets/layout/default";

export { metadata };

export default ({ children }: { children: ReactNode }) => {
	return (
		<AuthProvider>
			<QueryProvider>
				<DefaultLayout>{children}</DefaultLayout>
			</QueryProvider>
		</AuthProvider>
	);
};
