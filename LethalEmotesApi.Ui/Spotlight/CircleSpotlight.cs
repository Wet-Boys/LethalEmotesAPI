using LethalEmotesApi.Ui.Spotlight.MaskShape;

namespace LethalEmotesApi.Ui.Spotlight;

public class CircleSpotlight : SpotlightBase
{
    public float radius = 100f;

    public override ISpotlightMaskShape GetMaskShape(float scaleFactor) =>
        new CircleSpotlightShape
        {
            ScreenPos = ScreenPos,
            Radius = radius,
            ScaleFactor = scaleFactor,
            MaskColor = maskColor
        };
}