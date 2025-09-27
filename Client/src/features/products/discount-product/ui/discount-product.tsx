"use client";

import { FC, useState } from "react";
import { Percent } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogHeader,
	DialogTitle,
	DialogTrigger
} from "@shared/ui/dialog";
import { Spinner } from "@shared/ui/spinner";

import { useDiscountProduct } from "../api";
import { DiscountProductFormData, discountProductSchema } from "../model";

type DiscountProductProps = {
	productId: string;
};

export const DiscountProduct: FC<DiscountProductProps> = ({ productId }) => {
	const [open, setOpen] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		reset,
		setError
	} = useForm<DiscountProductFormData>({
		resolver: zodResolver(discountProductSchema),
		mode: "onBlur",
		defaultValues: {
			newPrice: undefined
		}
	});

	const { mutateAsync: discountProduct, isPending } = useDiscountProduct(setError);

	const onSubmit = async (data: DiscountProductFormData) => {
		try {
			await discountProduct({
				productId,
				updatedProductPriceData: {
					newPrice: data.newPrice
				}
			});
			handleClose();
		} catch (error) {
			console.error("Error applying discount:", error);
		}
	};

	const handleClose = () => {
		setOpen(false);
		reset();
	};

	return (
		<Dialog open={open} onOpenChange={setOpen}>
			<DialogTrigger asChild>
				<Button className="mr-[20px]" variant="outline" size="sm">
					<Percent className="h-4 w-4 mr-2" />
					Discount
				</Button>
			</DialogTrigger>
			<DialogContent className="sm:max-w-md">
				<DialogHeader>
					<DialogTitle>Apply Discount</DialogTitle>
					<DialogDescription>
						Set a new discounted price for this product. If the new price is lower than
						the current price, a discount will be applied automatically.
					</DialogDescription>
				</DialogHeader>
				<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
					<div className="space-y-2 relative">
						<Label htmlFor="newPrice">New Price</Label>
						<Input
							id="newPrice"
							type="number"
							step="0.01"
							min="0"
							placeholder="Enter new price"
							{...register("newPrice", { valueAsNumber: true })}
							className={errors.newPrice ? "border-red-500" : ""}
						/>
						<AnimatePresence>
							{errors.newPrice && (
								<motion.p
									initial={{
										opacity: 0,
										x: -15,
										filter: "blur(0.24rem)"
									}}
									animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
									exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
									className="text-sm text-red-500 absolute bottom-[-1.5rem]"
								>
									{errors.newPrice.message}
								</motion.p>
							)}
						</AnimatePresence>
					</div>
					<div className="flex justify-end gap-2 pt-4">
						<Button type="button" variant="outline" onClick={handleClose}>
							Cancel
						</Button>
						<Button type="submit" disabled={isSubmitting || isPending}>
							{isSubmitting || isPending ? (
								<>
									<Spinner />
									<span>Applying...</span>
								</>
							) : (
								<span>Apply Discount</span>
							)}
						</Button>
					</div>
				</form>
			</DialogContent>
		</Dialog>
	);
};
