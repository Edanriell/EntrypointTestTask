import { create } from "zustand";

type Errors = {
	loginCredentials: Array<string> | null;
	passwordCredentials: Array<string> | null;
};

interface SessionState {
	errors: Errors;
	addLoginCredentialsErrors: (loginErrors: Array<string>) => void;
	addPasswordCredentialsErrors: (passwordErrors: Array<string>) => void;
	clearAllSessionErrors: () => void;
}

export const useSessionStore = create<SessionState>()((set) => ({
	errors: {
		loginCredentials: null,
		passwordCredentials: null
	},
	addLoginCredentialsErrors: (loginErrors) =>
		set((state) => ({ errors: { ...state.errors, loginCredentials: loginErrors } })),
	addPasswordCredentialsErrors: (passwordErrors) =>
		set((state) => ({ errors: { ...state.errors, passwordCredentials: passwordErrors } })),
	clearAllSessionErrors: () =>
		set((state) => ({ errors: { loginCredentials: null, passwordCredentials: null } }))
}));
