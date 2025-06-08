import type { User } from "next-auth";

import type { Session as SessionModel, User as UserModel } from "./model";

type UserId = string;

// declare module "next-auth/jwt" {
// 	interface JWT {
// 		id: UserId;
// 		test: string;
// 	}
// }

declare module "next-auth" {
	interface Session extends SessionModel {
		user: User & UserModel;
	}
}
