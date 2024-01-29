using UnityEngine;

namespace DefaultNamespace
{
    public class ImageSprite
    {
        public Sprite Sprite;
        public int Id;
    }

    public class ItemDto
    {
        public int X, Y, Index, Id;
        public bool IsHas;

        public ItemDto(int x, int y, int index, int id, bool isHas)
        {
            X = x;
            Y = y;
            Index = index;
            Id = id;
            IsHas = isHas;
        }

        public ItemDto FlipAxis(Enums.Axis axis)
        {
            if (axis == Enums.Axis.Horizontal)
            {
                var tempItemDto = new ItemDto(Y, X, Index, Id, IsHas);
                return tempItemDto;
            }
             return this;
        }

        public void Print()
        {
            Debug.Log($"Item x: {X} y: {Y} index: {Index} id: {Id} isHas: {IsHas}");
        }
    }
}