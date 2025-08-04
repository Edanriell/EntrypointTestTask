import { FC, useEffect } from "react";
import { Edit } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Currency } from "@entities/products";

import { DropdownMenuItem } from "@shared/ui/dropdown-menu";
import {
	Sheet,
	SheetClose,
	SheetContent,
	SheetDescription,
	SheetFooter,
	SheetHeader,
	SheetTitle,
	SheetTrigger
} from "@shared/ui/sheet";
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Textarea } from "@shared/ui/textarea";
import { Spinner } from "@shared/ui/spinner";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { getChangedFields } from "@shared/lib/utils";

import { useGetProductById, useUpdateProduct } from "../api";
import { CURRENCY_MAPPING, EditProductFormData, editProductSchema } from "../model";
import { PRODUCT_UPDATABLE_FIELDS } from "../config";
import { currencyComparator } from "../lib";

type EditProductProps = {
	productId: string;
};

export const EditProduct: FC<EditProductProps> = ({ productId }) => {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		watch,
		setError
	} = useForm<EditProductFormData>({
		resolver: zodResolver(editProductSchema),
		mode: "onTouched",
		defaultValues: {
			name: "",
			description: "",
			price: 0,
			currency: undefined,
			stock: 0
		}
	});

	const selectedCurrency = watch("currency");

	// Fetch product data for prefilling the form
	const { data: productData, isLoading: isLoadingProduct } = useGetProductById(productId);

	// Update product mutation
	const { mutateAsync: updateProduct, isPending } = useUpdateProduct(setError);

	// Prefill form when product data is loaded
	useEffect(() => {
		if (productData) {
			setValue("name", productData.name);
			setValue("description", productData.description);
			setValue("price", productData.price);
			setValue("currency", CURRENCY_MAPPING[productData.currency]);
			setValue("stock", productData.totalStock);
		}
	}, [productData, setValue]);

	const onSubmit = async (data: EditProductFormData) => {
		try {
			const updatedProductData = getChangedFields(
				productData!,
				data,
				PRODUCT_UPDATABLE_FIELDS,
				currencyComparator
			);

			if (Object.keys(updatedProductData).length > 0) {
				await updateProduct({ productId, updatedProductData });
			} else {
				console.log("No changes detected");
			}
		} catch (error) {
			console.error("Error updating product:", error);
		}
	};

	return (
		<Sheet>
			<SheetTrigger asChild>
				<DropdownMenuItem onSelect={(e) => e.preventDefault()}>
					<Edit className="mr-2 h-4 w-4" />
					Edit
				</DropdownMenuItem>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Edit product</SheetTitle>
					<SheetDescription>
						Make changes to the product details below. Click save when you're done.
					</SheetDescription>
				</SheetHeader>
				{isLoadingProduct ? (
					<div className="flex justify-center items-center h-64">
						<Spinner />
					</div>
				) : (
					<form onSubmit={handleSubmit(onSubmit)}>
						<div className="grid flex-1 auto-rows-min gap-8 px-4 mb-10">
							<div className="grid gap-2 col-span-full relative">
								<Label htmlFor="edit-name">Name</Label>
								<Input
									id="edit-name"
									type="text"
									placeholder="Enter product name"
									{...register("name")}
									className={errors.name ? "border-red-500" : ""}
								/>
								<AnimatePresence>
									{errors.name && (
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
											{errors.name.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-2 col-span-full relative">
								<Label htmlFor="edit-description">Description</Label>
								<Textarea
									id="edit-description"
									placeholder="Product description"
									className={`resize-none ${errors.description ? "border-red-500" : ""}`}
									{...register("description")}
								/>
								<AnimatePresence>
									{errors.description && (
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
											{errors.description.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid grid-cols-2 gap-3">
								<div className="grid gap-2 relative">
									<Label htmlFor="edit-price">Price</Label>
									<Input
										id="edit-price"
										type="number"
										step="0.01"
										placeholder="Enter product price"
										{...register("price", { valueAsNumber: true })}
										className={errors.price ? "border-red-500" : ""}
									/>
									<AnimatePresence>
										{errors.price && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.price.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
								<div className="grid gap-2 relative">
									<Label htmlFor="edit-currency">Currency</Label>
									<Select
										value={
											selectedCurrency !== undefined
												? selectedCurrency.toString()
												: ""
										}
										onValueChange={(value) =>
											setValue("currency", value as Currency, {
												shouldValidate: true
											})
										}
									>
										<SelectTrigger
											id="edit-currency"
											className={`w-full ${errors.currency ? "border-red-500" : ""}`}
										>
											<SelectValue placeholder="Select currency" />
										</SelectTrigger>
										<SelectContent>
											<SelectItem value="Eur">EUR</SelectItem>
											<SelectItem value="Usd">USD</SelectItem>
										</SelectContent>
									</Select>
									<AnimatePresence>
										{errors.currency && (
											<motion.p
												initial={{
													opacity: 0,
													x: -15,
													filter: "blur(0.24rem)"
												}}
												animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
												exit={{
													opacity: 0,
													x: 15,
													filter: "blur(0.24rem)"
												}}
												className="text-sm text-red-500 absolute bottom-[-1.5rem]"
											>
												{errors.currency.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>
							<div className="grid gap-2 col-span-full relative">
								<Label htmlFor="edit-stock">Stock</Label>
								<Input
									id="edit-stock"
									type="number"
									placeholder="Enter product stock"
									{...register("stock", { valueAsNumber: true })}
									className={errors.stock ? "border-red-500" : ""}
								/>
								<AnimatePresence>
									{errors.stock && (
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
											{errors.stock.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>

							{/* Display reserved stock as read-only info */}
							{productData && productData.reserved > 0 && (
								<div className="grid gap-2 col-span-full">
									<Label>Reserved Stock (Read Only)</Label>
									<div className="px-3 py-2 bg-muted rounded-md text-sm text-muted-foreground">
										{productData.reserved} units are currently reserved
									</div>
								</div>
							)}
						</div>
						<SheetFooter>
							<Button type="submit" disabled={isSubmitting || isPending}>
								{isSubmitting || isPending ? (
									<>
										<Spinner />
										<span>Saving...</span>
									</>
								) : (
									<span>Save Changes</span>
								)}
							</Button>
							<SheetClose asChild>
								<Button variant="outline">Cancel</Button>
							</SheetClose>
						</SheetFooter>
					</form>
				)}
			</SheetContent>
		</Sheet>
	);
};
