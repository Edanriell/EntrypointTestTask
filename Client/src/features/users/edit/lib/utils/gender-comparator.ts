import { Gender } from "@entities/users";

export const genderComparator = {
	gender: (apiGender: string, formGender: Gender) => {
		// Compare string gender with enum by converting enum to string
		return apiGender === Gender[formGender];
	}
};
