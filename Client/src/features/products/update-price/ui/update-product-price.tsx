import { FC, KeyboardEvent, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AnimatePresence, motion } from "motion/react";

import { Input } from "@shared/ui/input";
import { Spinner } from "@shared/ui/spinner";
import { formatCurrency, getChangedFields } from "@shared/lib/utils";

import { UpdateProductPriceFormData, updateProductPriceSchema } from "../model";
import { useGetProductById, useUpdateProductPrice } from "../api";
import { PRODUCT_PRICE_UPDATABLE_FIELDS } from "../config";
import { priceComparator } from "../lib";

type UpdateProductPriceProps = {
	productId: string;
};

export const UpdateProductPrice: FC<UpdateProductPriceProps> = ({ productId }) => {
	const [editingPrice, setEditingPrice] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		watch,
		reset,
		setError
	} = useForm<UpdateProductPriceFormData>({
		resolver: zodResolver(updateProductPriceSchema),
		mode: "onTouched",
		defaultValues: {
			price: 0
		}
	});

	const price = watch("price");

	const { data: productData } = useGetProductById(productId);
	const { mutateAsync: updatePrice, isPending } = useUpdateProductPrice(setError);

	// Set initial price when product data loads or editing starts
	useEffect(() => {
		if (productData && editingPrice) {
			setValue("price", productData.price);
		}
	}, [productData, editingPrice, setValue]);

	const handlePriceSubmit = async (data?: UpdateProductPriceFormData) => {
		try {
			if (data && productData) {
				const updatedProductPriceData = getChangedFields(
					productData!,
					data,
					PRODUCT_PRICE_UPDATABLE_FIELDS,
					priceComparator
				);

				console.log(productData);
				console.log(data);

				if (Object.keys(updatedProductPriceData).length > 0) {
					await updatePrice({
						productId,
						updatedProductPriceData
					});
				} else {
					console.log("No price changes detected");
				}
			}
			setEditingPrice(false);
		} catch (error) {
			console.error("Error updating product price:", error);
		}
	};

	const handleCancel = () => {
		if (productData) {
			setValue("price", productData.price);
		}

		reset();

		setEditingPrice(false);
	};

	const handleKeyDown = (e: KeyboardEvent) => {
		if (e.key === "Enter") {
			e.preventDefault();
			handleSubmit(handlePriceSubmit)();
		}

		if (e.key === "Escape") {
			e.preventDefault();
			handleCancel();
		}
	};

	if (!productData) {
		return (
			<div className="flex flex-col gap-1 min-w-0 flex-shrink-0">
				<div className="text-xs text-muted-foreground text-center">Price</div>
				<div className="w-20 h-8 flex items-center justify-center">
					<Spinner />
				</div>
			</div>
		);
	}

	return (
		<div className="flex flex-col gap-1 min-w-0 flex-shrink-0 relative">
			<div className="text-xs text-muted-foreground text-center">Price</div>
			{editingPrice ? (
				<form onSubmit={handleSubmit(handlePriceSubmit)} className="relative">
					<div className="relative">
						<Input
							type="number"
							min="0"
							step="0.01"
							{...register("price", { valueAsNumber: true })}
							onBlur={handleSubmit(handlePriceSubmit)}
							onKeyDown={handleKeyDown}
							className={`w-20 h-8 text-sm text-center no-arrows pr-6 ${
								errors.price ? "border-red-500" : ""
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
						{errors.price && (
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
								{errors.price.message}
							</motion.p>
						)}
					</AnimatePresence>
				</form>
			) : (
				<div
					className="flex items-center gap-1 cursor-pointer hover:bg-muted/50 px-2 py-1 rounded text-sm font-medium min-h-8"
					onClick={() => setEditingPrice(true)}
				>
					<span>{formatCurrency(productData.price)}</span>
				</div>
			)}
		</div>
	);
};
