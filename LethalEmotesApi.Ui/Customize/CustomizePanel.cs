using LethalEmotesApi.Ui.Customize.DragDrop;
using LethalEmotesApi.Ui.Customize.Preview;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize;

[DisallowMultipleComponent]
[RequireComponent(typeof(EmoteDragDropController))]
public class CustomizePanel : MonoBehaviour
{
    public EmoteDragDropController? dragDropController;
    public PreviewController? previewController;

    private void Awake()
    {
        if (dragDropController is null)
            dragDropController = GetComponent<EmoteDragDropController>();

        if (previewController is null)
            previewController = GetComponentInChildren<PreviewController>();
    }
}