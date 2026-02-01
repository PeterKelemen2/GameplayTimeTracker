using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class Theme : BaseDataModel
{
    public string ThemeName { get; set; } = "Default Dark";
    public string CFooter { get; set; } = "#6A6F99";
    public string CFooterFont { get; set; } = "#DAE4FF";
    public string CBackground { get; set; } = "#1E2030";
    public string CCard1 { get; set; } = "#414769";
    public string CCard2 { get; set; } = "#2E324A";
    public string CProgressBar1 { get; set; } = "#89ACF2";
    public string CProgressBar2 { get; set; } = "#B7BDF8";
    public ProgressBarType ProgressBarGradOrient { get; set; } = ProgressBarType.Horizontal;
    public string CFont { get; set; } = "#DAE4FF";
    public string CRunningIndicator { get; set; } = "#C3E88D";
    public string CButton { get; set; } = "#3BC9E3";
    public string CButtonFont { get; set; } = "#115d6b";
    public string CButtonPositive { get; set; } = "#90EE90";
    public string CButtonPositiveFont { get; set; } = "#369936";
    public string CButtonNegative { get; set; } = "#ED0C0C";
    public string CButtonNegativeFont { get; set; } = "#7a0606";
    public string CShadow { get; set; } = "#151515";
    public string CTransparency { get; set; } = "#00000000";
}