using User_lib;

namespace EversayApi.Dtos
{
    public class PublicUserDto
    {
        public string userId { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
        public UserType UserRole { get; set; }
    }
}
