using System.ComponentModel.DataAnnotations;

namespace smartoffice_web.WebApi.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}