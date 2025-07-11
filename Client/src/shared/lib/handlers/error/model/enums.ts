export enum ErrorType {
	VALIDATION = "ValidationFailure",
	AUTHENTICATION = "AuthenticationFailure",
	AUTHORIZATION = "AuthorizationFailure",
	NOT_FOUND = "NotFound",
	CONFLICT = "Conflict",
	SERVER_ERROR = "ServerError",
	NETWORK = "NetworkError",
	INVALID_CREDENTIALS = "User.InvalidCredentials"
}
