// src/features/orders/mark-out-for-delivery/ui/mark-out-for-delivery.tsx
import { FC, useState } from "react";
import { Truck } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { OrderStatus } from "@entities/orders";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Spinner } from "@shared/ui/spinner";

import { useMarkOutForDelivery } from "../api";
import { MarkOutForDeliveryFormData, markOutForDeliverySchema } from "../model";

type MarkOutForDeliveryProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
};

export const MarkOutForDelivery: FC<MarkOutForDeliveryProps> = ({
	orderId,
	orderNumber,
	orderStatus
}) => {
	const [isOpen, setIsOpen] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		reset,
		setError
	} = useForm<MarkOutForDeliveryFormData>({
		resolver: zodResolver(markOutForDeliverySchema),
		mode: "onTouched",
		defaultValues: {
			estimatedDeliveryDate: ""
		}
	});

	const { mutateAsync: markOutForDelivery, isPending } = useMarkOutForDelivery(reset, setError);

	const onSubmit = async (data: MarkOutForDeliveryFormData) => {
		try {
			await markOutForDelivery({
				orderId,
				estimatedDeliveryDate: new Date(data.estimatedDeliveryDate).toISOString()
			});
			setIsOpen(false);
		} catch (error) {
			console.error("Error marking order out for delivery:", error);
		}
	};

	const handleOpenChange = (open: boolean) => {
		setIsOpen(open);
		if (!open) {
			reset();
		}
	};

	const handleCancel = () => {
		setIsOpen(false);
		reset();
	};

	// Get tomorrow's date as minimum
	const getTomorrowDate = () => {
		const tomorrow = new Date();
		tomorrow.setDate(tomorrow.getDate() + 1);
		return tomorrow.toISOString().split("T")[0];
	};

	// Only show button if order is shipped
	if (orderStatus !== OrderStatus.Shipped) {
		return null;
	}

	return (
		<Popover open={isOpen} onOpenChange={handleOpenChange}>
			<PopoverTrigger asChild>
				<Button
					variant="outline"
					size="sm"
					className="text-orange-600 dark:text-orange-400"
				>
					<Truck className="mr-2 h-4 w-4" />
					Mark Out for Delivery
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-96">
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid gap-4">
						<div className="space-y-2">
							<h4 className="font-medium leading-none">Mark Out for Delivery</h4>
							<p className="text-sm text-muted-foreground">
								Update delivery date for{" "}
								{orderNumber ? `order "${orderNumber}"` : "this order"}
							</p>
						</div>

						{/* Estimated Delivery Date */}
						<div className="grid gap-2 relative">
							<Label htmlFor="estimatedDeliveryDate">Estimated Delivery Date *</Label>
							<Input
								id="estimatedDeliveryDate"
								type="date"
								min={getTomorrowDate()}
								className={errors.estimatedDeliveryDate ? "border-red-500" : ""}
								disabled={isPending}
								{...register("estimatedDeliveryDate")}
							/>
							<AnimatePresence>
								{errors.estimatedDeliveryDate && (
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
										{errors.estimatedDeliveryDate.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>

						<div className="flex justify-between gap-2 mt-6">
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
								className="bg-orange-600 hover:bg-orange-700 dark:bg-orange-600 dark:hover:bg-orange-700"
							>
								{isPending ? (
									<>
										<Spinner className="h-4 w-4 mr-1" />
										Updating...
									</>
								) : (
									<>Mark Out for Delivery</>
								)}
							</Button>
						</div>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
