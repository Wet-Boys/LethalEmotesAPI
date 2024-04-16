using LethalEmotesApi.Ui.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SegmentLabel : UIBehaviour
{
    private readonly TweenRunner<Vector3Tween> _scaleTweenRunner = new();

    public RectTransform? targetLabel;
    public TextMeshProUGUI? targetText;
    public RectTransform? missingLabel;
    public TextMeshProUGUI? missingText;
    public RectTransform? lockedLabel;
    public TextMeshProUGUI? lockedText;

    private RectTransform? _rectTransform;
    private string? _emoteKey;

    public RectTransform RectTransform
    {
        get
        {
            _rectTransform ??= GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    protected DrivenRectTransformTracker tracker;

    protected SegmentLabel()
    {
        _scaleTweenRunner.Init(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (targetLabel is null || missingLabel is null)
            return;

        tracker.Add(this, targetLabel, DrivenTransformProperties.Rotation);
        tracker.Add(this, missingLabel, DrivenTransformProperties.Rotation);
        tracker.Add(this, lockedLabel, DrivenTransformProperties.Rotation);


        var worldRot = RectTransform.eulerAngles;
        targetLabel.localEulerAngles = -worldRot;
        missingLabel.localEulerAngles = -worldRot;
        lockedLabel.localEulerAngles = -worldRot;

        UpdateText();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        tracker.Clear();
    }

    public void SetEmote(string? emoteKey)
    {
        _emoteKey = emoteKey;
        UpdateText();
    }

    private void UpdateText()
    {
        if (targetText is null || missingLabel is null || missingText is null || _emoteKey is null)
            return;

        var emoteName = EmoteUiManager.GetEmoteName(_emoteKey);

        lockedLabel.gameObject.SetActive(false);
        missingLabel.gameObject.SetActive(false);
        if (!EmoteUiManager.EmoteDb.EmoteExists(_emoteKey))
        {
            targetText.SetText("");

            var modName = EmoteUiManager.GetEmoteModName(_emoteKey);

            missingText.SetText($"<color=#FFFFFF>{emoteName}</color>\n<color=#eed202>Requires</color>\n<color=#FFFFFF>{modName}</color>");

            missingLabel.gameObject.SetActive(true);
        }
        else
        {
            if (!EmoteUiManager.GetEmoteVisibility(_emoteKey))
            {
                targetText.SetText("");
                lockedText.SetText($"<color=#eed202>Unlock</color>\n{emoteName}\n<color=#eed202>With</color>\nTooManyEmotes");
                lockedLabel.gameObject.SetActive(true);
            }
            else
            {
                targetText.SetText(emoteName);
            }
        }
    }

    public void TweenScale(Vector3 targetScale, float duration, bool ignoreTimeScale)
    {
        if (transform.localScale == targetScale)
        {
            _scaleTweenRunner.StopTween();
            return;
        }

        Vector3Tween tween = new Vector3Tween
        {
            Duration = duration,
            StartValue = transform.localScale,
            TargetValue = targetScale,
            IgnoreTimeScale = ignoreTimeScale
        };

        tween.AddOnChangedCallback(TweenScaleChanged);

        _scaleTweenRunner.StartTween(tween);
    }

    private void TweenScaleChanged(Vector3 scale)
    {
        transform.localScale = scale;
    }
}