namespace Server.DTO.Shared
{
    public class RestBasicDTO<T>
    {
        public T Data { get; set; } = default!;
        public int? RecordCount { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
