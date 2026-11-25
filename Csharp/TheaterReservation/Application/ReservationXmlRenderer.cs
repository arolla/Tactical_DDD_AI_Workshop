using System.Text;

namespace TheaterReservation.Application;

public sealed class ReservationXmlRenderer : IReservationOutput
{
    public string Render(ReservationResult result)
    {
        var sb = new StringBuilder();
        sb.Append("<reservation>\n");
        sb.Append("\t<performance>\n");
        sb.Append("\t\t<play>").Append(result.Performance.Play).Append("</play>\n");
        sb.Append("\t\t<date>").Append(result.Performance.StartTime).Append("</date>\n");
        sb.Append("\t</performance>\n");
        sb.Append("\t<reservationId>").Append(result.ReservationId).Append("</reservationId>\n");

        if (result.Seats.Count > 0)
        {
            sb.Append("\t<reservationStatus>FULFILLABLE</reservationStatus>\n");
            sb.Append("\t\t<seats>\n");
            foreach (var seat in result.Seats)
            {
                sb.Append("\t\t\t<seat>\n");
                sb.Append("\t\t\t\t<id>").Append(seat.Id).Append("</id>\n");
                sb.Append("\t\t\t\t<category>").Append(seat.Category).Append("</category>\n");
                sb.Append("\t\t\t</seat>\n");
            }
            sb.Append("\t\t</seats>\n");
        }
        else
        {
            sb.Append("\t<reservationStatus>ABORTED</reservationStatus>\n");
        }

        sb.Append("\t<seatCategory>").Append(result.RequestedCategory).Append("</seatCategory>\n");
        sb.Append("\t<totalAmountDue>").Append(result.Total).Append("\n".TrimEnd()).Append("</totalAmountDue>\n");
        sb.Append("</reservation>\n");
        return sb.ToString();
    }
}
