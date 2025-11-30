using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class Theme : BaseDataModel
{
    [Display(Name = "Theme Name")] public string ThemeName { get; set; } = "";

    [Display(Name = "Footer Color")] public string CFooter { get; set; } = "";

    [Display(Name = "Footer Font Color")] public string CFooterFont { get; set; } = "";

    [Display(Name = "Background Color")] public string CBackground { get; set; } = "";

    [Display(Name = "Card Color #1")] public string CCard1 { get; set; } = "";

    [Display(Name = "Card Color #2")] public string CCard2 { get; set; } = "";

    [Display(Name = "Progress Bar Color #1")] public string CProgressBar1 { get; set; } = "";

    [Display(Name = "Progress Bar Color #2")] public string CProgressBar2 { get; set; } = "";

    [Display(Name = "Progress Bar Orientation")] public ProgressBarType ProgressBarGradOrient { get; set; } = ProgressBarType.Horizontal;

    [Display(Name = "Font Color")] public string CFont { get; set; } = "";

    [Display(Name = "Running Indication Font Color")] public string CRunningIndicator { get; set; } = "";

    [Display(Name = "Button Color")] public string CButton { get; set; } = "";

    [Display(Name = "Button Font Color")] public string CButtonFont { get; set; } = "";

    [Display(Name = "Positive Button Color")] public string CButtonPositive { get; set; } = "";

    [Display(Name = "Positive Button Font Color")] public string CButtonPositiveFont { get; set; } = "";

    [Display(Name = "Negative Button Color")] public string CButtonNegative { get; set; } = "";

    [Display(Name = "Negative Button Font Color")] public string CButtonNegativeFont { get; set; } = "";

    [Display(Name = "Shadow Color")] public string CShadow { get; set; } = "";

    [Display(Name = "Transparency Effect Color")] public string CTransparency { get; set; } = "";
}