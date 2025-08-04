import { FC, useState } from "react";
import { Trash2 } from "lucide-react";

import { DropdownMenuItem } from "@shared/ui/dropdown-menu";
import {
	AlertDialog,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
	AlertDialogTrigger
} from "@shared/ui/alert-dialog";
import { HoldToPressButton } from "@shared/ui/hold-to-press-button";
import { Spinner } from "@shared/ui/spinner";

import { useDeleteUser } from "../api";

type DeleteUserProps = {
	userId: string;
	userFullName?: string;
};

export const DeleteUser: FC<DeleteUserProps> = ({ userId, userFullName }) => {
	const [isDialogOpen, setIsDialogOpen] = useState<boolean>(false);

	const { mutateAsync: deleteUser, isPending } = useDeleteUser(setIsDialogOpen);

	return (
		<AlertDialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
			<AlertDialogTrigger asChild>
				<DropdownMenuItem
					className="text-red-600 dark:text-red-400"
					onSelect={(e) => e.preventDefault()}
				>
					<Trash2 className="mr-2 h-4 w-4" />
					Delete
				</DropdownMenuItem>
			</AlertDialogTrigger>
			<AlertDialogContent>
				<AlertDialogHeader>
					<AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
					<AlertDialogDescription>
						This action cannot be undone. This will permanently delete{" "}
						{userFullName ? `${userFullName}'s account` : "the user account"} and remove
						all associated data from our servers.
					</AlertDialogDescription>
				</AlertDialogHeader>
				<AlertDialogFooter>
					<AlertDialogCancel disabled={isPending}>Cancel</AlertDialogCancel>
					<HoldToPressButton
						holdDuration={1500}
						onPressAction={() => deleteUser({ userId })}
						disabled={isPending}
					>
						<span
							className={`flex items-center gap-2 ${isPending ? "opacity-50" : ""}`}
						>
							{isPending ? (
								<>
									<Spinner className="h-4 w-4" />
									Deleting...
								</>
							) : (
								<>
									<Trash2 className="h-4 w-4" />
									Hold to Delete
								</>
							)}
						</span>
					</HoldToPressButton>
				</AlertDialogFooter>
			</AlertDialogContent>
		</AlertDialog>
	);
};
