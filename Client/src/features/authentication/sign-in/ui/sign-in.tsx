import { ComponentProps } from "react";
import Image from "next/image";

import { cn } from "@shared/lib/functions";
import { Button } from "@shared/ui/button";
import { Card, CardContent } from "@shared/ui/card";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";

export function SignIn({ className, ...props }: ComponentProps<"div">) {
	return (
		<div className={cn("flex flex-col gap-6", className)} {...props}>
			<Card className="overflow-hidden p-0">
				<CardContent className="grid p-0 md:grid-cols-2">
					<form className="p-6 md:p-8">
						<div className="flex flex-col gap-6">
							<div className="flex flex-col items-center text-center">
								<h1 className="text-2xl font-bold">Welcome back</h1>
								<p className="text-muted-foreground text-balance">
									Login to your account
								</p>
							</div>
							<div className="grid gap-3">
								<Label htmlFor="email">Email</Label>
								<Input
									id="email"
									type="email"
									placeholder="m@example.com"
									required
								/>
							</div>
							<div className="grid gap-3">
								<Label htmlFor="password">Password</Label>
								<Input id="password" type="password" required />
							</div>
							<Button type="submit" className="w-full">
								Login
							</Button>
						</div>
					</form>
					<div className="bg-muted relative hidden md:block">
						<Image
							src="/images/banner.jpg"
							alt="Picture of woman working in office"
							className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
							layout="fill"
							priority
						/>
					</div>
				</CardContent>
			</Card>
		</div>
	);
}
