import { FC, ReactNode } from "react";
import { Calendar, Mail, MapPin, Phone } from "lucide-react";

import { Card, CardContent } from "@shared/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@shared/ui/avatar";
import { formatCurrency, formatDate } from "@shared/lib/utils";

import { Customer } from "../../model";
import { getInitials } from "../../lib";

import { ManagementActions } from "./management-actions";

type CustomerRowCardComponents = {
	ManagementActions: typeof ManagementActions;
};

type CustomerRowCardProps = {
	customer: Customer & {
		// Customer image is not present in our system, however, easily can be added.
		imageSrc?: string;
	};
	children: ReactNode;
};

type CustomerRowCard = FC<CustomerRowCardProps> & CustomerRowCardComponents;

export const CustomerRowCard: CustomerRowCard = ({ customer, children }) => {
	return (
		<Card className="w-full hover:shadow-md transition-shadow duration-200 pt-[unset] pb-[unset]">
			<CardContent className="pt-4 pb-4 pl-6 pr-6">
				<div className="flex items-center justify-between gap-4">
					<div className="flex items-center gap-4 flex-1 min-w-0">
						<Avatar className="h-12 w-12 flex-shrink-0">
							<AvatarImage src={customer.imageSrc} alt={customer.fullName} />
							<AvatarFallback className="bg-primary/10 text-primary font-semibold">
								{getInitials(customer.fullName)}
							</AvatarFallback>
						</Avatar>
						<div className="flex-1 min-w-0">
							<div className="flex items-center gap-2 mb-1">
								<h3 className="font-semibold text-foreground truncate">
									{customer.fullName}
								</h3>
							</div>
							<div className="flex items-center gap-1 text-sm text-muted-foreground mb-1">
								<Mail className="h-3 w-3" />
								<span className="truncate">{customer.email}</span>
							</div>
						</div>
					</div>
					<div className="hidden md:flex flex-col gap-1 text-sm text-muted-foreground min-w-0 flex-shrink-0 xl:mr-8">
						{customer.phoneNumber && (
							<div className="flex items-center gap-1">
								<Phone className="h-3 w-3" />
								<span>{customer.phoneNumber}</span>
							</div>
						)}
						{customer.fullAddress && (
							<div className="flex items-center gap-1">
								<MapPin className="h-3 w-3" />
								<span className="truncate">{customer.fullAddress}</span>
							</div>
						)}
					</div>
					<div className="hidden lg:flex flex-row gap-1 text-sm min-w-0 flex-shrink-0 xl:mr-8">
						{customer.totalOrders !== undefined && (
							<div className="text-center">
								<div className="font-semibold text-foreground">
									{customer.totalOrders}
								</div>
								<div className="text-xs text-muted-foreground">Orders</div>
							</div>
						)}
						{customer.totalSpent !== undefined && (
							<div className="text-center">
								<div className="font-semibold text-foreground">
									{formatCurrency(customer.totalSpent)}
								</div>
								<div className="text-xs text-muted-foreground">Spent</div>
							</div>
						)}
					</div>
					<div className="hidden sm:flex items-center gap-1 text-sm text-muted-foreground flex-shrink-0">
						<Calendar className="h-3 w-3" />
						<span>{formatDate(customer.createdOnUtc)}</span>
					</div>
					{children}
				</div>
			</CardContent>
		</Card>
	);
};

CustomerRowCard.ManagementActions = ManagementActions;
