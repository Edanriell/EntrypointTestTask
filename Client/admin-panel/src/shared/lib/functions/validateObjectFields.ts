type validateObjectFieldsParameters = {
	object: any;
	requiredFields: Array<string>;
};

export const validateObjectFields = ({
	object,
	requiredFields
}: validateObjectFieldsParameters) => {
	return requiredFields.every((field) => field in object);
};
