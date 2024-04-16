using UnityEngine;

namespace LethalEmotesApi.Ui.Elements.Recycle;

public interface IRecycleViewItem<in TData>
{
    public int ConstraintIndex { get; set; }
    
    public RectTransform RectTransform { get; }

    public void BindData(TData data);
}