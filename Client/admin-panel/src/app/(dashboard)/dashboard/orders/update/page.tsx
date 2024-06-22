import {
	Button,
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
	Input,
	Label
} from "@shared/ui";

import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { Fragment } from "react";

export default function Dashboard() {
	return (
		<Fragment>
			<div className="flex items-center gap-4">
				<h1 className="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">
					Update Order OrderId
				</h1>
				<div className="hidden items-center gap-2 md:ml-auto md:flex">
					<Button variant="outline" size="sm">
						Discard
					</Button>
					<Button size="sm">Update Order</Button>
				</div>
			</div>
			<div className="grid gap-4 md:grid-cols-[1fr_250px] lg:grid-cols-3 lg:gap-8">
				<div className="grid auto-rows-max items-start gap-4 lg:col-span-2 lg:gap-8">
					<Card x-chunk="dashboard-07-chunk-0">
						<CardHeader>
							<CardTitle>Order Details</CardTitle>
							<CardDescription>Lipsum dolor sit amet, consectetur adipiscing elit</CardDescription>
						</CardHeader>
						<CardContent>
							<div className="grid gap-6">
								<div className="grid gap-3">
									<Label htmlFor="name">UserId</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="description">Ship Address</Label>
									<Textarea
										id="description"
										defaultValue="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam auctor, nisl nec ultricies ultricies, nunc nisl ultricies nunc, nec ultricies nunc nisl nec nunc."
										className="min-h-32"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="description">Order Information</Label>
									<Textarea
										id="description"
										defaultValue="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam auctor, nisl nec ultricies ultricies, nunc nisl ultricies nunc, nec ultricies nunc nisl nec nunc."
										className="min-h-32"
									/>
								</div>
							</div>
						</CardContent>
					</Card>
				</div>
				<div className="grid auto-rows-max items-start gap-4 lg:gap-8">
					<Card x-chunk="dashboard-07-chunk-3">
						<CardHeader>
							<CardTitle>Order Status</CardTitle>
						</CardHeader>
						<CardContent>
							<div className="grid gap-6">
								<div className="grid gap-3">
									<Label htmlFor="status">Status</Label>
									<Select>
										<SelectTrigger id="status" aria-label="Select status">
											<SelectValue placeholder="Select status" />
										</SelectTrigger>
										<SelectContent>
											<SelectItem value="draft">Created</SelectItem>
											<SelectItem value="draft">Pending</SelectItem>
											<SelectItem value="draft">Paid</SelectItem>
											<SelectItem value="draft">In Transit</SelectItem>
											<SelectItem value="draft">Delivered</SelectItem>
											<SelectItem value="draft">Cancelled</SelectItem>
										</SelectContent>
									</Select>
								</div>
							</div>
						</CardContent>
					</Card>
				</div>
			</div>
			<div className="flex items-center justify-center gap-2 md:hidden">
				<Button variant="outline" size="sm">
					Discard
				</Button>
				<Button size="sm">Save Product</Button>
			</div>
		</Fragment>
	);
}
