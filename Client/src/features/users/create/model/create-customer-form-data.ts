import { z } from "zod";

import { createCustomerSchema } from "./schemas";

export type CreateCustomerFormData = z.infer<typeof createCustomerSchema>;
