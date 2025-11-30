using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class SettingsProfile : BaseDataModel
{
    public int SettingsId { get; set; }

    [Display(Name = "Profile Name")] public string ProfileName { get; set; } = "";
}