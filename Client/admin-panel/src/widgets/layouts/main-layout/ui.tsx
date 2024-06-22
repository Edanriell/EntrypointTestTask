import { FC, ReactNode } from "react";

type MainLayoutProps = {
	children: ReactNode;
};

export const MainLayout: FC<MainLayoutProps> = ({ children }) => <main>{children}</main>;
