using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker;

public enum ProgressBarType
{
    [Display(Name = "None")] None = 0,
    [Display(Name = "Horizontal")] Horizontal = 1,
    [Display(Name = "Vertical")] Vertical = 2,
}