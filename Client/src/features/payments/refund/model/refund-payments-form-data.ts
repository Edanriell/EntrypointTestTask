import { z } from "zod";

import { refundPaymentsSchema } from "./schema";

export type RefundPaymentsFormData = z.infer<typeof refundPaymentsSchema>;
