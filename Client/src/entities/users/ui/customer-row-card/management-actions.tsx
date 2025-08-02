import { FC, ReactNode, useState } from "react";
import { MoreHorizontal } from "lucide-react";

import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuGroup,
	DropdownMenuTrigger
} from "@shared/ui/dropdown-menu";
import { Button } from "@shared/ui/button";

type ManagementActionsProps = {
	children: ReactNode;
};

export const ManagementActions: FC<ManagementActionsProps> = ({ children }) => {
	const [open, setOpen] = useState(false);

	if (!children) return null;

	return (
		<DropdownMenu open={open} onOpenChange={setOpen}>
			<DropdownMenuTrigger asChild>
				<Button variant="ghost" size="sm">
					<MoreHorizontal />
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end" className="w-[200px]">
				<DropdownMenuGroup>{children}</DropdownMenuGroup>
			</DropdownMenuContent>
		</DropdownMenu>
	);
};
