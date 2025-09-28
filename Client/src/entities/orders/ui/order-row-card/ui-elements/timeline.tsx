import { FC } from "react";
import { CheckCircle2, Package, PackageCheck, TruckIcon, Zap } from "lucide-react";

import { cn } from "@shared/lib/utils";

import { OrdersResponse } from "../../../api";
import { OrderStatus } from "../../../model";

import { isStatusCompleted } from "../helpers";

type TimelineProps = {
	order: OrdersResponse;
};

export const Timeline: FC<TimelineProps> = ({ order }) => {
	const timelineItems = [
		{
			label: "Confirmed",
			status: OrderStatus.Confirmed,
			completed: isStatusCompleted({ order, targetStatus: OrderStatus.Confirmed }),
			color: "bg-green-500",
			icon: <CheckCircle2 className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Processing",
			status: OrderStatus.Processing,
			completed: isStatusCompleted({ order, targetStatus: OrderStatus.Processing }),
			color: "bg-blue-500",
			icon: <Zap className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Ready",
			status: OrderStatus.ReadyForShipment,
			completed: isStatusCompleted({ order, targetStatus: OrderStatus.ReadyForShipment }),
			color: "bg-amber-500",
			icon: <PackageCheck className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Shipped",
			status: OrderStatus.Shipped,
			completed: isStatusCompleted({ order, targetStatus: OrderStatus.Shipped }),
			color: "bg-purple-500",
			icon: <TruckIcon className="h-2.5 w-2.5 text-white" />
		},
		{
			label: "Delivered",
			status: OrderStatus.Delivered,
			completed: isStatusCompleted({ order, targetStatus: OrderStatus.Delivered }),
			color: "bg-emerald-500",
			icon: <Package className="h-2.5 w-2.5 text-white" />
		}
	];

	const getVisibleTimelineItems = () => {
		if (
			[OrderStatus.Cancelled, OrderStatus.Returned, OrderStatus.Failed].includes(
				order.status as OrderStatus
			)
		) {
			return timelineItems.filter((item) => item.completed);
		}
		return timelineItems;
	};

	return (
		<>
			{getVisibleTimelineItems().length > 0 && (
				<div className="space-y-4">
					<h4 className="text-sm font-medium text-foreground">Order Progress</h4>
					<div className="relative px-4 pb-12">
						<div className="flex items-center justify-between relative h-6">
							{getVisibleTimelineItems().map((item, index) => (
								<div
									key={item.status}
									className="flex flex-col items-center relative z-20"
								>
									<div
										className={cn(
											"h-6 w-6 rounded-full border-2 flex items-center justify-center shadow-sm",
											item.completed
												? cn(item.color, "border-transparent")
												: "bg-white border-muted-foreground"
										)}
									>
										{item.completed ? (
											item.icon
										) : (
											<div className="h-2 w-2 rounded-full bg-muted-foreground" />
										)}
									</div>
									{index < getVisibleTimelineItems().length - 1 && (
										<div
											className="absolute top-3 left-6 h-0.5 -z-10"
											style={{
												width: `calc(100vw / ${getVisibleTimelineItems().length} - 24px)`,
												maxWidth: "300px"
											}}
										>
											<div
												className={cn(
													"h-full transition-all duration-300",
													item.completed &&
														getVisibleTimelineItems()[index + 1]
															?.completed
														? "bg-primary"
														: item.completed
															? "bg-gradient-to-r from-primary to-muted"
															: "bg-muted"
												)}
											/>
										</div>
									)}
									<div className="absolute top-8 left-1/2 transform -translate-x-1/2 text-center min-w-max">
										<div
											className={cn(
												"text-xs font-medium whitespace-nowrap",
												item.completed
													? "text-foreground"
													: "text-muted-foreground"
											)}
										>
											{item.label}
										</div>
										<div className="h-4 mt-1 flex items-center justify-center">
											{item.completed && item.status === order.status && (
												<div className="text-xs text-primary font-medium">
													Current
												</div>
											)}
											{!item.completed && (
												<div className="text-xs text-muted-foreground">
													Pending
												</div>
											)}
										</div>
									</div>
								</div>
							))}
						</div>
					</div>
				</div>
			)}
		</>
	);
};
