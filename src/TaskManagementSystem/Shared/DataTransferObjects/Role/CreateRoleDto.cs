using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Role;

public record class CreateRoleDto
{
    [Required(ErrorMessage = "Role Name is required.")]
    public string Name { get; set; }
}
