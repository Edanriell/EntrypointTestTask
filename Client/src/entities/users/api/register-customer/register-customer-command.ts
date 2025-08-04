import { Gender } from "../../model";

export type RegisterCustomerCommand = {
	firstName: string;
	lastName: string;
	email: string;
	phoneNumber: string;
	gender: Gender;
	country: string;
	city: string;
	zipCode: string;
	street: string;
	password: string;
};
