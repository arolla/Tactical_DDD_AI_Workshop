namespace TheaterReservation.Domain.ValueObjects;

public sealed class ReservationStatus : IEquatable<ReservationStatus>
{
    private readonly string _value;

    public static readonly ReservationStatus Pending = new("PENDING");
    public static readonly ReservationStatus Aborted = new("ABORTED");
    public static readonly ReservationStatus Cancelled = new("CANCELLED");
    public static readonly ReservationStatus Confirmed = new("CONFIRMED");

    private ReservationStatus(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static ReservationStatus From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Reservation status cannot be empty.", nameof(value));

        return value.ToUpper() switch
        {
            "PENDING" => Pending,
            "ABORTED" => Aborted,
            "CANCELLED" => Cancelled,
            "CONFIRMED" => Confirmed,
            _ => throw new ArgumentException($"Unknown reservation status: {value}", nameof(value))
        };
    }

    public bool IsPending => this == Pending;
    public bool IsAborted => this == Aborted;
    public bool IsCancelled => this == Cancelled;
    public bool IsConfirmed => this == Confirmed;
    public bool IsActive => IsPending || IsConfirmed;

    public override string ToString() => _value;
    public override bool Equals(object? obj) => Equals(obj as ReservationStatus);
    public bool Equals(ReservationStatus? other) => other != null && _value == other._value;
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(ReservationStatus? left, ReservationStatus? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ReservationStatus? left, ReservationStatus? right)
    {
        return !Equals(left, right);
    }
}
