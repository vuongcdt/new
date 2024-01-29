using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;

public class MyItem : MonoBehaviour
{
    // [SerializeField] private Color _baseColor = Color.green, _offsetColor = Color.yellow;

    [SerializeField] private GameObject _myItem;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GridManager _gridManager;
    public int _x, _y, _index, _id;
    public bool _isHas;

    public MyItem(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public MyItem Init(int x, int y, int index, bool isHas, [CanBeNull] ImageSprite imageSprite,
        GridManager gridManager)
    {
        _x = x;
        _y = y;
        _index = index;
        _isHas = isHas;
        _id = imageSprite.Id;
        _gridManager = gridManager;
        _spriteRenderer.sprite = imageSprite.Sprite;
        _myItem.name = $"Item {x} {y}";
        _myItem.transform.localScale = new Vector3(0.97f, 0.97f);

        return this;
    }

    public MyItem FlipAxis(Enums.Axis axis)
    {
        if (axis == Enums.Axis.Horizontal)
        {
            var tempItem = new MyItem(0,0);
            tempItem._x = _y;
            tempItem._y = _x;
            return tempItem;
        }
         return this;
    }

    void OnMouseDown()
    {
        Print();
        _gridManager.SetItem(this);
    }

    public void SetShow(bool isActive)
    {
        _myItem.SetActive(isActive);
    }

    public void Print()
    {
        Debug.Log($"Down x: {_x} y: {_y} index: {_index} id: {_id} _isHas: {_isHas}");
    }
}