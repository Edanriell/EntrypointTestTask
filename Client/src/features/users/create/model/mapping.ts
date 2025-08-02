import { FieldPath } from "react-hook-form";

import { CreateCustomerFormData } from "./create-customer-form-data";

export const CREATE_CUSTOMER_FORM_FIELD_MAPPING: Record<
	string,
	FieldPath<CreateCustomerFormData>
> = {
	FirstName: "firstName",
	LastName: "lastName",
	Email: "email",
	PhoneNumber: "phoneNumber",
	Gender: "gender",
	Country: "country",
	City: "city",
	ZipCode: "zipCode",
	Street: "street",
	Password: "password"
} as const;
