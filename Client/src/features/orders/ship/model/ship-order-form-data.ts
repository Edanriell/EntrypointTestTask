import { z } from "zod";

import { shipOrderSchema } from "./ship-order-schema";

export type ShipOrderFormData = z.infer<typeof shipOrderSchema>;
