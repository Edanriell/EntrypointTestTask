export const formatNumberWithSeparators = (number: number, separateAfter: number) => {
	let formattedNumber = "";
	const numberString = String(number);

	// @ts-ignore
	for (const [index, character] of [...numberString].entries()) {
		const totalStringLength = numberString.length;

		formattedNumber += character;

		if ((totalStringLength - index - 1) % separateAfter === 0 && index !== totalStringLength - 1)
			formattedNumber += ",";
	}

	return formattedNumber;
};
