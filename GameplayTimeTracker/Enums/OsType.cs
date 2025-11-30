using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker;

public enum OsType
{
    [Display(Name = "Windows")] Windows = 0,
    [Display(Name = "Linux")] Linux = 1,
}