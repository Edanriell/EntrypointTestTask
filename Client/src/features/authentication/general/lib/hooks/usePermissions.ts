import { useAuthUser } from "@features/authentication/general/lib/hooks";
import type { AuthStrategy } from "@features/authentication/general/model";

type UsePermissionsOptions = {
	strategy?: AuthStrategy;
};

export const usePermissions = (options?: UsePermissionsOptions) => {
	const { user } = useAuthUser(options?.strategy);

	const hasPermission = (permission: string): boolean => {
		return user?.permissions?.includes(permission) ?? false;
	};

	const hasAllPermissions = (permissions: string[]): boolean => {
		return permissions.every((permission) => hasPermission(permission));
	};

	const hasAnyPermission = (permissions: string[]): boolean => {
		return permissions.some((permission) => hasPermission(permission));
	};

	const hasRole = (role: string): boolean => {
		return user?.roles?.includes(role) ?? false;
	};

	const hasAllRoles = (roles: string[]): boolean => {
		return roles.every((role) => hasRole(role));
	};

	const hasAnyRole = (roles: string[]): boolean => {
		return roles.some((role) => hasRole(role));
	};

	return {
		hasPermission,
		hasAllPermissions,
		hasAnyPermission,
		hasRole,
		hasAllRoles,
		hasAnyRole,
		permissions: user?.permissions || [],
		roles: user?.roles || []
	};
};
