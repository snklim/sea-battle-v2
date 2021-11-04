using MediatR;

namespace SeaBattle.Web.Events
{
    public class InfoEvent : INotification

    {
    public string FromUser { get; set; }
    public string ToUser { get; set; }
    public string Message { get; set; }
    }
}