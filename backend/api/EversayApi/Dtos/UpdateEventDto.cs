namespace EversayApi.Dtos
{
    public class UpdateEventDto
    {
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Duration { get; set; }
        public string? EventLocation { get; set; }
        public string? EventImage { get; set; }
        public string? EventPreview { get; set; }
        public string? EventCategory { get; set; }
    }
}
