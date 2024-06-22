import Image from "next/image";
import { Upload } from "lucide-react";
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

import { Fragment } from "react";

export default function Dashboard() {
	return (
		<Fragment>
			<div className="flex items-center gap-4">
				<h1 className="flex-1 shrink-0 whitespace-nowrap text-xl font-semibold tracking-tight sm:grow-0">
					Update Customer CustomerName
				</h1>
				<div className="hidden items-center gap-2 md:ml-auto md:flex">
					<Button variant="outline" size="sm">
						Discard
					</Button>
					<Button size="sm">Update Customer</Button>
				</div>
			</div>
			<div className="grid gap-4 md:grid-cols-[1fr_250px] lg:grid-cols-3 lg:gap-8">
				<div className="grid auto-rows-max items-start gap-4 lg:col-span-2 lg:gap-8">
					<Card x-chunk="dashboard-07-chunk-0">
						<CardHeader>
							<CardTitle>Customer Details</CardTitle>
							<CardDescription>Lipsum dolor sit amet, consectetur adipiscing elit</CardDescription>
						</CardHeader>
						<CardContent>
							<div className="grid gap-6">
								<div className="grid gap-3">
									<Label htmlFor="name">Username</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="name">Name</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="name">Surname</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="name">Password</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="name">Birth Date</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
								<div className="grid gap-3">
									<Label htmlFor="name">Gender</Label>
									<Input
										id="name"
										type="text"
										className="w-full"
										defaultValue="Gamer Gear Pro Controller"
									/>
								</div>
							</div>
						</CardContent>
					</Card>
				</div>
				<div className="grid auto-rows-max items-start gap-4 lg:gap-8">
					<Card className="overflow-hidden" x-chunk="dashboard-07-chunk-4">
						<CardHeader>
							<CardTitle>User Image</CardTitle>
							<CardDescription>Lipsum dolor sit amet, consectetur adipiscing elit</CardDescription>
						</CardHeader>
						<CardContent>
							<div className="grid gap-2">
								<Image
									alt="Product image"
									className="aspect-square w-full rounded-md object-cover"
									height="300"
									src="/placeholder.svg"
									width="300"
								/>
								<div className="grid grid-cols-3 gap-2">
									<button>
										<Image
											alt="Product image"
											className="aspect-square w-full rounded-md object-cover"
											height="84"
											src="/placeholder.svg"
											width="84"
										/>
									</button>
									<button>
										<Image
											alt="Product image"
											className="aspect-square w-full rounded-md object-cover"
											height="84"
											src="/placeholder.svg"
											width="84"
										/>
									</button>
									<button className="flex aspect-square w-full items-center justify-center rounded-md border border-dashed">
										<Upload className="h-4 w-4 text-muted-foreground" />
										<span className="sr-only">Upload</span>
									</button>
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
