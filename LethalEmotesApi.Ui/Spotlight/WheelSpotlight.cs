using LethalEmotesApi.Ui.Spotlight.MaskShape;

namespace LethalEmotesApi.Ui.Spotlight;

public class WheelSpotlight : SpotlightBase
{
    public float minRadius = 100f;
    public float maxRadius = 400f;

    public override ISpotlightMaskShape GetMaskShape(float scaleFactor) =>
        new WheelSpotlightShape
        {
            ScreenPos = ScreenPos,
            MinRadius = minRadius,
            MaxRadius = maxRadius,
            ScaleFactor = scaleFactor,
            MaskColor = maskColor
        };
}