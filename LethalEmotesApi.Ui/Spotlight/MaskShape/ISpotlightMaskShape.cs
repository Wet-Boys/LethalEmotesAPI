using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Spotlight.MaskShape;

public interface ISpotlightMaskShape
{
    float ScaleFactor { get; }
    Color MaskColor { get; }
    
    void PopulateMesh(in VertexHelper vh, ref int vertexOffset);
}