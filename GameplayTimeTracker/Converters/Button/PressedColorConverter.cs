namespace GameplayTimeTracker.Converters.Button;

public class PressedColorConverter : ColorBrightnessConverter
{
    public PressedColorConverter()
    {
        Factor = -0.2;
    }
}