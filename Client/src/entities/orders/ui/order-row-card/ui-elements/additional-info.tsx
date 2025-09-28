import { FC } from "react";
import { AlertCircle, Info } from "lucide-react";

import { Separator } from "@shared/ui/separator";

import { OrdersResponse } from "../../../api";

type AdditionalInfoProps = {
	order: OrdersResponse;
};

export const AdditionalInfo: FC<AdditionalInfoProps> = ({ order }) => {
	return (
		<>
			{(order.info ||
				order.cancellationReason ||
				order.returnReason ||
				order.refundReason) && (
				<>
					<Separator />
					<div className="space-y-3">
						{order.info && (
							<div className="flex items-start space-x-2">
								<Info className="h-4 w-4 text-blue-500 mt-0.5 flex-shrink-0" />
								<div className="text-sm">
									<span className="font-medium">Info:</span>
									<span className="text-muted-foreground ml-2">{order.info}</span>
								</div>
							</div>
						)}
						{order.returnReason && (
							<div className="flex items-start space-x-2">
								<AlertCircle className="h-4 w-4 text-orange-500 mt-0.5 flex-shrink-0" />
								<div className="text-sm">
									<span className="font-medium">Return:</span>
									<span className="text-muted-foreground ml-2">
										{order.returnReason}
									</span>
								</div>
							</div>
						)}
						{order.refundReason && (
							<div className="flex items-start space-x-2">
								<AlertCircle className="h-4 w-4 text-purple-500 mt-0.5 flex-shrink-0" />
								<div className="text-sm">
									<span className="font-medium">Refund:</span>
									<span className="text-muted-foreground ml-2">
										{order.refundReason}
									</span>
								</div>
							</div>
						)}
					</div>
				</>
			)}
		</>
	);
};
