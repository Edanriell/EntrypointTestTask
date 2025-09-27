"use client";

import React, { ComponentType, FC, ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";

import { useRequireAuth } from "../hooks";
import type { AuthStrategy } from "../../model";

type WithAuthOptions = {
	fallback?: ReactNode;
	loadingComponent?: ReactNode;
	redirectTo?: string;
	requiredRoles?: string[];
	requireAllRoles?: boolean;
	requiredPermissions?: string[];
	requireAllPermissions?: boolean;
	strategy?: AuthStrategy;
};

export function withAuthGuard<P extends object>(
	WrappedComponent: ComponentType<P>,
	{
		fallback,
		loadingComponent,
		redirectTo = "/sign-in",
		requiredRoles = [],
		requireAllRoles = false,
		requiredPermissions = [],
		requireAllPermissions = false,
		strategy
	}: WithAuthOptions = {}
) {
	const ComponentWithAuth = (props: P) => {
		const { session, error } = useRequireAuth(strategy);
		const { isAuthenticated, isLoading, user } = session;
		const router = useRouter();

		useEffect(() => {
			if (!isLoading && !isAuthenticated) {
				router.push(redirectTo);
			}
		}, [isAuthenticated, isLoading, router, redirectTo]);

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

		if (!isAuthenticated) {
			return fallback || null;
		}

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

		if (requiredPermissions.length > 0 && user) {
			const userPermissions = user.permissions || [];
			const hasRequiredPermission = requireAllPermissions
				? requiredPermissions.every((perm) => userPermissions.includes(perm))
				: requiredPermissions.some((perm) => userPermissions.includes(perm));

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

		return <WrappedComponent {...props} />;
	};

	ComponentWithAuth.displayName = `WithAuthGuard(${
		WrappedComponent.displayName || WrappedComponent.name || "Component"
	})`;

	return ComponentWithAuth as FC<P>;
}
