using LethalEmotesApi.Ui.Customize.DragDrop;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize;

[DisallowMultipleComponent]
[RequireComponent(typeof(EmoteDragDropController))]
public class CustomizePanel : MonoBehaviour
{
    public EmoteDragDropController? dragDropController;

    private void Awake()
    {
        if (dragDropController is null)
            dragDropController = GetComponent<EmoteDragDropController>();
    }
}