using System.ComponentModel.DataAnnotations;

namespace Entities.ConfigurationModels;

public class UploadConfig
{
    [Required]
    public string UploadFilepath { get; set; }
    [MinLength(1, ErrorMessage = "Allowed Extensions most have at least one value.")]
    public string[] AllowedExtensions { get; set; }
}
