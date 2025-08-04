import { EditUserFormData } from "../model";

export const USER_UPDATABLE_FIELDS: (keyof EditUserFormData)[] = [
	"firstName",
	"lastName",
	"email",
	"phoneNumber",
	"gender",
	"country",
	"city",
	"zipCode",
	"street"
];
