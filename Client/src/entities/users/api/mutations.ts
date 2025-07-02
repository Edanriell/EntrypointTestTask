import { useMutation, UseMutationOptions, useQueryClient } from "@tanstack/react-query";
import { createUser } from "./create-user";
import { updateUser } from "./update-user";
import { deleteUser } from "./delete-user";
import { RegisterUserRequest, UpdateUserRequest } from "../model";
import { usersQueries } from "./users.query";

export const useCreateUser = (options?: UseMutationOptions<string, Error, RegisterUserRequest>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createUser,
		onSuccess: () => {
			// Invalidate and refetch users list
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
			queryClient.invalidateQueries({ queryKey: usersQueries.clients() });
		},
		...options
	});
};

export const useUpdateUser = (options?: UseMutationOptions<void, Error, UpdateUserRequest>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: updateUser,
		onSuccess: (_, variables) => {
			// Invalidate and refetch users list and specific user
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
			queryClient.invalidateQueries({ queryKey: usersQueries.clients() });
			queryClient.invalidateQueries({
				queryKey: usersQueries.details()
			});
			queryClient.invalidateQueries({
				queryKey: usersQueries.me().queryKey
			});
		},
		...options
	});
};

export const useDeleteUser = (options?: UseMutationOptions<void, Error, string>) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteUser,
		onSuccess: (_, userId) => {
			// Invalidate and refetch users list
			queryClient.invalidateQueries({ queryKey: usersQueries.lists() });
			queryClient.invalidateQueries({ queryKey: usersQueries.clients() });
			// Remove the specific user from cache
			queryClient.removeQueries({
				queryKey: [...usersQueries.details(), userId]
			});
		},
		...options
	});
};

// /**/
// import { useCreateUser } from '@entities/users';
// import { toast } from 'sonner';
//
// export const UserManagement = () => {
//   const { mutate: createUser, isPending, error, reset } = useCreateUser({
//     // Custom success handler
//     onSuccess: (userId: string, variables: { firstName: string }) => {
//       toast.success(`User ${variables.firstName} created successfully!`);
//       // Additional logic...
//     },
//
//     // Custom error handler
//     onError: (error: Error, variables: { firstName: string }) => {
//       toast.error(`Failed to create user ${variables.firstName}: ${error.message}`);
//     },
//
//     // Custom settings handler
//     onSettled: (
//       data: unknown,
//       error: Error | null,
//       variables: { email: string }
//     ) => {
//       // This runs after both success and error
//       console.log('Mutation completed for:', variables.email);
//     },
//
//     // Retry configuration
//     retry: 2,
//     retryDelay: 1000,
//   });
//
//   // Reset error state when needed
//   const handleReset = () => {
//     reset();
//   };
//
//   return (
//     <div>
//       {/* Your form */}
//       {error && (
//         <div className="error">
//           <p>Error: {error.message}</p>
//           <button onClick={handleReset}>Try Again</button>
//         </div>
//       )}
//     </div>
//   );
// };
