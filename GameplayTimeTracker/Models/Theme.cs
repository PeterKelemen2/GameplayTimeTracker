using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class Theme : BaseDataModel
{
    public string ThemeName { get; set; } = "";
    public string CFooter { get; set; } = "";
    public string CFooterFont { get; set; } = "";
    public string CBackground { get; set; } = "";
    public string CCard1 { get; set; } = "";
    public string CCard2 { get; set; } = "";
    public string CProgressBar1 { get; set; } = "";
    public string CProgressBar2 { get; set; } = "";
    public ProgressBarType ProgressBarGradOrient { get; set; } = ProgressBarType.Horizontal;
    public string CFont { get; set; } = "";
    public string CRunningIndicator { get; set; } = "";
    public string CButton { get; set; } = "";
    public string CButtonFont { get; set; } = "";
    public string CButtonPositive { get; set; } = "";
    public string CButtonPositiveFont { get; set; } = "";
    public string CButtonNegative { get; set; } = "";
    [Display(Name = "Negative Button Font Color")] public string CButtonNegativeFont { get; set; } = "";
    [Display(Name = "Shadow Color")] public string CShadow { get; set; } = "";
    [Display(Name = "Transparency Effect Color")] public string CTransparency { get; set; } = "";
}