using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker;

public enum DisplayType
{
    [Display(Name = "Horizontal")] Horizontal = 0,
    [Display(Name = "Vertical")] Vertical = 1,
    [Display(Name = "Compact")] Compact = 2,
}