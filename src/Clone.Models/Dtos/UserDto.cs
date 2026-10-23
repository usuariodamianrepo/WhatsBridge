using System.ComponentModel.DataAnnotations;

namespace Clone.Models.Dtos
{
    public class UserDto
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public UserDto(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
