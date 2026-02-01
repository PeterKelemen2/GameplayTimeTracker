using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.ViewModels;

public class ThemeViewModel : Theme
{
    public string ThemeNameLabel => "Theme Name";
    public string CFooterLabel => "Footer Color";
    public string CFooterFontLabel => "Footer Font Color";
    public string CBackgroundLabel => "Background Color";
    public string CCard1Label => "Card Color #1";
    public string CCard2Label => "Card Color #2";
    public string CProgressBar1Label => "Progress Bar Color #1";
    public string CProgressBar2Label => "Progress Bar Color #2";
    public string ProgressBarGradOrientLabel => "Progress Bar Gradient Orientation";
    public string CFontLabel => "Font Color";
    public string CRunningIndicatorLabel => "Running Indicator Font Color";
    public string CButtonLabel => "Button Color";
    public string CButtonFontLabel => "Button Font Color";
    public string CButtonPositiveLabel => "Positive Button Color";
    public string CButtonPositiveFontLabel => "Positive Button Font Color";
    public string CButtonNegativeLabel => "Negative Button Color";
    public string CButtonNegativeFontLabel => "Negative Button Font Color";
    public string CShadowLabel => "Shadow Color";
    public string CTransparencyLabel => "Transparency Effect Color";
}