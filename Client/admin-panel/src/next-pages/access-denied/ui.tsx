"use client";

import { signOut } from "next-auth/react";
import { Ban } from "lucide-react";

import { Button } from "@shared/ui";

export const AccessDenied = () => {
	const handleSignOutClick = async () => {
		await signOut();
	};

	return (
		<section className={"flex flex-col items-center justify-center min-h-[100vh]"}>
			<div className={"flex flex-col items-center gap-y-[1em]"}>
				<Ban className={"w-[8em] h-[8em]"} />
				<h2 className={"text-3xl"}>Access Denied</h2>
				<Button className={"w-full"} onClick={handleSignOutClick}>
					Logout
				</Button>
			</div>
		</section>
	);
};
