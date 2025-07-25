﻿using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderShippedDomainEvent(Guid OrderId) : IDomainEvent;
