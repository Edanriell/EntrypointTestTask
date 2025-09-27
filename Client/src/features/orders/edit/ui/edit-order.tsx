import { FC, useEffect } from "react";
import { Edit } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

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
import { getChangedFields } from "@shared/lib/utils";

import { useGetOrderById, useUpdateOrder } from "../api";
import { EditOrderFormData, editOrderSchema } from "../model";
import { ORDER_UPDATABLE_FIELDS } from "../config";
import { orderComparator, parseAddressString, transformOrderDataToFormFormat } from "../lib";

type EditOrderProps = {
	orderId: string;
};

export const EditOrder: FC<EditOrderProps> = ({ orderId }) => {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		setValue,
		setError
	} = useForm<EditOrderFormData>({
		resolver: zodResolver(editOrderSchema),
		mode: "onTouched",
		defaultValues: {
			street: "",
			city: "",
			zipCode: "",
			country: "",
			info: ""
		}
	});

	// Fetch order data for prefilling the form
	const { data: orderData, isLoading: isLoadingOrder } = useGetOrderById(orderId) as any;

	// Update order mutation
	const { mutateAsync: updateOrder, isPending } = useUpdateOrder(setError);

	// Prefill form when order data is loaded
	useEffect(() => {
		if (orderData) {
			// Parse the address string if it exists
			if (orderData.shippingAddress && typeof orderData.shippingAddress === "string") {
				// We are getting shippingAddress as a whole string. To be able to set initail value, we need to
				// split string into four components which are expected by inputs.
				const parsedAddress = parseAddressString(orderData.shippingAddress);

				setValue("street", parsedAddress.street!);
				setValue("city", parsedAddress.city!);
				setValue("zipCode", parsedAddress.zipCode!);
				setValue("country", parsedAddress.country!);
			}
			setValue("info", orderData.info || "");
		}
	}, [orderData, setValue]);

	const onSubmit = async (data: EditOrderFormData) => {
		// Shipping address is holded as whole string shippingAddress. To be able
		// to compare old data with new one, we need to make them equal by splitting shippingAddress
		// into four separate components (fields)
		const normalizedOldOrderData = transformOrderDataToFormFormat(orderData!);

		try {
			const updatedOrderData = getChangedFields(
				normalizedOldOrderData,
				data,
				ORDER_UPDATABLE_FIELDS,
				orderComparator
			);

			if (Object.keys(updatedOrderData).length > 0) {
				await updateOrder({ orderId, updatedOrderData });
			} else {
				console.log("No changes detected");
			}
		} catch (error) {
			console.error("Error updating order:", error);
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
					<SheetTitle>Edit order</SheetTitle>
					<SheetDescription>
						Make changes to the order shipping address and information below. Click save
						when you're done.
					</SheetDescription>
				</SheetHeader>
				{isLoadingOrder ? (
					<div className="flex justify-center items-center h-64">
						<Spinner />
					</div>
				) : (
					<form onSubmit={handleSubmit(onSubmit)}>
						<div className="grid flex-1 auto-rows-min gap-8 px-4 mb-10">
							{/* Order Information */}
							<div className="space-y-4">
								<h3 className="text-lg font-medium">Order Information</h3>
								<div className="grid gap-2 relative">
									<Label htmlFor="edit-info">Order Notes</Label>
									<Textarea
										id="edit-info"
										placeholder="Additional order information or notes"
										className={`resize-none ${errors.info ? "border-red-500" : ""}`}
										{...register("info")}
									/>
									<AnimatePresence>
										{errors.info && (
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
												{errors.info.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>

							{/* Shipping Address Section */}
							<div className="space-y-4">
								<h3 className="text-lg font-medium">Shipping Address</h3>
								<div className="grid grid-cols-2 gap-3">
									<div className="grid gap-2 relative">
										<Label htmlFor="edit-country">Country *</Label>
										<Input
											id="edit-country"
											placeholder="Enter country"
											{...register("country")}
											className={errors.country ? "border-red-500" : ""}
										/>
										<AnimatePresence>
											{errors.country && (
												<motion.p
													initial={{
														opacity: 0,
														x: -15,
														filter: "blur(0.24rem)"
													}}
													animate={{
														opacity: 1,
														x: 0,
														filter: "blur(0)"
													}}
													exit={{
														opacity: 0,
														x: 15,
														filter: "blur(0.24rem)"
													}}
													className="text-sm text-red-500 absolute bottom-[-1.5rem]"
												>
													{errors.country.message}
												</motion.p>
											)}
										</AnimatePresence>
									</div>
									<div className="grid gap-2 relative">
										<Label htmlFor="edit-city">City *</Label>
										<Input
											id="edit-city"
											placeholder="Enter city"
											{...register("city")}
											className={errors.city ? "border-red-500" : ""}
										/>
										<AnimatePresence>
											{errors.city && (
												<motion.p
													initial={{
														opacity: 0,
														x: -15,
														filter: "blur(0.24rem)"
													}}
													animate={{
														opacity: 1,
														x: 0,
														filter: "blur(0)"
													}}
													exit={{
														opacity: 0,
														x: 15,
														filter: "blur(0.24rem)"
													}}
													className="text-sm text-red-500 absolute bottom-[-1.5rem]"
												>
													{errors.city.message}
												</motion.p>
											)}
										</AnimatePresence>
									</div>
								</div>
								<div className="grid grid-cols-2 gap-3">
									<div className="grid gap-2 relative">
										<Label htmlFor="edit-zipCode">Zip Code *</Label>
										<Input
											id="edit-zipCode"
											placeholder="Enter zip code"
											{...register("zipCode")}
											className={errors.zipCode ? "border-red-500" : ""}
										/>
										<AnimatePresence>
											{errors.zipCode && (
												<motion.p
													initial={{
														opacity: 0,
														x: -15,
														filter: "blur(0.24rem)"
													}}
													animate={{
														opacity: 1,
														x: 0,
														filter: "blur(0)"
													}}
													exit={{
														opacity: 0,
														x: 15,
														filter: "blur(0.24rem)"
													}}
													className="text-sm text-red-500 absolute bottom-[-1.5rem]"
												>
													{errors.zipCode.message}
												</motion.p>
											)}
										</AnimatePresence>
									</div>
									<div className="grid gap-2 relative">
										<Label htmlFor="edit-street">Street *</Label>
										<Input
											id="edit-street"
											placeholder="Enter street address"
											{...register("street")}
											className={errors.street ? "border-red-500" : ""}
										/>
										<AnimatePresence>
											{errors.street && (
												<motion.p
													initial={{
														opacity: 0,
														x: -15,
														filter: "blur(0.24rem)"
													}}
													animate={{
														opacity: 1,
														x: 0,
														filter: "blur(0)"
													}}
													exit={{
														opacity: 0,
														x: 15,
														filter: "blur(0.24rem)"
													}}
													className="text-sm text-red-500 absolute bottom-[-1.5rem]"
												>
													{errors.street.message}
												</motion.p>
											)}
										</AnimatePresence>
									</div>
								</div>
							</div>

							{/* Display read-only order information */}
							{orderData && (
								<div className="space-y-4">
									<h3 className="text-lg font-medium">
										Order Details (Read Only)
									</h3>
									<div className="bg-muted/50 p-4 rounded-lg space-y-2">
										<div className="text-sm">
											<strong>Order ID:</strong> {orderData.id}
										</div>
										<div className="text-sm">
											<strong>Status:</strong> {orderData.status}
										</div>
										<div className="text-sm">
											<strong>Total:</strong> $
											{orderData.totalAmount?.toFixed(2)} {orderData.currency}
										</div>
										<div className="text-sm">
											<strong>Created:</strong>{" "}
											{new Date(orderData.createdAt).toLocaleDateString()}
										</div>
										{orderData.customer && (
											<div className="text-sm">
												<strong>Customer:</strong>{" "}
												{orderData.customer.firstName}{" "}
												{orderData.customer.lastName}
											</div>
										)}
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
