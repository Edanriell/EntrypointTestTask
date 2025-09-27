import { FC, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { RefreshCcw } from "lucide-react";

import { OrderStatus } from "@entities/orders";

import { Button } from "@shared/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@shared/ui/popover";
import { Label } from "@shared/ui/label";
import { Textarea } from "@shared/ui/textarea";
import { Spinner } from "@shared/ui/spinner";

import { useRefundPayments } from "../api";
import { type RefundPaymentsFormData, refundPaymentsSchema } from "../model";

type RefundPaymentsProps = {
	orderId: string;
	orderNumber?: string;
	orderStatus: string;
	paidAmount: number;
	currency: string;
};

export const Refund: FC<RefundPaymentsProps> = ({
	orderId,
	orderNumber,
	orderStatus,
	paidAmount,
	currency
}) => {
	const [isOpen, setIsOpen] = useState(false);
	const { mutateAsync: processRefund, isPending } = useRefundPayments();

	const {
		register,
		handleSubmit,
		formState: { errors, isValid },
		reset,
		watch
	} = useForm<RefundPaymentsFormData>({
		resolver: zodResolver(refundPaymentsSchema),
		mode: "onChange",
		defaultValues: {
			refundReason: ""
		}
	});

	const refundReason = watch("refundReason");

	const onSubmit = async (data: RefundPaymentsFormData) => {
		try {
			await processRefund({
				orderId,
				refundReason: data.refundReason
			});
			setIsOpen(false);
			reset();
		} catch (error) {
			console.error("Error refunding payments:", error);
		}
	};

	const handleOpenChange = (open: boolean) => {
		setIsOpen(open);
		if (!open) {
			reset();
		}
	};

	// Only show button for returned orders
	if (orderStatus !== OrderStatus.Returned) {
		return null;
	}

	// Don't show if no payments to refund
	if (paidAmount <= 0) {
		return null;
	}

	return (
		<Popover open={isOpen} onOpenChange={handleOpenChange}>
			<PopoverTrigger asChild>
				<Button
					variant="outline"
					size="sm"
					className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300"
				>
					<RefreshCcw className="mr-2 h-4 w-4" />
					Refund Payments
				</Button>
			</PopoverTrigger>
			<PopoverContent className="w-96">
				<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
					<div className="space-y-2">
						<h4 className="text-lg font-medium">Refund Payments</h4>
						<p className="text-sm text-muted-foreground">
							{orderNumber ? `Order #${orderNumber}` : `Order ID: ${orderId}`}
						</p>
						<div className="bg-red-50 dark:bg-red-950/20 p-3 rounded-md">
							<p className="text-sm font-medium text-red-800 dark:text-red-200">
								Refund Amount: {paidAmount.toFixed(2)} {currency}
							</p>
							<p className="text-xs text-red-600 dark:text-red-400 mt-1">
								This will refund all payments for this returned order.
							</p>
						</div>
					</div>
					<div className="space-y-2">
						<Label htmlFor="refundReason">
							Refund Reason <span className="text-red-500">*</span>
						</Label>
						<Textarea
							id="refundReason"
							placeholder="Please explain why you are refunding this order..."
							className={`min-h-[100px] ${errors.refundReason ? "border-red-500" : ""}`}
							{...register("refundReason")}
						/>
						{errors.refundReason && (
							<p className="text-sm text-red-500">{errors.refundReason.message}</p>
						)}
						<div className="flex justify-between">
							<p className="text-xs text-muted-foreground">
								{refundReason.length}/500 characters
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
							className="bg-red-600 hover:bg-red-700 text-white"
						>
							{isPending ? (
								<>
									<Spinner className="mr-2 h-4 w-4" />
									Processing...
								</>
							) : (
								"Refund Payments"
							)}
						</Button>
					</div>
				</form>
			</PopoverContent>
		</Popover>
	);
};
