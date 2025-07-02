import NextAuth from "next-auth";

import { authConfig } from "@features/authentication/general/config";

const handler = NextAuth(authConfig);

export { handler as GET, handler as POST };
