using System.ComponentModel.DataAnnotations;

namespace Singula.Core.Services.Dto
{
    public class UserProfileUpdateDto
    {
        [StringLength(100)]
        public string? Username { get; set; }

        [StringLength(500)]
        public string? Biografia { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }
    }
}
