// src/features/orders/return/ui/return-order.tsx
import { FC, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { RotateCcw } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Label } from "@shared/ui/label";
import { Textarea } from "@shared/ui/textarea";
import { Spinner } from "@shared/ui/spinner";

import { useReturnOrder } from "../api";
import { type ReturnOrderFormData, returnOrderSchema } from "../model";

type ReturnOrderProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
};

export const Return: FC<ReturnOrderProps> = ({ orderId, orderNumber, orderStatus }) => {
	const [isOpen, setIsOpen] = useState(false);
	const { mutateAsync: processReturn, isPending } = useReturnOrder();

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		reset,
		watch
	} = useForm<ReturnOrderFormData>({
		resolver: zodResolver(returnOrderSchema),
		mode: "onChange",
		defaultValues: {
			returnReason: ""
		}
	});

	const returnReason = watch("returnReason");

	const onSubmit = async (data: ReturnOrderFormData) => {
		try {
			await processReturn({
				orderId,
				returnReason: data.returnReason
			});
			setIsOpen(false);
			reset();
		} catch (error) {
			console.error("Error returning order:", error);
		}
	};

	const handleOpenChange = (open: boolean) => {
		setIsOpen(open);
		if (!open) {
			reset();
		}
	};

	// Only show button for completed orders
	if (orderStatus !== OrderStatus.Completed) {
		return null;
	}

	return (
		<Popover open={isOpen} onOpenChange={handleOpenChange}>
			<PopoverTrigger asChild>
				<Button
					variant="outline"
					size="sm"
					className="text-orange-600 dark:text-orange-400 hover:text-orange-700 dark:hover:text-orange-300"
				>
					<RotateCcw className="mr-2 h-4 w-4" />
					Return Order
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-96">
				<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
					<div className="space-y-2">
						<h4 className="text-lg font-medium">Return Order</h4>
						<p className="text-sm text-muted-foreground">
							{orderNumber ? `Order #${orderNumber}` : `Order ID: ${orderId}`}
						</p>
						<p className="text-sm text-muted-foreground">
							Please provide a reason for returning this order.
						</p>
					</div>

					<div className="space-y-2">
						<Label htmlFor="returnReason">
							Return Reason <span className="text-red-500">*</span>
						</Label>
						<Textarea
							id="returnReason"
							placeholder="Please explain why you want to return this order..."
							className={`min-h-[100px] ${errors.returnReason ? "border-red-500" : ""}`}
							{...register("returnReason")}
						/>
						{errors.returnReason && (
							<p className="text-sm text-red-500">{errors.returnReason.message}</p>
						)}
						<div className="flex justify-between">
							<p className="text-xs text-muted-foreground">
								{returnReason.length}/500 characters
							</p>
							<p className="text-xs text-muted-foreground">
								Min. 10 characters required
							</p>
						</div>
					</div>

					<div className="flex justify-end gap-2 pt-2">
						<Button
							type="button"
							variant="outline"
							onClick={() => handleOpenChange(false)}
							disabled={isPending}
						>
							Cancel
						</Button>
						<Button
							type="submit"
							disabled={!isValid || isPending}
							className="bg-orange-600 hover:bg-orange-700 text-white"
						>
							{isPending ? (
								<>
									<Spinner className="mr-2 h-4 w-4" />
									Processing...
								</>
							) : (
								"Return Order"
							)}
						</Button>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
