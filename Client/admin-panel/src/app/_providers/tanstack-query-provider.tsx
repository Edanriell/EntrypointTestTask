"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactNode } from "react";

export const TanStackQueryProvider = ({ children }: { children: ReactNode }) => {
	// const [queryClient] = useState(() => new QueryClient());
	const queryClient = new QueryClient();

	return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
};
