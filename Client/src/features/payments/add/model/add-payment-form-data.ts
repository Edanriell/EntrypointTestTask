import { z } from "zod";

import { addPaymentSchema } from "./schema";

export type AddPaymentFormData = z.infer<typeof addPaymentSchema>;
