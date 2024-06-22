"use client";

import { ChangeEventHandler, FormEvent, useState } from "react";
import { toast } from "sonner";
import { signIn } from "next-auth/react";
import { Button, Input, Label } from "@shared/ui";

type Credentials = {
	username: string;
	password: string;
};

type Errors = {
	loginErrors: Array<string> | null;
	passwordErrors: Array<string> | null;
};

export const Login = () => {
	const [credentials, setCredentials] = useState<Credentials>({ username: "", password: "" });
	const [errors, setErrors] = useState<Errors>({ loginErrors: null, passwordErrors: null });

	const handleLoginClick = async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();

		setErrors({ loginErrors: null, passwordErrors: null });

		const { username, password } = credentials;

		try {
			const signInResult = await signIn("credentials", {
				username,
				password,
				redirect: false
			});

			if (!signInResult!.ok) {
				const errors = JSON.parse(signInResult?.error!);
				if (errors["LoginCredentials.UserName"]) {
					setErrors((prevErrors) => ({
						...prevErrors,
						loginErrors: errors["LoginCredentials.UserName"]!
					}));
				}
				if (errors["LoginCredentials.Password"]) {
					setErrors((prevErrors) => ({
						...prevErrors,
						passwordErrors: errors["LoginCredentials.Password"]!
					}));
				}
			}

			if (signInResult!.ok) {
				// redirect("/dashboard");
				window.location.href = "/dashboard";
			}
		} catch (error) {
			console.error(error);
			toast.error("Unsuccessful login attempt");
		}
	};

	const handleUsernameChange: ChangeEventHandler<HTMLInputElement> = (event) => {
		setCredentials({ ...credentials, username: event.target.value });
	};

	const handlePasswordChange: ChangeEventHandler<HTMLInputElement> = (event) => {
		setCredentials({ ...credentials, password: event.target.value });
	};

	return (
		<article className="mx-auto grid w-[350px] gap-6">
			<div className="grid gap-2 text-center">
				<h2 className="text-3xl font-bold">Login</h2>
				<p className="text-balance text-muted-foreground">
					Enter your username and password below to login to your account
				</p>
			</div>
			<form onSubmit={handleLoginClick}>
				<div className="grid gap-4">
					<div className="grid gap-2">
						<Label htmlFor="username">Username</Label>
						<Input
							onChange={handleUsernameChange}
							id="username"
							type="text"
							placeholder="John"
							required
						/>
						{errors.loginErrors &&
							errors.loginErrors.map((error, index) => (
								<p key={"login" + index} className={"text-[0.8em] text-left text-red-600 mb-[1em]"}>
									{error}
								</p>
							))}
					</div>
					<div className="grid gap-2">
						<Label htmlFor="password">Password</Label>
						<Input onChange={handlePasswordChange} id="password" type="password" required />
						{errors.passwordErrors &&
							errors.passwordErrors.map((error, index) => (
								<p
									key={"password" + index}
									className={"text-[0.8em] text-left text-red-600 mb-[1em]"}
								>
									{error}
								</p>
							))}
					</div>
					<Button type="submit" className="w-full">
						Login
					</Button>
				</div>
			</form>
		</article>
	);
};
