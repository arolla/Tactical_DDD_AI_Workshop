using TheaterReservation.Domain;

namespace TheaterReservation.Application;

public sealed record PerformanceSnapshot(
    PerformanceId Id,
    string Play,
    DateTime StartTime,
    PerformanceNature Nature);

public sealed record SeatSelection(SeatId Id, SeatCategory Category);

public sealed record ReservationResult(
    ReservationId ReservationId,
    PerformanceSnapshot Performance,
    ReservationStatus Status,
    IReadOnlyList<SeatSelection> Seats,
    SeatCategory RequestedCategory,
    Money Total);

public interface IReservationIdGenerator
{
    ReservationId Next();
}

public interface ITheaterRoomRepository : ISeatAvailabilityRepository
{
}

public interface IPerformancePriceProvider
{
    Money GetBasePrice(PerformanceId performanceId);
}

public interface IVoucherPolicy
{
    decimal DiscountFor(DateTime date);
}

public interface ICustomerSubscriptionReader
{
    bool IsSubscribed(CustomerId customerId);
}

public interface IPerformanceCatalog
{
    PerformanceSnapshot Get(PerformanceId performanceId);
}

public interface ISeatAvailability
{
    int TotalSeats { get; }
    int RemainingSeats { get; }
    IReadOnlyList<SeatSelection> ReserveContiguous(int count, SeatCategory category);
    void Release(IReadOnlyCollection<SeatId> seats);
}

public interface ISeatAvailabilityRepository
{
    ISeatAvailability Get(PerformanceId performanceId);
    void Save(PerformanceId performanceId, ISeatAvailability availability);
}

public interface IReservationRepository
{
    void Save(DomainReservation reservation);
    DomainReservation Get(ReservationId reservationId);
}

public interface IPricePolicy
{
    Money PriceFor(PerformanceSnapshot performance, IReadOnlyList<SeatSelection> seats, CustomerId customerId);
}

public interface IReservationOutput
{
    string Render(ReservationResult result);
}

public sealed class DomainReservation
{
    private readonly List<SeatSelection> seats;
    public ReservationId Id { get; }
    public PerformanceId PerformanceId { get; }
    public ReservationStatus Status { get; private set; }
    public IReadOnlyList<SeatSelection> Seats => seats;

    private DomainReservation(ReservationId id, PerformanceId performanceId, IReadOnlyList<SeatSelection> seats, ReservationStatus status)
    {
        Id = id;
        PerformanceId = performanceId;
        this.seats = seats.ToList();
        Status = status;
    }

    public static DomainReservation Create(ReservationId id, PerformanceId performanceId, IReadOnlyList<SeatSelection> seats)
        => new(id, performanceId, seats, seats.Count > 0 ? ReservationStatus.Pending : ReservationStatus.Aborted);

    public void Cancel()
    {
        Status = ReservationStatus.Cancelled;
        seats.Clear();
    }
}
