import { FC, ReactNode } from "react";

type QuickActionsProps = {
	children: ReactNode;
};

export const QuickActions: FC<QuickActionsProps> = ({ children }) => {
	return <>{children}</>;
};
