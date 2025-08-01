﻿using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderCompletedDomainEvent(Guid OrderId, Guid ClientId) : IDomainEvent;
