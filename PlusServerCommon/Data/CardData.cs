using PlusServerCommon.Enum;

namespace PlusServerCommon.Data
{
    /// <summary>
    /// 牌面数据
    /// </summary>
    public struct CardData
    {
        private CardColor _color;
        private CardSize _size;
        public CardData(CardColor color, CardSize size)
        {
            this._color = color;
            this._size = size;
        }

        public CardColor Color
        {
            get { return _color; }
        }
        public CardSize Size
        {
            get { return _size; }
        }
    }
}
