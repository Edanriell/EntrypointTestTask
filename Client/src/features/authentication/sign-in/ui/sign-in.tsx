// "use client";
//
// import { ComponentProps, FormEvent } from "react";
// import Image from "next/image";
//
// import { cn } from "@shared/lib/functions";
// import { Button } from "@shared/ui/button";
// import { Card, CardContent } from "@shared/ui/card";
// import { Input } from "@shared/ui/input";
// import { Label } from "@shared/ui/label";
// import { useAuth } from "@shared/lib/auth";
//
// export function SignIn({ className, ...props }: ComponentProps<"div">) {
// 	const { login } = useAuth();
//
// 	const handleSignIn = async (event: FormEvent<HTMLFormElement>) => {
// 		event.preventDefault();
//
// 		await login();
// 	};
//
// 	return (
// 		<div className={cn("flex flex-col gap-6", className)} {...props}>
// 			<Card className="overflow-hidden p-0">
// 				<CardContent className="grid p-0 md:grid-cols-2">
// 					<form onSubmit={handleSignIn} className="p-6 md:p-8">
// 						<div className="flex flex-col gap-6">
// 							<div className="flex flex-col items-center text-center">
// 								<h1 className="text-2xl font-bold">Welcome back</h1>
// 								<p className="text-muted-foreground text-balance">
// 									Login to your account
// 								</p>
// 							</div>
// 							<div className="grid gap-3">
// 								<Label htmlFor="email">Email</Label>
// 								<Input
// 									id="email"
// 									type="email"
// 									placeholder="m@example.com"
// 									required
// 								/>
// 							</div>
// 							<div className="grid gap-3">
// 								<Label htmlFor="password">Password</Label>
// 								<Input id="password" type="password" required />
// 							</div>
// 							<Button type="submit" className="w-full">
// 								Login
// 							</Button>
// 						</div>
// 					</form>
// 					<div className="bg-muted relative hidden md:block">
// 						<Image
// 							src="/images/banner.jpg"
// 							alt="Picture of woman working in office"
// 							className="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
// 							layout="fill"
// 							priority
// 						/>
// 					</div>
// 				</CardContent>
// 			</Card>
// 		</div>
// 	);
// }

// src/features/authentication/sign-in/ui/sign-in.tsx

"use client";

import { useState } from "react";
import Image from "next/image";

import { useCredentialsAuth } from "@features/authentication/general/lib/hooks";
import { Button } from "@shared/ui/button";
import { Input } from "@shared/ui/input";
import { Label } from "@shared/ui/label";
import { Card, CardContent } from "@shared/ui/card";
import { cn } from "@shared/lib/functions";

export function SignIn({ className, ...props }: { className?: string }) {
	const [credentials, setCredentials] = useState({
		email: "",
		password: ""
	});

	// const { directLogin, isLoading, error } = useDirectAuth();
	const { login, isLoading } = useCredentialsAuth();

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		// await directLogin(credentials);
		await login(credentials);
	};

	const handleInputChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
		setCredentials((prev) => ({
			...prev,
			[field]: e.target.value
		}));
	};

	return (
		<div className={cn("flex flex-col gap-6", className)} {...props}>
			<Card className="overflow-hidden p-0">
				<CardContent className="grid p-0 md:grid-cols-2">
					<form onSubmit={handleSubmit} className="p-6 md:p-8">
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
									value={credentials.email}
									onChange={handleInputChange("email")}
									required
									disabled={isLoading}
								/>
							</div>

							<div className="grid gap-3">
								<Label htmlFor="password">Password</Label>
								<Input
									id="password"
									type="password"
									value={credentials.password}
									onChange={handleInputChange("password")}
									required
									disabled={isLoading}
								/>
							</div>

							{/*{error && (*/}
							{/*	<div className="text-red-500 text-sm text-center">{error}</div>*/}
							{/*)}*/}

							<Button type="submit" className="w-full" disabled={isLoading}>
								{isLoading ? "Signing in..." : "Login"}
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
