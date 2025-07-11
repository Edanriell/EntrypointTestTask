import { FC, ReactNode } from "react";

import { Toaster } from "@shared/ui/sonner";

type ToasterProvidersProps = {
	children: ReactNode;
};

export const ToasterProvider: FC<ToasterProvidersProps> = ({ children }) => {
	return (
		<>
			{children}
			<Toaster richColors={true} expand={false} position="top-center" />
		</>
	);
};
