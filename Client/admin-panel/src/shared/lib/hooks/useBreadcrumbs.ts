import { usePathname } from "next/navigation";

export const useBreadcrumbs = () => {
	const fullPath = usePathname();

	const pathParts = fullPath.split("/").filter((part) => part);

	return pathParts.map((part, index) => {
		const href = "/" + pathParts.slice(0, index + 1).join("/");
		const name = part.charAt(0).toUpperCase() + part.slice(1);
		return { href, name };
	});
};
