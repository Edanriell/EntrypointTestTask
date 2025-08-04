import { z } from "zod";

import { discountProductSchema } from "./schema";

export type DiscountProductFormData = z.infer<typeof discountProductSchema>;
