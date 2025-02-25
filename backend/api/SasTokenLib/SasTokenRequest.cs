namespace SasTokenLib
{
    public class SasTokenRequest
    {
        public string? BlobName { get; set; }
        public string Permission { get; set; } = "r";
        public int ExpiryMinutes { get; set; } = 60;
    }
}
