using TheaterReservation.Dao;
using TheaterReservation.Data;

namespace TheaterReservation;

public sealed class ReservationService
{
    private long currentId;
    private readonly ReservationDao reservations;

    public ReservationService(long initialId = 123_455, ReservationDao? reservations = null)
    {
        currentId = initialId;
        this.reservations = reservations ?? new ReservationDao();
    }

    public string initNewReservation() => Interlocked.Increment(ref currentId).ToString();

    public void updateReservation(Reservation reservation) => reservations.update(reservation);

    public Reservation findReservation(long reservationId) => reservations.find(reservationId);

    public void cancelReservation(long reservationId)
    {
        var reservation = reservations.find(reservationId);
        reservation.setStatus("CANCELLED");
        reservation.setSeats(Array.Empty<string>());
        reservations.update(reservation);
    }
}