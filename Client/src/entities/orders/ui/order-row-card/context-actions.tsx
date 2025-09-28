import { FC, ReactNode } from "react";

type ContextActionsProps = {
	children: ReactNode;
};

export const ContextActions: FC<ContextActionsProps> = ({ children }) => {
	return <div className="flex items-center gap-2 flex-wrap">{children}</div>;
};
