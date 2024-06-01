namespace Server.DTO.Shared
{
    public class BadRequestDTO
    {
        public Dictionary<string, List<string>> Errors { get; set; } =
            new Dictionary<string, List<string>>();
        public string Type { get; set; } = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
        public string Title { get; set; } = "One or more validation errors occurred.";
        public int Status { get; set; } = 400;
        public string TraceId { get; set; } = string.Empty;
    }
}
