export const sanitizeInput = (input: string) => {
	return input
		.replace(/[\0\n\r\b\t\\'"\x1a\$\%]/g, function (char) {
			switch (char) {
				case "\0":
				case "\n":
				case "\r":
				case "\b":
				case "\t":
				case "\x1a":
				case "$":
				case "%":
				case "'":
				case '"':
					return "";
				default:
					return "" + char;
			}
		})
		.trim();
};
