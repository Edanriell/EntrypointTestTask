"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";

import { useRequireAuth } from "@features/authentication/general/lib/hooks";
import type { AuthStrategy } from "@features/authentication/general/model";

type AuthGuardProps = {
	children: ReactNode;
	fallback?: ReactNode;
	loadingComponent?: ReactNode;
	redirectTo?: string;
	requiredRoles?: string[];
	requireAllRoles?: boolean;
	requiredPermissions?: string[];
	requireAllPermissions?: boolean;
	strategy?: AuthStrategy;
};

export const AuthGuard = ({
	children,
	fallback,
	loadingComponent,
	redirectTo = "/sign-in",
	requiredRoles = [],
	requireAllRoles = false,
	requiredPermissions = [],
	requireAllPermissions = false,
	strategy
}: AuthGuardProps) => {
	const { session, error } = useRequireAuth(strategy);
	const { isAuthenticated, isLoading, user } = session;
	const router = useRouter();

	useEffect(() => {
		if (!isLoading && !isAuthenticated) {
			router.push(redirectTo);
		}
	}, [isAuthenticated, isLoading, router, redirectTo]);

	// Show loading state
	if (isLoading) {
		return (
			loadingComponent || (
				<div className="flex items-center justify-center min-h-screen">
					<div className="flex flex-col items-center space-y-2">
						<div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
						<p className="text-sm text-muted-foreground">Loading...</p>
					</div>
				</div>
			)
		);
	}

	// Show error state
	if (error) {
		return (
			<div className="flex items-center justify-center min-h-screen">
				<div className="text-center">
					<p className="text-destructive">Authentication Error</p>
					<p className="text-sm text-muted-foreground">{error}</p>
				</div>
			</div>
		);
	}

	// Check authentication
	if (!isAuthenticated) {
		return fallback || null;
	}

	// Check role-based access
	if (requiredRoles.length > 0 && user) {
		const userRoles = user.roles || [];
		const hasRequiredRole = requireAllRoles
			? requiredRoles.every((role) => userRoles.includes(role))
			: requiredRoles.some((role) => userRoles.includes(role));

		if (!hasRequiredRole) {
			return (
				<div className="flex items-center justify-center min-h-screen">
					<div className="text-center">
						<p className="text-destructive">Access Denied</p>
						<p className="text-sm text-muted-foreground">
							You don't have the required role to access this page
						</p>
					</div>
				</div>
			);
		}
	}

	// Check permission-based access
	if (requiredPermissions.length > 0 && user) {
		const userPermissions = user.permissions || [];
		const hasRequiredPermission = requireAllPermissions
			? requiredPermissions.every((permission) => userPermissions.includes(permission))
			: requiredPermissions.some((permission) => userPermissions.includes(permission));

		if (!hasRequiredPermission) {
			return (
				<div className="flex items-center justify-center min-h-screen">
					<div className="text-center">
						<p className="text-destructive">Access Denied</p>
						<p className="text-sm text-muted-foreground">
							You don't have the required permissions to access this page
						</p>
						<p className="text-xs text-muted-foreground mt-1">
							Required: {requiredPermissions.join(", ")}
						</p>
					</div>
				</div>
			);
		}
	}

	return <>{children}</>;
};
