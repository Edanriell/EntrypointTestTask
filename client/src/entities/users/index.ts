export * from "./ui";

// Types
export type {
	User,
	UserResponse,
	RegisterUserRequest,
	UpdateUserRequest,
	LoginUserRequest,
	AccessTokenResponse,
	UserListQuery,
	UserDetailQuery
} from "./model";

// Query factory
export { usersQueries } from "./api/users.query";

// Mutations
export { useCreateUser, useUpdateUser, useDeleteUser, useLoginUser } from "./api/mutations";

// API functions (if needed for direct usage)
export { getUsers, getUserById, getLoggedInUser, getClients } from "./api/get-users";
export { createUser } from "./api/create-user";
export { updateUser } from "./api/update-user";
export { deleteUser } from "./api/delete-user";
export { loginUser } from "./api/login-user";
