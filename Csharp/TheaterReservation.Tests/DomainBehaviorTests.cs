using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheaterReservation.Application;
using TheaterReservation.Domain;

namespace TheaterReservation.Tests;

[TestClass]
public class DomainBehaviorTests
{
    [TestMethod]
    public void SeatAvailability_reserves_first_contiguous_matching_sequence()
    {
        var availability = new SeatAvailability(new[]
        {
            new[]
            {
                (SeatId.Create("A1"), SeatCategory.Standard, SeatStatus.Booked),
                (SeatId.Create("A2"), SeatCategory.Standard, SeatStatus.Free),
                (SeatId.Create("A3"), SeatCategory.Standard, SeatStatus.Free),
                (SeatId.Create("A4"), SeatCategory.Standard, SeatStatus.Free)
            }
        });

        var selected = availability.ReserveContiguous(2, SeatCategory.Standard);

        CollectionAssert.AreEqual(new[] { "A2", "A3" }, selected.Select(seat => seat.Id.Value).ToArray());
    }

    [TestMethod]
    public void Reservation_cancel_clears_seats_and_changes_status()
    {
        var reservation = DomainReservation.Create(
            ReservationId.Create(1),
            PerformanceId.Create(1),
            new[] { new SeatSelection(SeatId.Create("A1"), SeatCategory.Standard) });

        reservation.Cancel();

        Assert.AreEqual(ReservationStatus.Cancelled, reservation.Status);
        Assert.AreEqual(0, reservation.Seats.Count);
    }
}
