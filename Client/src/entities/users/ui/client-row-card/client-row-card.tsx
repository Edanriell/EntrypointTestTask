import { FC, ReactNode } from "react";
import { Calendar, Mail, MapPin, Phone } from "lucide-react";

import { getInitials } from "@entities/users/lib";

import { Card, CardContent } from "@shared/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@shared/ui/avatar";
// import {
// 	DropdownMenu,
// 	DropdownMenuContent,
// 	DropdownMenuGroup,
// 	DropdownMenuTrigger
// } from "@shared/ui/dropdown-menu";
import { formatCurrency, formatDate } from "@shared/lib/utils";

import { ManagementActions } from "@entities/users/ui/client-row-card/management-actions";

type ClientRowCardComponents = {
	ManagementActions: typeof ManagementActions;
};

type ClientRowCardProps = {
	client: {
		id: string | number;
		name: string;
		email: string;
		phone?: string;
		company?: string;
		status: "active" | "inactive" | "pending";
		location?: string;
		createdAt: string | Date;
		avatar?: string;
		totalOrders?: number;
		totalSpent?: number;
	};
	children: ReactNode;
};

type ClientRowCard = FC<ClientRowCardProps> & ClientRowCardComponents;

export const ClientRowCard: ClientRowCard = ({ client, children }) => {
	return (
		<Card className="w-full hover:shadow-md transition-shadow duration-200 pt-[unset] pb-[unset]">
			<CardContent className="pt-4 pb-4 pl-6 pr-6">
				<div className="flex items-center justify-between gap-4">
					{/* Client Info Section */}
					<div className="flex items-center gap-4 flex-1 min-w-0">
						{/* Avatar */}
						<Avatar className="h-12 w-12 flex-shrink-0">
							<AvatarImage src={client.avatar} alt={client.name} />
							<AvatarFallback className="bg-primary/10 text-primary font-semibold">
								{getInitials(client.name)}
							</AvatarFallback>
						</Avatar>

						{/* Basic Info */}
						<div className="flex-1 min-w-0">
							<div className="flex items-center gap-2 mb-1">
								<h3 className="font-semibold text-foreground truncate">
									{client.name}
								</h3>
							</div>

							<div className="flex items-center gap-1 text-sm text-muted-foreground mb-1">
								<Mail className="h-3 w-3" />
								<span className="truncate">{client.email}</span>
							</div>
						</div>
					</div>

					{/* Contact Info Section */}
					<div className="hidden md:flex flex-col gap-1 text-sm text-muted-foreground min-w-0 flex-shrink-0 xl:mr-8">
						{client.phone && (
							<div className="flex items-center gap-1">
								<Phone className="h-3 w-3" />
								<span>{client.phone}</span>
							</div>
						)}
						{client.location && (
							<div className="flex items-center gap-1">
								<MapPin className="h-3 w-3" />
								<span className="truncate">{client.location}</span>
							</div>
						)}
					</div>

					{/* Stats Section */}
					<div className="hidden lg:flex flex-row gap-1 text-sm min-w-0 flex-shrink-0 xl:mr-8">
						{client.totalOrders !== undefined && (
							<div className="text-center">
								<div className="font-semibold text-foreground">
									{client.totalOrders}
								</div>
								<div className="text-xs text-muted-foreground">Orders</div>
							</div>
						)}
						{client.totalSpent !== undefined && (
							<div className="text-center">
								<div className="font-semibold text-foreground">
									{formatCurrency(client.totalSpent)}
								</div>
								<div className="text-xs text-muted-foreground">Spent</div>
							</div>
						)}
					</div>

					{/* Created Date Section */}
					<div className="hidden sm:flex items-center gap-1 text-sm text-muted-foreground flex-shrink-0">
						<Calendar className="h-3 w-3" />
						<span>{formatDate(client.createdAt)}</span>
					</div>

					{children}

					{/* Edit Button */}
					{/*<DropdownMenu open={open} onOpenChange={setOpen}>*/}
					{/*	<DropdownMenuTrigger asChild>*/}
					{/*		<Button variant="ghost" size="sm">*/}
					{/*			<MoreHorizontal />*/}
					{/*		</Button>*/}
					{/*	</DropdownMenuTrigger>*/}
					{/*	<DropdownMenuContent align="end" className="w-[200px]">*/}
					{/*		<DropdownMenuGroup>*/}
					{/*			/!*Edit*!/*/}
					{/*			/!*Delete*!/*/}
					{/*		</DropdownMenuGroup>*/}
					{/*	</DropdownMenuContent>*/}
					{/*</DropdownMenu>*/}
				</div>
			</CardContent>
		</Card>
	);
};

ClientRowCard.ManagementActions = ManagementActions;
