using System;

namespace Game.CodeBase.Data
{
    [Serializable]
    public class ColorData
    {
        public ColorType Color;
        public Action Changed;

        public ColorData()
        {
            Color = ColorType.Red;
        }
        
        public void ChangeColor(ColorType colorType)
        {
            Color = colorType;
            Changed?.Invoke();
        }
    }
}