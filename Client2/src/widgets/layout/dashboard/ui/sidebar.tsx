import { FC, ReactNode } from "react";

type DashboardSidebarProps = {
	children: ReactNode;
};

export const Sidebar: FC<DashboardSidebarProps> = ({ children }) => {
	return <>{children}</>;
};
