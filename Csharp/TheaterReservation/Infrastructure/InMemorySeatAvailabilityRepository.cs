using TheaterReservation.Application;
using TheaterReservation.Dao;
using TheaterReservation.Data;
using TheaterReservation.Domain;

namespace TheaterReservation.Infrastructure;

public sealed class InMemorySeatAvailabilityRepository : ITheaterRoomRepository
{
    private readonly TheaterRoomDao roomDao = new();
    private readonly Dictionary<PerformanceId, ISeatAvailability> cache = new();

    public ISeatAvailability Get(PerformanceId performanceId)
    {
        if (cache.TryGetValue(performanceId, out var cached)) return cached;
        var room = roomDao.fetchTheaterRoom(performanceId.Value);
        var rows = room.getZones().SelectMany(zone => zone.getRows().Select(row =>
            row.getSeats().Select(seat =>
                (SeatId.Create(seat.getSeatId()),
                 DomainParsing.ParseCategory(zone.getCategory()),
                 ParseStatus(seat.getStatus())))));
        return cache[performanceId] = new SeatAvailability(rows);
    }

    public void Save(PerformanceId performanceId, ISeatAvailability availability) => cache[performanceId] = availability;

    private static SeatStatus ParseStatus(string status) => status?.ToUpperInvariant() switch
    {
        "BOOKED" => SeatStatus.Booked,
        "BOOKING_PENDING" => SeatStatus.BookingPending,
        _ => SeatStatus.Free
    };
}
