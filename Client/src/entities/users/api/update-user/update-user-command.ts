import { Gender } from "../../model";

export type UpdateUserCommand = {
	userId: string;
	updatedUserData: UserData;
};

export type UserData = {
	firstName?: string | null;
	lastName?: string | null;
	email?: string | null;
	phoneNumber?: string | null;
	gender?: Gender | null;
	country?: string | null;
	city?: string | null;
	zipCode?: string | null;
	street?: string | null;
};
