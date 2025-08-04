import { FC, useState } from "react";
import { Ban } from "lucide-react";
import { AnimatePresence, motion } from "motion/react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { OrderStatus } from "@entities/orders";

import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Button } from "@shared/ui/button";
import { Textarea } from "@shared/ui/textarea";
import { Label } from "@shared/ui/label";
import { Spinner } from "@shared/ui/spinner";

import { useCancelOrder } from "../api";
import { CancelOrderFormData, cancelOrderSchema } from "../model";

type CancelOrderProps = {
	orderId: string;
	orderNumber: string;
	orderStatus: string;
};

export const CancelOrder: FC<CancelOrderProps> = ({ orderId, orderNumber, orderStatus }) => {
	const [isOpen, setIsOpen] = useState(false);

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		reset,
		setError
	} = useForm<CancelOrderFormData>({
		resolver: zodResolver(cancelOrderSchema),
		mode: "onTouched",
		defaultValues: {
			cancellationReason: ""
		}
	});

	const { mutateAsync: cancelOrder, isPending } = useCancelOrder(reset, setError);

	const onSubmit = async (data: CancelOrderFormData) => {
		try {
			await cancelOrder({
				orderId,
				cancellationReason: data.cancellationReason.trim()
			});
			setIsOpen(false); // Close popover on successful submission
		} catch (error) {
			console.error("Error cancelling order:", error);
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
				<Button
					variant="outline"
					size="sm"
					className="text-orange-600 dark:text-orange-400"
				>
					<Ban className="mr-2 h-4 w-4" />
					Cancel Order
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-90">
				<form onSubmit={handleSubmit(onSubmit)}>
					<div className="grid gap-4">
						<div className="space-y-2">
							<h4 className="font-medium leading-none">Cancel Order</h4>
							<p className="text-sm text-muted-foreground">
								Are you sure you want to cancel{" "}
								{orderNumber ? `order "${orderNumber}"` : "this order"}?
							</p>
						</div>

						<div className="grid gap-2 relative">
							<Label htmlFor="cancellationReason">Cancellation Reason *</Label>
							<Textarea
								id="cancellationReason"
								placeholder="Please provide a reason for cancellation..."
								className={`resize-none min-h-[80px] ${
									errors.cancellationReason ? "border-red-500" : ""
								}`}
								disabled={isPending}
								{...register("cancellationReason")}
							/>
							<AnimatePresence>
								{errors.cancellationReason && (
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
										{errors.cancellationReason.message}
									</motion.p>
								)}
							</AnimatePresence>
						</div>

						<div className="flex justify-between gap-2 mt-4">
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
								variant="destructive"
								size="sm"
								disabled={isPending || !isValid}
							>
								{isPending ? (
									<>
										<Spinner className="h-4 w-4 mr-1" />
										Cancelling...
									</>
								) : (
									<>Cancel Order</>
								)}
							</Button>
						</div>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
