import { FieldPath } from "react-hook-form";

import { EditUserFormData, Gender } from "../model";

export const EDIT_USER_FORM_FIELDS_MAPPING: Record<string, FieldPath<EditUserFormData>> = {
	FirstName: "firstName",
	LastName: "lastName",
	Email: "email",
	PhoneNumber: "phoneNumber",
	Gender: "gender",
	Country: "country",
	City: "city",
	ZipCode: "zipCode",
	Street: "street"
} as const;

export const GENDER_MAPPING: Record<string, Gender> = {
	Male: 0,
	Female: 1
} as const;
