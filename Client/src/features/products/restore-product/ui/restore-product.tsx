"use client";

import { FC } from "react";
import { RotateCcw } from "lucide-react";

import { Button } from "@shared/ui/button";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
	AlertDialogTrigger
} from "@shared/ui/alert-dialog";
import { Spinner } from "@shared/ui/spinner";

import { useRestoreProduct } from "../api";

type RestoreProductProps = {
	productId: string;
};

export const RestoreProduct: FC<RestoreProductProps> = ({ productId }) => {
	const { mutateAsync: restoreProduct, isPending } = useRestoreProduct();

	const handleRestore = async () => {
		try {
			await restoreProduct({ productId });
		} catch (error) {
			console.error("Error restoring product:", error);
		}
	};

	return (
		<AlertDialog>
			<AlertDialogTrigger asChild>
				<Button variant="outline" size="sm">
					<RotateCcw className="h-4 w-4 mr-2" />
					Restore
				</Button>
			</AlertDialogTrigger>
			<AlertDialogContent>
				<AlertDialogHeader>
					<AlertDialogTitle>Restore Product</AlertDialogTitle>
					<AlertDialogDescription>
						Are you sure you want to restore this product? It will become available
						again and visible to customers.
					</AlertDialogDescription>
				</AlertDialogHeader>
				<AlertDialogFooter>
					<AlertDialogCancel>Cancel</AlertDialogCancel>
					<AlertDialogAction onClick={handleRestore} disabled={isPending}>
						{isPending ? (
							<>
								<Spinner />
								<span>Restoring...</span>
							</>
						) : (
							<span>Restore Product</span>
						)}
					</AlertDialogAction>
				</AlertDialogFooter>
			</AlertDialogContent>
		</AlertDialog>
	);
};
