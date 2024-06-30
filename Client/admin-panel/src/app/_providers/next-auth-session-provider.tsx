"use client";

import { ReactNode } from "react";
import { SessionProvider } from "next-auth/react";

export const NextAuthSessionProvider = ({ children }: { children: ReactNode }) => {
	return <SessionProvider>{children}</SessionProvider>;
};
