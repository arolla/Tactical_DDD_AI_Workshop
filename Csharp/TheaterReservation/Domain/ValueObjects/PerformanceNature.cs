namespace TheaterReservation.Domain.ValueObjects;

public sealed class PerformanceNature : IEquatable<PerformanceNature>
{
    private readonly string _value;

    public static readonly PerformanceNature Premiere = new("PREMIERE");
    public static readonly PerformanceNature Preview = new("PREVIEW");
    public static readonly PerformanceNature Regular = new("REGULAR");

    private PerformanceNature(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static PerformanceNature From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Performance nature cannot be empty.", nameof(value));

        return value.ToUpper() switch
        {
            "PREMIERE" => Premiere,
            "PREVIEW" => Preview,
            "REGULAR" => Regular,
            _ => throw new ArgumentException($"Unknown performance nature: {value}", nameof(value))
        };
    }

    public bool IsPremiere => this == Premiere;
    public bool IsPreview => this == Preview;
    public bool IsRegular => this == Regular;

    public override string ToString() => _value;
    public override bool Equals(object? obj) => Equals(obj as PerformanceNature);
    public bool Equals(PerformanceNature? other) => other != null && _value == other._value;
    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(PerformanceNature? left, PerformanceNature? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(PerformanceNature? left, PerformanceNature? right)
    {
        return !Equals(left, right);
    }
}
