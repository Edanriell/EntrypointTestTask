"use client";

import { FC } from "react";

import { CreateUser } from "@features/users/create";
import { EditUser } from "@features/users/edit";
import { DeleteUser } from "@features/users/delete";

import { ClientRowCard } from "@entities/users";

const mockClients = [
	{
		id: 1,
		name: "John Doe",
		email: "john.doe@example.com",
		phone: "+1 (555) 123-4567",
		company: "Acme Corp",
		status: "active" as const,
		location: "New York, USA",
		createdAt: "2024-01-15",
		totalOrders: 12,
		totalSpent: 2450.0
	},
	{
		id: 2,
		name: "Jane Smith",
		email: "jane.smith@example.com",
		phone: "+1 (555) 987-6543",
		company: "Tech Solutions Inc",
		status: "pending" as const,
		location: "Los Angeles, USA",
		createdAt: "2024-02-20",
		totalOrders: 5,
		totalSpent: 890.0
	},
	{
		id: 3,
		name: "Mike Johnson",
		email: "mike.johnson@example.com",
		status: "inactive" as const,
		location: "Chicago, USA",
		createdAt: "2024-01-08",
		totalOrders: 0,
		totalSpent: 0
	}
];

// TODO
// Data must be fetched in a server component !

export const ClientsPage: FC = () => {
	return (
		<>
			<div className="flex flex-1 flex-col gap-4 p-4">
				<CreateUser />
				{mockClients.map((client) => (
					<ClientRowCard key={client.id} client={client}>
						<ClientRowCard.ManagementActions>
							<EditUser />
							<DeleteUser />
						</ClientRowCard.ManagementActions>
					</ClientRowCard>
				))}
			</div>
		</>
	);
};
