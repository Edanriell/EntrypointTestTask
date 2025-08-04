import { FC, useState } from "react";
import { CreditCard } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Spinner } from "@shared/ui/spinner";

import { Currency, PaymentMethod } from "@entities/payments";

import { useAddPayment } from "../api";
import { AddPaymentFormData, addPaymentSchema } from "../model";
import { OrderStatus } from "@entities/orders";

type AddPaymentProps = {
	orderId: string;
	orderStatus: string;
	orderNumber?: string;
	outstandingAmount?: number;
	orderCurrency?: string;
};

// TODO222
// USE Z ENUM, HAVE HERE STUPID CONVERSION

// Helper function to map API string currency to enum values
const mapCurrencyToEnum = (currency: string): Currency => {
	switch (currency?.toUpperCase()) {
		case "USD":
			return "Usd" as Currency;
		case "EUR":
			return "Eur" as Currency;
		default:
			return "Usd" as Currency; // Default fallback
	}
};

// Helper function to map display currency to what we expect from API
const mapCurrencyToDisplay = (currency: string): string => {
	switch (currency?.toUpperCase()) {
		case "USD":
			return "USD";
		case "EUR":
			return "EUR";
		default:
			return currency || "";
	}
};

export const AddPayment: FC<AddPaymentProps> = ({
	orderId,
	orderStatus,
	orderNumber,
	outstandingAmount,
	orderCurrency
}) => {
	const [isOpen, setIsOpen] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		setValue,
		watch,
		reset,
		setError
	} = useForm<AddPaymentFormData>({
		resolver: zodResolver(addPaymentSchema),
		mode: "onTouched",
		defaultValues: {
			amount: outstandingAmount || 0,
			currency: mapCurrencyToDisplay(orderCurrency || ""),
			paymentMethod: ""
		}
	});

	const selectedCurrency = watch("currency");
	const selectedPaymentMethod = watch("paymentMethod");

	const { mutateAsync: addPayment, isPending } = useAddPayment(reset, setError);

	const onSubmit = async (data: AddPaymentFormData) => {
		try {
			await addPayment({
				orderId,
				amount: data.amount,
				currency: mapCurrencyToEnum(data.currency),
				paymentMethod: data.paymentMethod as PaymentMethod
			});
			setIsOpen(false); // Close popover on successful submission
		} catch (error) {
			console.error("Error adding payment:", error);
		}
	};

	const handleOpenChange = (open: boolean) => {
		setIsOpen(open);
		if (!open) {
			reset(); // Reset form when popover closes
		}
	};

	const handleCancel = () => {
		setIsOpen(false);
		reset();
	};

	if (orderStatus !== OrderStatus.Pending) {
		return null;
	}

	return (
		<Popover open={isOpen} onOpenChange={handleOpenChange}>
			<PopoverTrigger asChild>
				<Button variant="outline" size="sm" className="text-green-600 dark:text-green-400">
					<CreditCard className="mr-2 h-4 w-4" />
					Add Payment
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-96">
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid gap-4">
						<div className="space-y-2">
							<h4 className="font-medium leading-none">Add Payment</h4>
							<p className="text-sm text-muted-foreground">
								Add a payment for{" "}
								{orderNumber ? `order "${orderNumber}"` : "this order"}
								{outstandingAmount && (
									<span className="block mt-1 font-medium">
										Outstanding: {outstandingAmount} {orderCurrency}
									</span>
								)}
							</p>
						</div>

						{/* Amount Field */}
						<div className="grid gap-2 relative">
							<Label htmlFor="amount">Amount *</Label>
							<Input
								id="amount"
								type="number"
								step="0.01"
								placeholder="Enter payment amount"
								className={`${errors.amount ? "border-red-500" : ""}`}
								disabled={isPending}
								{...register("amount", { valueAsNumber: true })}
							/>
							<div className="h-5">
								<AnimatePresence>
									{errors.amount && (
										<motion.p
											initial={{
												opacity: 0,
												x: -15,
												filter: "blur(0.24rem)"
											}}
											animate={{ opacity: 1, x: 0, filter: "blur(0)" }}
											exit={{ opacity: 0, x: 15, filter: "blur(0.24rem)" }}
											className="text-sm text-red-500"
										>
											{errors.amount.message}
										</motion.p>
									)}
								</AnimatePresence>
							</div>
						</div>

						{/* Currency and Payment Method */}
						<div className="grid grid-cols-2 gap-3">
							{/* Currency Field */}
							<div className="grid gap-2">
								<Label htmlFor="currency">Currency *</Label>
								<Select
									value={selectedCurrency}
									onValueChange={(value) =>
										setValue("currency", value, { shouldValidate: true })
									}
									disabled={isPending}
								>
									<SelectTrigger
										id="currency"
										className={`w-full ${errors.currency ? "border-red-500" : ""}`}
									>
										<SelectValue placeholder="Select currency" />
									</SelectTrigger>
									<SelectContent>
										<SelectItem value="EUR">EUR</SelectItem>
										<SelectItem value="USD">USD</SelectItem>
									</SelectContent>
								</Select>
								<div className="h-5">
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
												className="text-sm text-red-500"
											>
												{errors.currency.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>

							{/* Payment Method Field */}
							<div className="grid gap-2">
								<Label htmlFor="paymentMethod">Payment Method *</Label>
								<Select
									value={selectedPaymentMethod}
									onValueChange={(value) =>
										setValue("paymentMethod", value, { shouldValidate: true })
									}
									disabled={isPending}
								>
									<SelectTrigger
										id="paymentMethod"
										className={`w-full ${errors.paymentMethod ? "border-red-500" : ""}`}
									>
										<SelectValue placeholder="Select method" />
									</SelectTrigger>
									<SelectContent>
										<SelectItem value="CreditCard">Credit Card</SelectItem>
										<SelectItem value="BankTransfer">Bank Transfer</SelectItem>
										<SelectItem value="PayPal">PayPal</SelectItem>
										<SelectItem value="Cash">Cash</SelectItem>
									</SelectContent>
								</Select>
								<div className="h-5">
									<AnimatePresence>
										{errors.paymentMethod && (
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
												className="text-sm text-red-500"
											>
												{errors.paymentMethod.message}
											</motion.p>
										)}
									</AnimatePresence>
								</div>
							</div>
						</div>

						<div className="flex justify-between gap-2 mt-2">
							<Button
								type="button"
								variant="outline"
								size="sm"
								disabled={isPending}
								onClick={handleCancel}
							>
								Cancel
							</Button>
							<Button
								type="submit"
								variant="default"
								size="sm"
								disabled={isPending || !isValid}
								className="bg-green-600 hover:bg-green-700 dark:bg-green-600 dark:hover:bg-green-700"
							>
								{isPending ? (
									<>
										<Spinner className="h-4 w-4 mr-1" />
										Adding...
									</>
								) : (
									<>Add Payment</>
								)}
							</Button>
						</div>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
