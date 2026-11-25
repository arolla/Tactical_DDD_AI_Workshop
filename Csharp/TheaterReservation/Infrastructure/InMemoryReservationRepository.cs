using TheaterReservation.Application;
using TheaterReservation.Domain;

namespace TheaterReservation.Infrastructure;

public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly Dictionary<ReservationId, DomainReservation> reservations = new();
    public void Save(DomainReservation reservation) => reservations[reservation.Id] = reservation;
    public DomainReservation Get(ReservationId reservationId) => reservations.TryGetValue(reservationId, out var reservation)
        ? reservation
        : throw new KeyNotFoundException($"Reservation {reservationId} was not found.");
}
