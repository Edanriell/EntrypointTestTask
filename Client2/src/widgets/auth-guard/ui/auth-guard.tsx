"use client";

import { ReactNode } from "react";
import { useRequireAuth } from "@shared/lib/auth";

interface AuthGuardProps {
	children: ReactNode;
	fallback?: ReactNode;
}

// TODO
// Needs improvements
export const AuthGuard = ({ children, fallback }: AuthGuardProps) => {
	const { isAuthenticated, isLoading } = useRequireAuth();

	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-screen">
				<div>Loading...</div>
			</div>
		);
	}

	if (!isAuthenticated) {
		return fallback || null;
	}

	return <>{children}</>;
};
