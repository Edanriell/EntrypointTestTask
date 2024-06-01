namespace Server.DTO.Shared
{
    public class RestDTO<T> : RestBasicDTO<T>
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
