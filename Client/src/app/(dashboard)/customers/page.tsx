"use client";

import { CustomersPage } from "@views/clients";

import { withAuthGuard } from "@features/authentication/general";

const ProtectedCustomersPage = withAuthGuard(CustomersPage);

export default ProtectedCustomersPage;
