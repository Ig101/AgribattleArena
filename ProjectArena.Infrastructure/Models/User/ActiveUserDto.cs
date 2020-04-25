using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.User
{
    public class ActiveUserDto : UserDto
    {
        public string Email { get; set; }

        public UserState State { get; set; }
    }
}