using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width = 18, _height = 11;
    [SerializeField] private MyItem _item;

    [SerializeField] private Transform _camera;

    private List<ImageSprite> _images = new List<ImageSprite>();

    private List<ItemDto> _items = new List<ItemDto>();
    private List<ItemDto> _itemsNoValue = new List<ItemDto>();
    private List<int> _itemsNoValueByAxis = new List<int>();

    private MyItem _firstItem, _lastItem;

    public void SetItem(MyItem item)
    {
        if (!item._isHas)
        {
            _firstItem = null;
            _lastItem = null;
            return;
        }

        if (_firstItem?._id > 0) _lastItem = item;
        else _firstItem = item;

        if (_firstItem?._id == _lastItem?._id)
        {
            //audio witch
            foreach (var itemDto in _items)
            {
                if (itemDto.Id == _lastItem._id || _firstItem._id == itemDto.Id)
                    itemDto.IsHas = false;
            }

            if (CompareItem())
            {
                _firstItem.SetShow(false);
                _lastItem.SetShow(false);
            }

            foreach (var itemDto in _items)
            {
                if (itemDto.Id == _lastItem?._id || _firstItem?._id == itemDto.Id)
                    itemDto.IsHas = true;
            }

            _lastItem = null;
            _firstItem = null;
        }
        else if (_firstItem?._id > 0 & _lastItem?._id > 0 & _lastItem?._id != _firstItem?._id)
        {
            //audio oho
            Debug.Log("khong cung loai");
            _lastItem = null;
            _firstItem = null;
        }
    }

    private bool CompareItem()
    {
        _itemsNoValueByAxis = new List<int>();
        GetNoValue();
        // GetNoValueVertical();
        // GetNoValueHorizontal();
        GetNoValueByAxis(Enums.Axis.Vertical, _firstItem, _lastItem, null);
        Debug.Log(_itemsNoValueByAxis.Count);

        var isPass = false;
        foreach (var itemNoValueByAxis in _itemsNoValueByAxis)
        {
            var result = GetNoValueByAxis(Enums.Axis.Horizontal, _firstItem,
                             new MyItem(itemNoValueByAxis, _firstItem._y), _firstItem._x)
                         && GetNoValueByAxis(Enums.Axis.Horizontal, _lastItem,
                             new MyItem(itemNoValueByAxis, _lastItem._y), _lastItem._x);
            if (result)
            {
                isPass = true;
                break;
            }
        }

        Debug.Log($"isPass: {isPass}");
        return isPass;
    }

    private void GetNoValue()
    {
        foreach (var itemDto in _items)
        {
            if (!itemDto.IsHas) _itemsNoValue.Add(itemDto);
        }
    }

    private bool GetNoValueByAxis(Enums.Axis axis, MyItem firstItem, MyItem lastItem, int? sameValue)
    {
        Debug.Log($"{firstItem._x} {firstItem._y}, {lastItem._x} {lastItem._y}, {sameValue} ?????? axis:{axis}");
        var minItem = firstItem.FlipAxis(axis)._x < lastItem.FlipAxis(axis)._x ? firstItem : lastItem;
        var maxItem = firstItem.FlipAxis(axis)._x > lastItem.FlipAxis(axis)._x ? firstItem : lastItem;
        var distance = maxItem.FlipAxis(axis)._x - minItem.FlipAxis(axis)._x + 1;

        var items = new List<ItemDto>();
        foreach (var itemDto in _itemsNoValue)
        {
            if (sameValue.HasValue && itemDto.FlipAxis(axis).Y != sameValue)
                continue;
            if (minItem.FlipAxis(axis)._x <= itemDto.FlipAxis(axis).X &&
                maxItem.FlipAxis(axis)._x >= itemDto.FlipAxis(axis).X)
                items.Add(itemDto);
        }

        if (sameValue.HasValue) return items.Count > 0;

        Debug.Log(items.Count+"/////////");
        _itemsNoValueByAxis = items
            .GroupBy(e => e.FlipAxis(axis).Y)
            .Where(e => e.Count() == distance)
            .Select(e => e.Key)
            .ToList();

        Debug.Log(_itemsNoValueByAxis.Count+"}}}}}}}}}}");
        return true;
    }

    // private bool GetNoValueByAxis(MyItem firstItem, MyItem lastItem, Enums.Axis axis, int? sameValue)
    // {
    //     var firstItemByAxis = axis == Enums.Axis.Vertical ? firstItem._x : firstItem._y;
    //     var lastItemByAxis = axis == Enums.Axis.Vertical ? lastItem._x : lastItem._y;
    //
    //     var minItemByAxis = firstItemByAxis < lastItemByAxis ? firstItem : lastItem;
    //     var maxItemByAxis = firstItemByAxis > lastItemByAxis ? firstItem : lastItem;
    //
    //     var distance = axis == Enums.Axis.Vertical
    //         ? maxItemByAxis._x - minItemByAxis._x + 1
    //         : maxItemByAxis._y - minItemByAxis._y + 1;
    //
    //     var items = new List<ItemDto>();
    //
    //     foreach (var itemDto in _itemsNoValue)
    //     {
    //         if (sameValue.HasValue)
    //         {
    //             if (!(axis == Enums.Axis.Vertical
    //                     ? itemDto.Y == sameValue
    //                     : itemDto.X == sameValue)) continue;
    //         }
    //
    //         if (minItemByAxis._x <= itemDto.X && maxItemByAxis._x >= itemDto.X && axis == Enums.Axis.Vertical)
    //             items.Add(itemDto);
    //         if (minItemByAxis._y <= itemDto.Y && maxItemByAxis._y >= itemDto.Y && axis == Enums.Axis.Horizontal)
    //             items.Add(itemDto);
    //     }
    //
    //     if (sameValue.HasValue) return items.Count > 0;
    //
    //     _itemsNoValueVertical = items
    //         .GroupBy(e => axis == Enums.Axis.Vertical ? e.Y : e.X)
    //         .Where(e => e.Count() == distance)
    //         .Select(e => e.Key)
    //         .ToList();
    //     return true;
    // }

    // private void GetNoValueHorizontal()
    // {
    //     var minItemHorizontal = _firstItem._y < _lastItem._y ? _firstItem : _lastItem;
    //     var maxItemHorizontal = _firstItem._y > _lastItem._y ? _firstItem : _lastItem;
    //
    //     foreach (var itemDto in _itemsNoValue)
    //     {
    //         if (minItemHorizontal._y < itemDto.Y && maxItemHorizontal._y > itemDto.Y)
    //             _itemsNoValueHorizontal.Add(itemDto);
    //     }
    // }

    private void Awake()
    {
        GetResouece();
        GenerateGrid();
    }

    private void GetResouece()
    {
        if (_images.Count > 0) return;
        var nums = Enumerable.Range(1, 18);
        foreach (var num in nums)
        {
            var image = Resources.Load<Sprite>($"pieces{num}");
            var newItem = new ImageSprite()
            {
                Id = num,
                Sprite = image
            };
            _images.Add(newItem);
            _images.Add(newItem);
            _images.Add(newItem);
            _images.Add(newItem);
            
            _images.Add(newItem);
            _images.Add(newItem);
            _images.Add(newItem);
            _images.Add(newItem);
        }

        //fix

        _images = _images
            .OrderBy(e => Random.Range(0, _width * _height))
            .ToList();
    }

    private void GenerateGrid()
    {
        var count = 0;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var index = x * _height + y + 1;
                var spawnedItem = Instantiate(_item, new Vector3(x, y), Quaternion.identity);
                spawnedItem.name = $"Item {x} {y}";
                spawnedItem.transform.localScale = new Vector3(0.97f, 0.97f);
                var isHas = !(y == 0 || y == _height - 1 || x == 0 || x == _width - 1);
                // var ramdom = Random.Range(0, 36);

                spawnedItem.Init(x, y, index, isHas, isHas ? _images[count] : new ImageSprite(), this);
                _items.Add(new ItemDto(x, y, index, _images[count].Id, isHas));
                if (isHas) count++;
            }
        }

        _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }
}