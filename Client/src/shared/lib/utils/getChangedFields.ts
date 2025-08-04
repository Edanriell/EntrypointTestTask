export const getChangedFields = <TOriginal, TForm>(
	originalData: TOriginal,
	formData: TForm,
	fieldsToCheck: (keyof TForm)[],
	customComparisons?: Record<string, (original: any, form: any) => boolean>
): Partial<TForm> => {
	const changes = {} as Partial<TForm>;

	for (const field of fieldsToCheck) {
		const originalValue = originalData[field as keyof TOriginal];
		const formValue = formData[field];

		// Check if there's a custom comparison function for this field
		const customComparison = customComparisons?.[field as string];
		const areEqual = customComparison
			? customComparison(originalValue, formValue)
			: originalValue === formValue;

		if (!areEqual) {
			changes[field] = formValue;
		}
	}

	return changes;
};
