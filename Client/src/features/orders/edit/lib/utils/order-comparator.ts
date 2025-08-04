export const orderComparator = {
	street: (normalizedData: string, formValue: string): boolean => {
		return normalizedData === formValue;
	},
	city: (normalizedData: string, formValue: string): boolean => {
		return normalizedData === formValue;
	},
	country: (normalizedData: string, formValue: string): boolean => {
		return normalizedData === formValue;
	},
	zipCode: (normalizedData: string, formValue: string): boolean => {
		return normalizedData === formValue;
	},
	info: (normalizedData: string, formValue: string): boolean => {
		return normalizedData === formValue;
	}
};
