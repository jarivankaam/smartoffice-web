namespace smartoffice_web.WebApi.Models;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Gebruik GUID als ID
    public string IdentityUserId { get; set; } // Koppeling met Identity Framework user
    public string DisplayName { get; set; } // Extra info voor je app
}
