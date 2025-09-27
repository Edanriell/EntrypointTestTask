"use client";

import { FC } from "react";
import { SquarePen } from "lucide-react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { AnimatePresence, motion } from "motion/react";

import { Currency } from "@entities/products";

import { Button } from "@shared/ui/button";
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
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Textarea } from "@shared/ui/textarea";
import { Spinner } from "@shared/ui/spinner";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";

import { useCreateProduct } from "../api";
import { CreateProductFormData, createProductSchema } from "../model";

export const CreateProduct: FC = () => {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		reset,
		setError,
		watch,
		setValue
	} = useForm<CreateProductFormData>({
		resolver: zodResolver(createProductSchema),
		mode: "onTouched",
		defaultValues: {
			name: "",
			description: "",
			currency: undefined,
			price: undefined,
			totalStock: undefined
		}
	});

	const selectedCurrency = watch("currency");

	const { mutateAsync: createProduct, isPending } = useCreateProduct(reset, setError);

	const onSubmit = async (data: CreateProductFormData) => {
		// Convert string inputs to numbers for price and stock
		const productData = {
			...data,
			price: parseFloat(data.price.toString()),
			stock: parseInt(data.totalStock.toString())
		};

		await createProduct(productData);
	};

	return (
		<Sheet>
			<SheetTrigger asChild>
				<Button className="flex w-fit self-end ml-[20px]">
					<SquarePen />
					Create new
				</Button>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Create product</SheetTitle>
					<SheetDescription>
						Fill in the details below to create a new product. Click save when you're
						done.
					</SheetDescription>
				</SheetHeader>
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid flex-1 auto-rows-min gap-6 px-4 mb-10">
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="name">Name</Label>
							<Input
								id="name"
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
										className="text-sm text-red-500 absolute bottom-[-1.3rem]"
									>
										{errors.name.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="description">Description</Label>
							<Textarea
								id="description"
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
										className="text-sm text-red-500 absolute bottom-[-1.3rem]"
									>
										{errors.description.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid gap-2 col-span-full relative">
							<Label htmlFor="currency">Currency</Label>
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
									id="currency"
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
										exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
										className="text-sm text-red-500 absolute bottom-[-1.3rem]"
									>
										{errors.currency.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>
						<div className="grid grid-cols-2 gap-3">
							<div className="grid gap-2 relative">
								<Label htmlFor="price">Price</Label>
								<Input
									id="price"
									type="number"
									step="0.01"
									min="0"
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
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.3rem]"
										>
											{errors.price.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
							<div className="grid gap-2 relative">
								<Label htmlFor="stock">Stock</Label>
								<Input
									id="stock"
									type="number"
									min="0"
									placeholder="Enter product stock"
									{...register("totalStock", { valueAsNumber: true })}
									className={errors.totalStock ? "border-red-500" : ""}
								/>
								<AnimatePresence>
									{errors.totalStock && (
										<motion.p
											initial={{
												opacity: 0,
												x: -15,
												filter: "blur(0.24rem)"
											}}
											animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500 absolute bottom-[-1.3rem]"
										>
											{errors.totalStock.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
						</div>
					</div>
					<SheetFooter>
						<Button type="submit" disabled={isSubmitting || isPending}>
							{isSubmitting || isPending ? (
								<>
									<Spinner />
									<span>Creating...</span>
								</>
							) : (
								<span>Create New</span>
							)}
						</Button>
						<SheetClose asChild>
							<Button variant="outline">Cancel</Button>
						</SheetClose>
					</SheetFooter>
				</form>
			</SheetContent>
		</Sheet>
	);
};
