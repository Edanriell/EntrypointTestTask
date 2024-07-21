import { FC } from "react";
import Link from "next/link";
import { signOut, useSession } from "next-auth/react";
import { useQuery } from "@tanstack/react-query";

import type { User } from "@entities/users/model";
import { getUserById } from "@entities/users/api";

import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger
} from "@shared/ui/dropdown";
import { Button } from "@shared/ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "@shared/ui/avatar";

type UserProfileLinks = Array<{
	linkName: string;
	linkUrl: string;
}>;

const userProfileLinks: UserProfileLinks = [
	{
		linkName: "Settings",
		linkUrl: "/settings"
	}
];

export const UserProfile: FC = () => {
	const { data: session, status } = useSession();
	const userId = session?.user.id;
	const accessToken = session?.accessToken;

	const {
		data: userData,
		error: userError,
		isPending: isUserPending,
		isError: isUserError
	} = useQuery({
		queryKey: ["getUserById", userId, accessToken],
		queryFn: (): Promise<User> => getUserById(accessToken!, userId),
		enabled: !!userId && !!accessToken
	});

	const handleSignOutClick = async () => await signOut();

	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button variant="secondary" size="icon" className="rounded-full">
					<Avatar className="h-9 w-9 sm:flex">
						<AvatarImage
							src={userData?.photo ? "data:image/jpeg;base64," + userData?.photo : "#"}
							alt={userData?.name + " " + userData?.surname + " " + "avatar"}
						/>
						<AvatarFallback>
							{userData?.name[0]}
							{userData?.surname[0]}
						</AvatarFallback>
					</Avatar>
					<span className="sr-only">Toggle user menu</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				<DropdownMenuLabel>My Account</DropdownMenuLabel>
				<DropdownMenuSeparator />
				{userProfileLinks.map(({ linkName, linkUrl }, index) => (
					<DropdownMenuItem key={index + "-" + linkName}>
						<Link href={linkUrl}>{linkName}</Link>
					</DropdownMenuItem>
				))}
				<DropdownMenuSeparator />
				<DropdownMenuItem>
					<button onClick={handleSignOutClick}>Logout</button>
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
};
