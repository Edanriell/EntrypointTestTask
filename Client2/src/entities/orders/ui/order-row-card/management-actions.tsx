import { FC, ReactNode } from "react";
import { MoreHorizontal } from "lucide-react";

import { DropdownMenu, DropdownMenuContent, DropdownMenuTrigger } from "@shared/ui/dropdown-menu";
import { Button } from "@shared/ui/button";

type ManagementActionsProps = {
	children: ReactNode;
};

export const ManagementActions: FC<ManagementActionsProps> = ({ children }) => {
	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button variant="ghost" size="sm" className="flex-shrink-0 h-8 w-8 p-0">
					<MoreHorizontal className="h-4 w-4" />
					<span className="sr-only">Open menu</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">{children}</DropdownMenuContent>
		</DropdownMenu>
	);
};
