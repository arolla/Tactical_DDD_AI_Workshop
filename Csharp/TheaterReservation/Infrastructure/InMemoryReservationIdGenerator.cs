using TheaterReservation.Application;
using TheaterReservation.Domain;

namespace TheaterReservation.Infrastructure;

public sealed class InMemoryReservationIdGenerator : IReservationIdGenerator
{
    private long currentId;
    public InMemoryReservationIdGenerator(long initialId = 123_455) => currentId = initialId;
    public ReservationId Next() => ReservationId.Create(Interlocked.Increment(ref currentId));
}
