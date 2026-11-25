using TheaterReservation.Application;
using TheaterReservation.Data;
using TheaterReservation.Domain;
using TheaterReservation.Infrastructure;

namespace TheaterReservation;

public sealed class TheaterService
{
    private readonly ReservationApplicationService application;
    private readonly IReservationOutput output;
    private readonly ISeatAvailabilityRepository availabilityRepository;

    public TheaterService(
        IReservationIdGenerator? ids = null,
        ISeatAvailabilityRepository? availability = null,
        IReservationRepository? reservations = null,
        IPricePolicy? prices = null,
        IReservationOutput? output = null)
    {
        availabilityRepository = availability ?? new InMemorySeatAvailabilityRepository();
        application = new ReservationApplicationService(
            ids ?? new InMemoryReservationIdGenerator(),
            availabilityRepository,
            reservations ?? new InMemoryReservationRepository(),
            prices ?? new LegacyPricePolicy());
        this.output = output ?? new ReservationXmlRenderer();
    }

    public string reservation(long customerId, int reservationCount, string reservationCategory, Performance performance)
    {
        ArgumentNullException.ThrowIfNull(performance);
        var snapshot = new PerformanceSnapshot(
            PerformanceId.Create(performance.id),
            performance.play,
            performance.startTime,
            DomainParsing.ParseNature(performance.performanceNature));
        var result = application.Reserve(
            CustomerId.Create(customerId),
            snapshot,
            reservationCount,
            DomainParsing.ParseCategory(reservationCategory));
        return output.Render(result);
    }

    public void cancelReservation(string reservationId, long performanceId, List<string> seats)
    {
        if (long.TryParse(reservationId, out var parsedId))
        {
            try
            {
                application.Cancel(ReservationId.Create(parsedId));
                return;
            }
            catch (KeyNotFoundException)
            {
                // The legacy API does not provide a reservation instance. Fall through to the seat release path.
            }
        }

        var availability = availabilityRepository.Get(PerformanceId.Create(performanceId));
        availability.Release(seats.Select(SeatId.Create).ToArray());
        availabilityRepository.Save(PerformanceId.Create(performanceId), availability);
    }
}