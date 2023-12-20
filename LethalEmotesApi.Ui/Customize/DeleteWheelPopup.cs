using LethalEmotesApi.Ui.Customize.Wheel;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize;

public class DeleteWheelPopup : UIBehaviour
{
    public CustomizeWheelController? customizeWheelController;

    public void Cancel()
    {
        Destroy(gameObject);
    }

    public void Confirm()
    {
        if (customizeWheelController is null)
        {
            Cancel();
            return;
        }
        
        customizeWheelController.DeleteWheel();
    }
}