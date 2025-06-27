import { useRequireAuth } from "@shared/lib/auth";

export const usePermissions = () => {
	const { user } = useRequireAuth();

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
