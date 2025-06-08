import { Order } from "@entities/orders";

export enum Gender {
	Male,
	Female
}

export type User = {
	id: string;
	name: string;
	surname: string;
	userName: string;
	email: string;
	phoneNumber: string | null;
	address: string;
	birthDate: Date;
	gender: Gender;
	photo?: string | null;
	createdAt: Date;
	orders?: Order[] | null;
};
