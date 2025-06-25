import { FC } from "react";
import { SquarePen } from "lucide-react";

import { Button } from "@shared/ui/button";
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
import { Label } from "@shared/ui/label";
import { Input } from "@shared/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/ui/select";

export const CreateUser: FC = () => {
	return (
		<Sheet>
			<SheetTrigger asChild>
				<Button className="flex w-fit self-end">
					<SquarePen />
					Create new
				</Button>
			</SheetTrigger>
			<SheetContent className="overflow-y-auto">
				<SheetHeader>
					<SheetTitle>Create client</SheetTitle>
					<SheetDescription>
						Fill in the details below to create a new client. Click save when you're
						done.
					</SheetDescription>
				</SheetHeader>
				<div className="grid flex-1 auto-rows-min gap-6 px-4">
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="first-name">First Name</Label>
							<Input id="first-name" placeholder="Enter first name" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="last-name">Last Name</Label>
							<Input id="last-name" placeholder="Enter last name" />
						</div>
					</div>
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="email">Email</Label>
						<Input id="email" type="email" placeholder="Enter email address" />
					</div>
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="phone-number">Phone Number</Label>
						<Input id="phone-number" type="tel" placeholder="Enter phone number" />
					</div>
					<div className="grid gap-2 col-span-full">
						<Label htmlFor="gender">Gender</Label>
						<Select>
							<SelectTrigger id="gender" className="w-full">
								<SelectValue placeholder="Select gender" />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="male">Male</SelectItem>
								<SelectItem value="female">Female</SelectItem>
							</SelectContent>
						</Select>
					</div>
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="country">Country</Label>
							<Input id="country" placeholder="Enter country" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="city">City</Label>
							<Input id="city" placeholder="Enter city" />
						</div>
					</div>
					<div className="grid grid-cols-2 gap-3">
						<div className="grid gap-2">
							<Label htmlFor="zip-code">Zip Code</Label>
							<Input id="zip-code" placeholder="Enter zip code" />
						</div>
						<div className="grid gap-2">
							<Label htmlFor="street">Street</Label>
							<Input id="street" placeholder="Enter street address" />
						</div>
					</div>
					<div className="grid gap-2">
						<Label htmlFor="password">Password</Label>
						<Input id="password" type="password" placeholder="Enter password" />
					</div>
				</div>
				<SheetFooter>
					<Button type="submit">Create New</Button>
					<SheetClose asChild>
						<Button variant="outline">Cancel</Button>
					</SheetClose>
				</SheetFooter>
			</SheetContent>
		</Sheet>
	);
};
