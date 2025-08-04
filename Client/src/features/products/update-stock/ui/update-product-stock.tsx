import { FC, KeyboardEvent, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AnimatePresence, motion } from "motion/react";

import { Input } from "@shared/ui/input";
import { Spinner } from "@shared/ui/spinner";
import { getChangedFields } from "@shared/lib/utils";

import type { UpdateProductStockFormData } from "../model";
import { updateProductStockSchema } from "../model";
import { useGetProductById, useUpdateProductStock } from "../api";
import { PRODUCT_STOCK_UPDATABLE_FIELDS } from "../config";
import { stockComparator } from "../lib";

type UpdateProductStockProps = {
	productId: string;
};

export const UpdateProductStock: FC<UpdateProductStockProps> = ({ productId }) => {
	const [editingStock, setEditingStock] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		watch,
		reset,
		setError
	} = useForm<UpdateProductStockFormData>({
		resolver: zodResolver(updateProductStockSchema),
		mode: "onTouched",
		defaultValues: {
			totalStock: 0
		}
	});

	const stock = watch("totalStock");

	const { data: productData } = useGetProductById(productId);
	const { mutateAsync: updateStock, isPending } = useUpdateProductStock(setError);

	// Set initial stock when product data loads or editing starts
	useEffect(() => {
		if (productData && editingStock) {
			setValue("totalStock", productData.totalStock);
		}
	}, [productData, editingStock, setValue]);

	const handleStockSubmit = async (data?: UpdateProductStockFormData) => {
		try {
			if (data && productData) {
				const updatedProductStockData = getChangedFields(
					productData!,
					data,
					PRODUCT_STOCK_UPDATABLE_FIELDS,
					stockComparator
				);

				if (Object.keys(updatedProductStockData).length > 0) {
					await updateStock({
						productId,
						updatedProductStockData
					});
				} else {
					console.log("No stock changes detected");
				}
			}
			setEditingStock(false);
		} catch (error) {
			console.error("Error updating product stock:", error);
		}
	};

	const handleCancel = () => {
		if (productData) {
			setValue("totalStock", productData.totalStock);
		}

		reset();

		setEditingStock(false);
	};

	const handleKeyDown = (e: KeyboardEvent) => {
		if (e.key === "Enter") {
			e.preventDefault();
			handleSubmit(handleStockSubmit)();
		}

		if (e.key === "Escape") {
			e.preventDefault();
			handleCancel();
		}
	};

	if (!productData) {
		return (
			<div className="flex flex-col gap-1 min-w-0 flex-shrink-0">
				<div className="text-xs text-muted-foreground text-center">Stock</div>
				<div className="w-20 h-8 flex items-center justify-center">
					<Spinner />
				</div>
			</div>
		);
	}

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 relative">
			<div className="text-xs text-muted-foreground text-center">Stock</div>
			{editingStock ? (
				<form onSubmit={handleSubmit(handleStockSubmit)} className="relative">
					<div className="relative">
						<Input
							type="number"
							min="0"
							{...register("totalStock", { valueAsNumber: true })}
							onBlur={handleSubmit(handleStockSubmit)}
							onKeyDown={handleKeyDown}
							className={`w-20 h-8 text-sm text-center no-arrows pr-6 ${
								errors.totalStock ? "border-red-500" : ""
							}`}
							disabled={isPending}
							autoFocus
						/>
						{isPending && (
							<div className="absolute right-2 top-1/2 transform -translate-y-1/2">
								<Spinner className="w-3 h-3" />
							</div>
						)}
					</div>
					<AnimatePresence>
						{errors.totalStock && (
							<motion.p
								initial={{
									opacity: 0,
									y: -10,
									filter: "blur(0.24rem)"
								}}
								animate={{ opacity: 1, y: 0, filter: "blur(0)" }}
								exit={{ opacity: 0, y: -10, filter: "blur(0.24rem)" }}
								className="text-xs text-red-500 absolute top-full left-1/2 transform -translate-x-1/2 mt-1 whitespace-nowrap z-10 bg-background px-2 py-1 rounded shadow-md border"
							>
								{errors.totalStock.message}
							</motion.p>
						)}
					</AnimatePresence>
				</form>
			) : (
				<div
					className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium min-h-8"
					onClick={() => setEditingStock(true)}
				>
					<span>{productData.totalStock}</span>
				</div>
			)}
		</div>
	);
};
