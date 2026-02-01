namespace GameplayTimeTracker.Converters.Button;

public class HoverColorConverter : ColorBrightnessConverter
{
    public HoverColorConverter()
    {
        Factor = 0.2;
    }
}