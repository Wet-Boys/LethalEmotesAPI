using System;
using System.Collections.Generic;
using LethalEmotesApi.Ui.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Elements.Recycle;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public abstract class RecycleListView<TRecycleItem, TData> : UIBehaviour
    where TRecycleItem : Component, IRecycleViewItem<TData>
{
    public ScrollRect? scrollRect;
    public GameObject? listItemPrefab;
    public RectOffset padding = new();
    public float spacing;
    public Vector2 preferredItemSize;
    public GridLayoutGroup.Corner startCorner;
    public Constraint constraint;
    public int maxInConstraint;
    
    public int minPoolSize = 10;
    public float requiredCoverageMultiplier = 1.5f;
    public float recyclingThreshold = 0.2f;
    
    protected abstract IList<TData> ListData { get; }

    private RectTransform? _recycleViewTransform;
    private RectTransform? _viewport;
    private RectTransform? _content;
    private Bounds _viewBounds;
    private Vector2 _prevAnchoredPos;
    private bool _recycling;

    private readonly RecyclePool<TRecycleItem, TData> _pool = [];
    private DrivenRectTransformTracker _tracker;
    private DrivenRectTransformTracker _poolTracker;

    protected override void Awake()
    {   
        base.Awake();

        _recycleViewTransform = GetComponent<RectTransform>();

        if (scrollRect is null)
            return;

        _viewport = scrollRect.viewport;
        _content = scrollRect.content;

        if (_viewport is null || _content is null)
            return;

        _tracker.Add(this, _content,
            DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax |
            DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot |
            DrivenTransformProperties.SizeDelta);
        
        scrollRect.onValueChanged.AddListener(HandleScroll);
    }

    protected override void Start()
    {
        base.Start();
        
        Canvas.ForceUpdateCanvases();
        
        UpdateState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnValidate()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        UpdateContentSize();
        UpdateViewBounds();
        SetupPool();
    }

    private void UpdateContentSize()
    {
        if (_content is null)
            return;
        
        var maxItems = Mathf.CeilToInt(ListData.Count / (float)maxInConstraint);
        var minorAxisItemSize = constraint is Constraint.Columns ? preferredItemSize.y : preferredItemSize.x;
        var axisPadding = constraint is Constraint.Columns
            ? padding.top + padding.bottom
            : padding.left + padding.right;
        
        var contentSize = axisPadding + (spacing * (maxItems - 1) + minorAxisItemSize * maxItems);

        _content.sizeDelta = constraint is Constraint.Columns
            ? new Vector2(_content.sizeDelta.x, contentSize)
            : new Vector2(contentSize, _content.sizeDelta.y);
    }

    private void UpdateViewBounds()
    {
        if (_recycleViewTransform is null || _viewport is null)
            return;
        
        var viewportRect = _recycleViewTransform.GetRelativeRect(_viewport);

        var widthThreshold = (viewportRect.width / 2f) * recyclingThreshold;
        var heightThreshold = (viewportRect.height / 2f) * recyclingThreshold;

        _viewBounds.min = new Vector3(viewportRect.xMin - widthThreshold, viewportRect.yMin - heightThreshold);
        _viewBounds.max = new Vector3(viewportRect.xMax + widthThreshold, viewportRect.yMax + heightThreshold);
    }

    private void SetupPool()
    {
        if (listItemPrefab is null || _viewport is null || _content is null)
            return;
        
        _poolTracker.Clear();
        _pool.Clear();

        if (ListData.Count <= 0)
            return;
        
        var axisSize = constraint is Constraint.Columns ? _viewport.rect.width : _viewport.rect.height;
        var requiredCoverage = requiredCoverageMultiplier * axisSize;
        
        float posX = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
            ? padding.left
            : -padding.right;
        float posY = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
            ? -padding.bottom
            : -padding.top;
        
        var axisPadding = constraint is Constraint.Columns
            ? padding.left + padding.right
            : padding.top + padding.bottom;

        var maxAxisSize = (axisSize - axisPadding) / maxInConstraint - spacing * (maxInConstraint - 1);

        var clampedItemSize = constraint is Constraint.Columns
            ? new Vector2(Mathf.Min(preferredItemSize.x, maxAxisSize), preferredItemSize.y)
            : new Vector2(preferredItemSize.x, Mathf.Min(preferredItemSize.y, maxAxisSize));

        var pivotX = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
            ? 0f
            : 1f;
        var pivotY = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
            ? 0f
            : 1f;
        var pivot = new Vector2(pivotX, pivotY);
        
        float currentCoverage = 0;
        var itemsInConstraint = 0;

        while ((_pool.Size < minPoolSize || currentCoverage < requiredCoverage) && _pool.Size < ListData.Count)
        {
            var item = Instantiate(listItemPrefab, _content).GetComponent<TRecycleItem>();
            item.gameObject.name = $"RecycleViewListItem {_pool.Size + 1}";

            OnInstantiateListItem(item);

            item.ConstraintIndex = itemsInConstraint;

            var itemTransform = item.RectTransform;
            itemTransform.anchoredPosition = new Vector2(posX, posY);
            itemTransform.anchorMin = pivot;
            itemTransform.anchorMax = pivot;
            itemTransform.pivot = pivot;
            itemTransform.sizeDelta = clampedItemSize;

            var rect = item.RectTransform.rect;
            itemsInConstraint++;
            
            switch (constraint)
            {
                case Constraint.Columns:
                    switch (startCorner)
                    {
                        case GridLayoutGroup.Corner.UpperLeft:
                            posX = itemTransform.anchoredPosition.x + rect.width + spacing;
                            
                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = padding.left;
                                posY = itemTransform.anchoredPosition.y - (rect.height + spacing);
                            }
                            break;
                        case GridLayoutGroup.Corner.UpperRight:
                            posX = itemTransform.anchoredPosition.x - (rect.width + spacing);
                            
                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = -padding.right;
                                posY = itemTransform.anchoredPosition.y - (rect.height + spacing);
                            }
                            break;
                        case GridLayoutGroup.Corner.LowerLeft:
                            posX = itemTransform.anchoredPosition.x + rect.width + spacing;
                            
                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = padding.left;
                                posY = itemTransform.anchoredPosition.y + rect.height + spacing;
                            }
                            break;
                        case GridLayoutGroup.Corner.LowerRight:
                            posX = itemTransform.anchoredPosition.x - (rect.width + spacing);
                            
                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = -padding.right;
                                posY = itemTransform.anchoredPosition.y + rect.height + spacing;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (itemsInConstraint >= maxInConstraint)
                    {
                        itemsInConstraint = 0;
                        currentCoverage += rect.height + spacing;
                    }

                    break;
                case Constraint.Rows:
                    switch (startCorner)
                    {
                        case GridLayoutGroup.Corner.UpperLeft:
                            posY = itemTransform.anchoredPosition.y - (rect.height + spacing);

                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = itemTransform.anchoredPosition.x + rect.width + spacing;
                                posY = -padding.top;
                            }
                            break;
                        case GridLayoutGroup.Corner.UpperRight:
                            posY = itemTransform.anchoredPosition.y - (rect.height + spacing);

                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = itemTransform.anchoredPosition.x - (rect.width + spacing);
                                posY = -padding.top;
                            }
                            break;
                        case GridLayoutGroup.Corner.LowerLeft:
                            posY = itemTransform.anchoredPosition.y + rect.height + spacing;

                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = itemTransform.anchoredPosition.x + rect.width + spacing;
                                posY = padding.bottom;
                            }
                            break;
                        case GridLayoutGroup.Corner.LowerRight:
                            posY = itemTransform.anchoredPosition.y + rect.height + spacing;

                            if (itemsInConstraint >= maxInConstraint)
                            {
                                posX = itemTransform.anchoredPosition.x - (rect.width + spacing);
                                posY = padding.bottom;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    if (itemsInConstraint >= maxInConstraint)
                    {
                        itemsInConstraint = 0;
                        currentCoverage += rect.width + spacing;
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _poolTracker.Add(this, itemTransform,
                DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta |
                DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax |
                DrivenTransformProperties.Pivot);

            item.BindData(ListData[_pool.Size]);
            
            _pool.Add(item);
        }
    }

    protected abstract void OnInstantiateListItem(TRecycleItem instance);

    private void RefreshView()
    {
        var dataIndex = _pool.FirstIndex;
        foreach (var recycleItem in _pool)
        {
            recycleItem.BindData(ListData[dataIndex]);
            dataIndex++;
        }
    }

    private void HandleScroll(Vector2 _)
    {
        if (_content is null)
            return;

        var dir = _content.anchoredPosition - _prevAnchoredPos;
        RecycleOnScroll(dir);
        _prevAnchoredPos = _content.anchoredPosition;
    }

    private void RecycleOnScroll(Vector2 dir)
    {
        if (_recycling)
            return;
        
        if (_recycleViewTransform is null)
            return;

        if (ListData.Count <= 0)
            return;
        
        UpdateViewBounds();
        
        if (dir.y > 0 || dir.x > 0)
        {
            var lastItem = _pool.GetLast();
            var lastLocalRect = _recycleViewTransform.GetRelativeRect(lastItem.RectTransform);
            
            switch (constraint)
            {
                case Constraint.Columns:
                    if (lastLocalRect.yMax > _viewBounds.min.y && startCorner is GridLayoutGroup.Corner.UpperLeft or GridLayoutGroup.Corner.UpperRight)
                    {
                        ScrollRecycleForwards();
                    }
                    else if (lastLocalRect.yMin < _viewBounds.max.y && startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight)
                    {
                        ScrollRecycleBackwards();
                    }
                    break;
                case Constraint.Rows:
                    if (lastLocalRect.xMax > _viewBounds.min.x && startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft)
                    {
                        ScrollRecycleForwards();
                    }
                    else if (lastLocalRect.xMin < _viewBounds.max.x && startCorner is GridLayoutGroup.Corner.LowerRight or GridLayoutGroup.Corner.UpperRight)
                    {
                        ScrollRecycleBackwards();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (dir.y < 0 || dir.x < 0)
        {
            var firstItem = _pool.GetFirst();
            var firstLocalRect = _recycleViewTransform.GetRelativeRect(firstItem.RectTransform);
            
            switch (constraint)
            {
                case Constraint.Columns:
                    if (firstLocalRect.yMin < _viewBounds.max.y && startCorner is GridLayoutGroup.Corner.UpperLeft or GridLayoutGroup.Corner.UpperRight)
                    {
                        ScrollRecycleBackwards();
                    }
                    else if (firstLocalRect.yMax > _viewBounds.min.y && startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight)
                    {
                        ScrollRecycleForwards();
                    }
                    break;
                case Constraint.Rows:
                    if (firstLocalRect.xMin < _viewBounds.max.x && startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft)
                    {
                        ScrollRecycleBackwards();
                    }
                    else if (firstLocalRect.xMax > _viewBounds.min.x && startCorner is GridLayoutGroup.Corner.LowerRight or GridLayoutGroup.Corner.UpperRight)
                    {
                        ScrollRecycleForwards();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private bool IsItemInsideViewBounds(TRecycleItem item)
    {
        if (_recycleViewTransform is null)
            return false;
        
        var localItemRect = _recycleViewTransform.GetRelativeRect(item.RectTransform);

        return localItemRect.xMin > _viewBounds.min.x && localItemRect.xMax < _viewBounds.max.x &&
               localItemRect.yMin > _viewBounds.min.y && localItemRect.yMax < _viewBounds.max.y;
    }

    private void ScrollRecycleForwards()
    {
        _recycling = true;
        
        while (!IsItemInsideViewBounds(_pool.GetFirst()) && _pool.LastIndex + 1 < ListData.Count)
        {
            RecycleItemForwards();
            
            _pool.RecycleForwards()
                .BindData(ListData[_pool.LastIndex]);
        }

        _recycling = false;
    }
    
    private void ScrollRecycleBackwards()
    {
        _recycling = true;

        while (!IsItemInsideViewBounds(_pool.GetLast()) && _pool.LastIndex + 1 > _pool.Size)
        {
            RecycleItemBackwards();
            
            _pool.RecycleBackwards()
                .BindData(ListData[_pool.FirstIndex]);
        }

        _recycling = false;
    }

    private void RecycleItemForwards()
    {
        var firstItem = _pool.GetFirst();
        var lastItem = _pool.GetLast();

        var firstRect = firstItem.RectTransform;
        var lastRect = lastItem.RectTransform;

        var nextPos = lastRect.anchoredPosition;
        int axisPadding;
        
        switch (constraint)
        {
            case Constraint.Columns:
                axisPadding = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
                    ? padding.left
                    : -padding.right;
                
                if (lastItem.ConstraintIndex + 1 >= maxInConstraint)
                {
                    nextPos.x = axisPadding;
                    nextPos.y += startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                        ? lastRect.rect.height + spacing
                        : -(lastRect.rect.height + spacing);

                    firstItem.ConstraintIndex = 0;
                }
                else
                {
                    firstItem.ConstraintIndex = lastItem.ConstraintIndex + 1;
                    
                    nextPos.x = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
                        ? firstItem.ConstraintIndex * (lastRect.rect.width + spacing) + axisPadding
                        : firstItem.ConstraintIndex * -(lastRect.rect.width + spacing) - axisPadding;
                }
                break;
            case Constraint.Rows:
                axisPadding = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                    ? -padding.bottom
                    : -padding.top;
                
                if (lastItem.ConstraintIndex + 1 >= maxInConstraint)
                {
                    nextPos.x += startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
                        ? lastRect.rect.width + spacing
                        : -(lastRect.rect.width + spacing);

                    nextPos.y = axisPadding;

                    firstItem.ConstraintIndex = 0;
                }
                else
                {
                    firstItem.ConstraintIndex = lastItem.ConstraintIndex + 1;
                    
                    nextPos.y = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                        ? firstItem.ConstraintIndex * (lastRect.rect.height + spacing) + axisPadding
                        : firstItem.ConstraintIndex * -(lastRect.rect.height + spacing) - axisPadding;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        firstRect.anchoredPosition = nextPos;
    }

    private void RecycleItemBackwards()
    {
        var firstItem = _pool.GetFirst();
        var lastItem = _pool.GetLast();
        
        var firstRect = firstItem.RectTransform;
        var lastRect = lastItem.RectTransform;
        
        var nextPos = firstRect.anchoredPosition;
        int axisPadding;
        
        switch (constraint)
        {
            case Constraint.Columns:
                axisPadding = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
                    ? padding.left
                    : -padding.right;
                
                if (firstItem.ConstraintIndex - 1 < 0)
                {
                    nextPos.y -= startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                        ? firstRect.rect.height + spacing
                        : -(firstRect.rect.height + spacing);

                    lastItem.ConstraintIndex = maxInConstraint - 1;
                }
                else
                {
                    lastItem.ConstraintIndex = firstItem.ConstraintIndex - 1;
                }
                
                nextPos.x = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft 
                    ? lastItem.ConstraintIndex * (firstRect.rect.width + spacing) + axisPadding
                    : lastItem.ConstraintIndex * -(firstRect.rect.width + spacing) - axisPadding;
                
                break;
            case Constraint.Rows:
                axisPadding = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                    ? -padding.bottom
                    : -padding.top;
                
                if (firstItem.ConstraintIndex - 1 < 0)
                {
                    nextPos.x -= startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.UpperLeft
                        ? firstRect.rect.width + spacing
                        : -(firstRect.rect.width + spacing);
                    
                    lastItem.ConstraintIndex = maxInConstraint - 1;
                }
                else
                {
                    lastItem.ConstraintIndex = firstItem.ConstraintIndex - 1;
                }
                
                nextPos.y = startCorner is GridLayoutGroup.Corner.LowerLeft or GridLayoutGroup.Corner.LowerRight
                    ? lastItem.ConstraintIndex * (firstRect.rect.height + spacing) + axisPadding
                    : lastItem.ConstraintIndex * -(firstRect.rect.height + spacing) - axisPadding;
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        lastRect.anchoredPosition = nextPos;
    }

    public enum Constraint
    {
        Columns,
        Rows
    }
}