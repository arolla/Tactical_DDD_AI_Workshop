using TheaterReservation.Domain;

namespace TheaterReservation.Application;

public sealed class ReservationApplicationService
{
    private readonly IReservationIdGenerator ids;
    private readonly ISeatAvailabilityRepository availabilityRepository;
    private readonly IReservationRepository reservations;
    private readonly IPricePolicy prices;

    public ReservationApplicationService(
        IReservationIdGenerator ids,
        ISeatAvailabilityRepository availabilityRepository,
        IReservationRepository reservations,
        IPricePolicy prices)
    {
        this.ids = ids;
        this.availabilityRepository = availabilityRepository;
        this.reservations = reservations;
        this.prices = prices;
    }

    public ReservationResult Reserve(CustomerId customerId, PerformanceSnapshot performance, int count, SeatCategory category)
    {
        var availability = availabilityRepository.Get(performance.Id);
        var selected = availability.ReserveContiguous(count, category);
        var reservation = DomainReservation.Create(ids.Next(), performance.Id, selected);
        reservations.Save(reservation);

        var threshold = performance.Nature switch
        {
            PerformanceNature.Premiere => 0.5m,
            PerformanceNature.Preview => 0.9m,
            _ => 0m
        };
        if (threshold > 0 && availability.RemainingSeats < availability.TotalSeats * threshold)
            selected = Array.Empty<SeatSelection>();

        availabilityRepository.Save(performance.Id, availability);
        return new ReservationResult(reservation.Id, performance, reservation.Status, selected, category, prices.PriceFor(performance, selected, customerId));
    }

    public void Cancel(ReservationId reservationId)
    {
        var reservation = reservations.Get(reservationId);
        var availability = availabilityRepository.Get(reservation.PerformanceId);
        availability.Release(reservation.Seats.Select(seat => seat.Id).ToArray());
        reservation.Cancel();
        availabilityRepository.Save(reservation.PerformanceId, availability);
        reservations.Save(reservation);
    }
}
