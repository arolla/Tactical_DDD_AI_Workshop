namespace TheaterReservation.Domain.DomainServices;

using TheaterReservation.Domain.ValueObjects;

public interface IReservationIdAllocator
{
    ReservationId AllocateNewId();
}

public class SequentialReservationIdAllocator : IReservationIdAllocator
{
    private long _currentId;
    private readonly object _lock = new object();

    public SequentialReservationIdAllocator(long startingId = 123455)
    {
        if (startingId <= 0)
            throw new ArgumentException("Starting ID must be positive.", nameof(startingId));

        _currentId = startingId;
    }

    public ReservationId AllocateNewId()
    {
        lock (_lock)
        {
            _currentId++;
            return new ReservationId(_currentId);
        }
    }
}
