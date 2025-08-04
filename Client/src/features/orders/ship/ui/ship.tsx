import { FC, useState } from "react";
import { Truck } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { Courier, OrderStatus } from "@entities/orders";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";
import { Spinner } from "@shared/ui/spinner";

import { useShipOrder } from "../api";
import { ShipOrderFormData, shipOrderSchema } from "../model";

type ShipOrderProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
};

// Mapping from frontend enum strings to backend numeric values
const courierToNumber = {
	[Courier.DHL]: 0,
	[Courier.FedEx]: 1,
	[Courier.UPS]: 2,
	[Courier.DPD]: 3
};

export const Ship: FC<ShipOrderProps> = ({ orderId, orderNumber, orderStatus }) => {
	const [isOpen, setIsOpen] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		reset,
		setError,
		setValue,
		watch
	} = useForm<ShipOrderFormData>({
		resolver: zodResolver(shipOrderSchema),
		mode: "onTouched",
		defaultValues: {
			trackingNumber: "",
			courier: undefined,
			estimatedDeliveryDate: ""
		}
	});

	const { mutateAsync: shipOrder, isPending } = useShipOrder(reset, setError);

	const onSubmit = async (data: ShipOrderFormData) => {
		try {
			await shipOrder({
				orderId,
				orderData: {
					trackingNumber: data.trackingNumber.trim(),
					courier: data.courier, // This will be the numeric value
					estimatedDeliveryDate: new Date(data.estimatedDeliveryDate).toISOString()
				}
			});
			setIsOpen(false);
		} catch (error) {
			console.error("Error shipping order:", error);
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

	// Handle courier selection - convert string enum to numeric value
	const handleCourierChange = (courierString: string) => {
		const numericValue = courierToNumber[courierString as Courier];
		setValue("courier", numericValue, { shouldValidate: true });
	};

	// Only show button if order is ready for shipment
	if (orderStatus !== OrderStatus.ReadyForShipment) {
		return null;
	}

	return (
		<Popover open={isOpen} onOpenChange={handleOpenChange}>
			<PopoverTrigger asChild>
				<Button
					variant="outline"
					size="sm"
					className="text-purple-600 dark:text-purple-400"
				>
					<Truck className="mr-2 h-4 w-4" />
					Ship Order
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-96">
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid gap-4">
						<div className="space-y-2">
							<h4 className="font-medium leading-none">Ship Order</h4>
							<p className="text-sm text-muted-foreground">
								Provide shipping details for{" "}
								{orderNumber ? `order "${orderNumber}"` : "this order"}
							</p>
						</div>

						{/* Tracking Number */}
						<div className="grid gap-2 relative">
							<Label htmlFor="trackingNumber">Tracking Number *</Label>
							<Input
								id="trackingNumber"
								placeholder="Enter tracking number..."
								className={errors.trackingNumber ? "border-red-500" : ""}
								disabled={isPending}
								{...register("trackingNumber")}
							/>
							<AnimatePresence>
								{errors.trackingNumber && (
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
										{errors.trackingNumber.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>

						{/* Courier Selection */}
						<div className="grid gap-2 relative mt-4">
							<Label htmlFor="courier">Courier *</Label>
							<Select onValueChange={handleCourierChange} disabled={isPending}>
								<SelectTrigger className={errors.courier ? "border-red-500" : ""}>
									<SelectValue placeholder="Select courier" />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value={Courier.DHL}>{Courier.DHL}</SelectItem>
									<SelectItem value={Courier.FedEx}>{Courier.FedEx}</SelectItem>
									<SelectItem value={Courier.UPS}>{Courier.UPS}</SelectItem>
									<SelectItem value={Courier.DPD}>{Courier.DPD}</SelectItem>
								</SelectContent>
							</Select>
							<AnimatePresence>
								{errors.courier && (
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
										{errors.courier.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>

						{/* Estimated Delivery Date */}
						<div className="grid gap-2 relative mt-4">
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
								className="bg-purple-600 hover:bg-purple-700 dark:bg-purple-600 dark:hover:bg-purple-700"
							>
								{isPending ? (
									<>
										<Spinner className="h-4 w-4 mr-1" />
										Shipping...
									</>
								) : (
									<>Ship Order</>
								)}
							</Button>
						</div>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
