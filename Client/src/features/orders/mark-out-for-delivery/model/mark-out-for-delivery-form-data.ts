import { z } from "zod";

import { markOutForDeliverySchema } from "./schema";

export type MarkOutForDeliveryFormData = z.infer<typeof markOutForDeliverySchema>;
