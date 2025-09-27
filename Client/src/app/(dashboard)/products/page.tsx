"use client";

import { ProductsPage } from "@views/products";

import { withAuthGuard } from "@features/authentication/general";

const ProtectedProductsPage = withAuthGuard(ProductsPage);

export default ProtectedProductsPage;
