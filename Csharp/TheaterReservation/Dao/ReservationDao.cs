using TheaterReservation.Data;

namespace TheaterReservation.Dao;

public class ReservationDao
{

    private readonly Dictionary<Int64, Reservation> reservationMap = new Dictionary<Int64, Reservation>();
    public void update(Reservation reservation)
    {
        if (reservationMap.ContainsKey(reservation.getReservationId()))
        {
            reservationMap.Remove(reservation.getReservationId());
        }
        reservationMap.Add(reservation.getReservationId(), reservation);
    }

    public Reservation find(Int64 reservationId)
    {
        if (!reservationMap.TryGetValue(reservationId, out var reservation))
        {
            throw new KeyNotFoundException($"Reservation {reservationId} was not found.");
        }

        return reservation;
    }
}