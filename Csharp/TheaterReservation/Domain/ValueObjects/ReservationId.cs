namespace TheaterReservation.Domain.ValueObjects;

public sealed class ReservationId : IEquatable<ReservationId>
{
    private readonly long _value;

    public ReservationId(long value)
    {
        if (value <= 0)
            throw new ArgumentException("Reservation ID must be positive.", nameof(value));

        _value = value;
    }

    public long Value => _value;

    public override string ToString() => _value.ToString();
    public override bool Equals(object? obj) => Equals(obj as ReservationId);
    public bool Equals(ReservationId? other) => other != null && _value == other._value;
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(ReservationId? left, ReservationId? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ReservationId? left, ReservationId? right)
    {
        return !Equals(left, right);
    }
}
