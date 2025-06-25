import { FC, useRef, useState } from "react";
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
import { Button } from "@shared/ui/button";

export const DeleteProduct: FC = () => {
	const [isDeleting, setIsDeleting] = useState(false);
	const [isPressed, setIsPressed] = useState(false);
	const timeoutRef = useRef<NodeJS.Timeout | null>(null);

	const handleMouseDown = () => {
		setIsPressed(true);
		timeoutRef.current = setTimeout(() => {
			setIsDeleting(true);
			// Add your delete logic here
			console.log("User deleted!");
		}, 1500); // 1.5 seconds hold time
	};

	const handleMouseUp = () => {
		setIsPressed(false);
		if (timeoutRef.current) {
			clearTimeout(timeoutRef.current);
			timeoutRef.current = null;
		}
	};

	const handleMouseLeave = () => {
		setIsPressed(false);
		if (timeoutRef.current) {
			clearTimeout(timeoutRef.current);
			timeoutRef.current = null;
		}
	};

	return (
		<AlertDialog>
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
						Are you sure, that you want to delete this product ? This action will make
						product temporarily unavailable to clients.
					</AlertDialogDescription>
				</AlertDialogHeader>
				<AlertDialogFooter>
					<AlertDialogCancel>Cancel</AlertDialogCancel>
					<Button
						variant="outline"
						className="relative overflow-hidden transition-transform duration-150 ease-out active:scale-95"
						onMouseDown={handleMouseDown}
						onMouseUp={handleMouseUp}
						onMouseLeave={handleMouseLeave}
						disabled={isDeleting}
						style={{
							transform: isPressed ? "scale(0.97)" : "scale(1)"
						}}
					>
						<span
							className={`flex items-center gap-2 ${isDeleting ? "opacity-50" : ""}`}
						>
							<Trash2 className="h-4 w-4" />
							Hold to Delete
						</span>
						<div
							className="absolute inset-0 flex items-center justify-center gap-2 rounded-md bg-red-50 text-red-600 dark:bg-red-950 dark:text-red-400"
							style={{
								clipPath: isPressed
									? "inset(0px 0px 0px 0px)"
									: "inset(0px 100% 0px 0px)",
								transition: isPressed
									? "clip-path 1.5s linear"
									: "clip-path 0.2s ease-out"
							}}
						>
							<Trash2 className="h-4 w-4" />
							Hold to Delete
						</div>
					</Button>
				</AlertDialogFooter>
			</AlertDialogContent>
		</AlertDialog>
	);
};
