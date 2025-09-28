import { FC } from "react";
import { CreditCard, Package } from "lucide-react";

import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@shared/ui/accordion";
import { Card, CardContent, CardHeader, CardTitle } from "@shared/ui/card";
import { Badge } from "@shared/ui/badge";
import { Separator } from "@shared/ui/separator";
import { cn, formatCurrency } from "@shared/lib/utils";

import type { PaymentResponse } from "../../../api";

import { getPaymentStatusBadge } from "../helpers";

type PaymentsProps = {
	payments?: Array<PaymentResponse>;
	remainingAmount: number;
};

export const Payments: FC<PaymentsProps> = ({ payments, remainingAmount }) => {
	if (!payments || payments.length === 0) return null;

	const totalPaid = payments.reduce(
		(sum, payment) =>
			payment.paymentStatus.toLowerCase() === "paid" ? sum + payment.paymentTotalAmount : sum,
		0
	);

	return (
		<Accordion type="single" collapsible className="w-full">
			<AccordionItem value="payments">
				<AccordionTrigger className="flex items-center justify-between px-2 py-2 rounded-lg hover:bg-muted/40">
					<div className="flex items-center space-x-2">
						<Package className="h-4 w-4 text-primary" />
						<span className="font-semibold text-foreground">
							Payments ({payments.length})
						</span>
					</div>
				</AccordionTrigger>
				<AccordionContent>
					<div className="space-y-3 mt-2">
						{payments.map((payment, index) => {
							const status = getPaymentStatusBadge(payment.paymentStatus);
							const StatusIcon = status.icon;

							return (
								<Card
									key={payment.paymentId}
									className="shadow-sm hover:shadow-md transition-all duration-200 border-border/60"
								>
									<CardHeader className="pb-2">
										<div className="flex items-center justify-between">
											<CardTitle className="text-sm font-medium flex items-center gap-2">
												<CreditCard className="h-4 w-4 text-muted-foreground" />
												Payment #{index + 1}
											</CardTitle>

											<Badge
												variant="outline"
												className={cn(
													"flex items-center gap-1 text-xs font-medium",
													status.className
												)}
											>
												<StatusIcon className="h-3.5 w-3.5" />
												{status.label}
											</Badge>
										</div>
									</CardHeader>
									<CardContent className="pt-0 text-sm">
										<div className="flex justify-between items-center">
											<div className="text-muted-foreground">
												ID:{" "}
												<span className="text-foreground font-medium">
													{payment.paymentId}
												</span>
											</div>
											<div className="font-semibold text-foreground">
												{formatCurrency(payment.paymentTotalAmount)}
											</div>
										</div>
										<div className="flex justify-between mt-1 text-xs text-muted-foreground">
											<span>Status:</span>
											<span className="capitalize">
												{payment.paymentStatus}
											</span>
										</div>
									</CardContent>
								</Card>
							);
						})}
						<Card className="bg-muted/40 border-border/60 mt-4">
							<CardContent className="py-3">
								<div className="space-y-2 text-sm">
									<div className="flex justify-between items-center">
										<span className="font-medium">Total Paid:</span>
										<span className="font-semibold text-green-600">
											{formatCurrency(totalPaid)}
										</span>
									</div>
									{remainingAmount > 0 && (
										<>
											<Separator className="my-1" />
											<div className="flex justify-between items-center">
												<span className="font-medium">Remaining:</span>
												<span className="font-semibold text-amber-600">
													{formatCurrency(remainingAmount)}
												</span>
											</div>
										</>
									)}
								</div>
							</CardContent>
						</Card>
					</div>
				</AccordionContent>
			</AccordionItem>
		</Accordion>
	);
};
