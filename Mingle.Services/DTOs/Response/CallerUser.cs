namespace Mingle.Services.DTOs.Response
{
    public class CallerUser
    {
        public required string DisplayName { get; init; }

        public required Uri ProfilePhoto { get; init; }
    }
}