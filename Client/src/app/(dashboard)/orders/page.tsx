"use client";

import { OrdersPage } from "@views/orders";

import { withAuthGuard } from "@features/authentication/general";

const ProtectedOrdersPage = withAuthGuard(OrdersPage);

export default ProtectedOrdersPage;
