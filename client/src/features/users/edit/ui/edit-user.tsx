import { FC } from "react";
import { Edit } from "lucide-react";

import { DropdownMenuItem } from "@shared/ui/dropdown-menu";
import {
	Sheet,
	SheetClose,
	SheetContent,
	SheetDescription,
	SheetFooter,
	SheetHeader,
	SheetTitle,
	SheetTrigger
} from "@shared/ui/sheet";
import { Button } from "@shared/ui/button";
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";

export const EditUser: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<DropdownMenuItem onSelect={(e) => e.preventDefault()}>
					<Edit className="mr-2 h-4 w-4" />
					Edit
				</DropdownMenuItem>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Edit client</SheetTitle>
					<SheetDescription>
						Make changes to the client details below. Click save when you're done.
					</SheetDescription>
				</SheetHeader>
				<div className="grid flex-1 auto-rows-min gap-6 px-4">
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="edit-first-name">First Name</Label>
							<Input id="edit-first-name" placeholder="Enter first name" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="edit-last-name">Last Name</Label>
							<Input id="edit-last-name" placeholder="Enter last name" />
						</div>
					</div>
					<div className="grid gap-2">
						<Label htmlFor="edit-email">Email</Label>
						<Input id="edit-email" type="email" placeholder="Enter email address" />
					</div>
					<div className="grid gap-2">
						<Label htmlFor="edit-phone-number">Phone Number</Label>
						<Input id="edit-phone-number" type="tel" placeholder="Enter phone number" />
					</div>
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="edit-country">Country</Label>
							<Input id="edit-country" placeholder="Enter country" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="edit-city">City</Label>
							<Input id="edit-city" placeholder="Enter city" />
						</div>
					</div>
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="edit-zip-code">Zip Code</Label>
							<Input id="edit-zip-code" placeholder="Enter zip code" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="edit-street">Street</Label>
							<Input id="edit-street" placeholder="Enter street address" />
						</div>
					</div>
					<div className="grid gap-2">
						<Label htmlFor="edit-password">Password</Label>
						<Input
							id="edit-password"
							type="password"
							placeholder="Enter new password"
						/>
					</div>
				</div>
				<SheetFooter>
					<Button type="submit">Save Changes</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
