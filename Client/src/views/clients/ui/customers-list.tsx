import { FC, useLayoutEffect, useState } from "react";
import { Reorder } from "motion/react";

import { EditUser } from "@features/users/edit";
import { DeleteUser } from "@features/users/delete";

import { Customer, CustomerRowCard } from "@entities/users";

type CustomersList = {
	customers: Array<Customer>;
};

export const CustomersList: FC<CustomersList> = ({ customers }) => {
	const [items, setItems] = useState(customers);

	useLayoutEffect(() => {
		setItems(customers);
	}, [customers]);

	return (
		<Reorder.Group axis="y" values={items} onReorder={setItems} className="space-y-4">
			{items.map((customer) => (
				<Reorder.Item key={customer.id} value={customer}>
					<CustomerRowCard customer={customer}>
						<CustomerRowCard.ManagementActions>
							<EditUser userId={customer.id} />
							<DeleteUser userId={customer.id} userFullName={customer.fullName} />
						</CustomerRowCard.ManagementActions>
					</CustomerRowCard>
				</Reorder.Item>
			))}
		</Reorder.Group>
	);
};
