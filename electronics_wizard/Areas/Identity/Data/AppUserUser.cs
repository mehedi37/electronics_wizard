using Microsoft.AspNetCore.Identity;

namespace electronics_wizard.Areas.Identity.Data;

public class AppUserUser : IdentityUser
{
    [PersonalData]
    public required string Name { get; set; }
    [PersonalData]
    public string ProfilePicture { get; set; } = string.Empty;
}

