namespace TheaterReservation.Domain.ValueObjects;

public sealed class SeatStatus : IEquatable<SeatStatus>
{
    private readonly string _value;

    public static readonly SeatStatus Free = new("FREE");
    public static readonly SeatStatus Booked = new("BOOKED");
    public static readonly SeatStatus BookingPending = new("BOOKING_PENDING");

    private SeatStatus(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static SeatStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seat status cannot be empty.", nameof(value));

        return value.ToUpper() switch
        {
            "FREE" => Free,
            "BOOKED" => Booked,
            "BOOKING_PENDING" => BookingPending,
            _ => throw new ArgumentException($"Unknown seat status: {value}", nameof(value))
        };
    }

    public bool IsFree => this == Free;
    public bool IsBooked => this == Booked;
    public bool IsBookingPending => this == BookingPending;
    public bool IsAvailable => IsFree || (!IsBooked && !IsBookingPending);

    public override string ToString() => _value;
    public override bool Equals(object? obj) => Equals(obj as SeatStatus);
    public bool Equals(SeatStatus? other) => other != null && _value == other._value;
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(SeatStatus? left, SeatStatus? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SeatStatus? left, SeatStatus? right)
    {
        return !Equals(left, right);
    }
}
