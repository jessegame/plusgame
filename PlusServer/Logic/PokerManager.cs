using System;
using System.Collections.Generic;
using System.Linq;
using PlusCommon;
using PlusServer.Logic.Data;
using PlusServer.Logic.Enum;
using PlusServer.LogicActors;
using PlusServerCommon.Data;
using PlusServerCommon.Enum;

namespace Common.Logic
{
    public class PokerManager
    {
        public static List<int> CardDatas = new List<int>();

        #region 工具类

        /// <summary>
        /// 获取扑克牌对应的数字
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static byte GetCardNum(CardData card)
        {
            if (card.Color == CardColor.Wang || card.Color == CardColor.King)
                return (byte)(card.Size - 1);

            return (byte)(((byte)card.Size - 1) * 4 + (byte)(card.Color) - 1);
        }

        /// <summary>
        /// 通过数字获取一张扑克牌
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static CardData GetCardData(byte num)
        {
            num = (byte)(num + 1);
            if (num > 54 || num < 0)
                throw new Exception("num is error!");

            if (num == (byte)(CardSize.C_Wang))
                return new CardData(CardColor.Wang, CardSize.C_Wang);
            if (num == (byte)(CardSize.C_King))
                return new CardData(CardColor.King, CardSize.C_King);

            CardColor color = (CardColor)((num - 1) % 4 + 1);
            CardSize size = (CardSize)((num - 1) / 4 + 1);

            return new CardData(color, size);
        }

        #endregion

        #region 棋牌类公用部分

        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="cardData"></param>
        public static void ShuffleCard(List<int> cardData)
        {
            if (CardDatas.Count != 0)
            {
                cardData.Clear();
                cardData.AddRange(CardDatas);
            }
            else
            {
                RandomCards(cardData);
            }
        }

        /// <summary>
        /// 随机取牌
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="timeNum">次数</param>
        /// <returns></returns>
        private static void RandomCards(List<int> cardData, int timeNum = 0)
        {
            int count = cardData.Count;
            int randCount = 0;
            int index = 0;
            int endIndex = 0;
            int temp = 0;
            int val = 0;
            do
            {
                int endPos = count - randCount;
                if (endPos <= 0)
                {
                    break;
                }
                index = RandomUtils.GetRandom(0, endPos);
                endIndex = endPos - 1;
                if (endIndex == index)
                {
                    //自己与自己不替换
                    randCount++;
                    continue;
                }
                temp = cardData[endIndex];
                val = cardData[index];
                cardData[endIndex] = val;
                cardData[index] = temp;
                randCount++;
            } while (randCount < count && (timeNum == 0 || randCount < timeNum));

        }

        #endregion

        #region 不同棋牌具体实现

        public static int GetRealSize(CardSize size)
        {
            if ((int)size > 10)
                return 10;
            return (int)size;
        }

        /// <summary>
        /// 获取手牌大小，比较大小用
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="roomData"></param>
        /// <returns></returns>
        public static int GetCardSize(List<CardData> srcCards, RoomActor roomData)
        {
            var cards = new List<CardData>(srcCards);//防止引用修改顺序

            if (cards.Count != 5)
                throw new Exception("cards 为 null");

            int niuNum = GetNiuNum(cards, roomData);

            CardData max = new CardData();
            if (niuNum == 21 || niuNum == 81)
            {
                max = cards.OrderBy(m => (int)m.Size * 10 + (int)m.Color).First();
            }
            else
            {
                max = cards.OrderByDescending(m => (int)m.Size * 10 + (int)m.Color).First();
            }

            return niuNum * 1000 + (int)max.Size * 10 + (int)max.Color;
        }

        public static int GetNiuNum(List<CardData> srcCards, RoomActor roomData)
        {
            var cards = new List<CardData>(srcCards);//防止引用修改顺序
            cards.Sort((x, y) => (int)x.Size - (int)y.Size);

            List<int> nums = new List<int>();
            foreach (var item in cards)
            {
                nums.Add(GetRealSize(item.Size));
            }

            

            if (roomData.StraightFlush != 0)
            {
                //同花顺

                //同花牛
                if (cards.TrueForAll(m => m.Color == cards[0].Color))
                {
                    //顺子牛
                    if (cards[0].Size != cards[1].Size &&
                        cards[1].Size != cards[2].Size &&
                        cards[2].Size != cards[3].Size &&
                        cards[3].Size != cards[4].Size &&
                        cards[0].Size + 4 == cards[4].Size)
                    {
                        return 80;
                    }

                    if (cards[0].Size == CardSize.C_1 &&
                        cards[1].Size == CardSize.C_10 &&
                        cards[2].Size == CardSize.C_J &&
                        cards[3].Size == CardSize.C_Q &&
                        cards[4].Size == CardSize.C_K)
                    {
                        return 81;
                    }
                }
            }

            if (roomData.FiveNiu != 0)
            {
                //五小牛
                if (cards.Sum(m => (int)m.Size) <= 10 &&
                    cards.TrueForAll(m => m.Size < CardSize.C_5))
                {
                    return 70;
                }
            }

            if (roomData.Quad != 0)
            {
                //炸弹
                if (cards[0].Size == cards[3].Size ||
                    cards[1].Size == cards[4].Size)
                {
                    return 60;
                }
            }

            if (roomData.GourdNiu != 0)
            {
                //葫芦牛
                if ((cards[0].Size == cards[2].Size && cards[3].Size == cards[4].Size)||
                    (cards[0].Size == cards[1].Size && cards[2].Size == cards[4].Size))
                {
                    return 50;
                }
            }

            if (roomData.FlushNiu != 0)
            {
                //同花牛
                if (cards.TrueForAll(m => m.Color == cards[0].Color))
                {
                    return 40;
                }
            }

            if (roomData.GoldNiu != 0)
            {
                //金牛
                if ((int)cards[0].Size > 10)
                {
                    return 30;
                }
            }

            if (roomData.StraightNiu != 0)
            {
                //顺子牛
                if (cards[0].Size != cards[1].Size &&
                    cards[1].Size != cards[2].Size &&
                    cards[2].Size != cards[3].Size &&
                    cards[3].Size != cards[4].Size &&
                    cards[0].Size + 4 == cards[4].Size)
                {
                    return 20;
                }

                if (cards[0].Size == CardSize.C_1 &&
                    cards[1].Size == CardSize.C_10 &&
                    cards[2].Size == CardSize.C_J &&
                    cards[3].Size == CardSize.C_Q &&
                    cards[4].Size == CardSize.C_K)
                {
                    return 21;
                }
            }

            if (roomData.SilverNiu != 0)
            {
                //银牛 只有一张牌是10
                if ((int)cards[0].Size == 10 && (int)cards[1].Size > 10)
                {
                    return 15;
                }
            }

            int num = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    for (int k = j + 1; k < 5; k++)
                    {
                        if ((nums[i] + nums[j] + nums[k]) % 10 == 0)
                        {
                            num = nums.Sum() % 10;
                            num = num == 0 ? 10 : num;
                        }
                    }
                }
            }
            return num;
        }

        public static int GetNiuNum(List<CardData> srcCards)
        {
            var cards = new List<CardData>(srcCards);//防止引用修改顺序
            cards.Sort((x, y) => (int)x.Size - (int)y.Size);

            List<int> nums = new List<int>();
            foreach (var item in cards)
            {
                nums.Add(GetRealSize(item.Size));
            }


            //同花顺

            //同花牛
            if (cards.TrueForAll(m => m.Color == cards[0].Color))
            {
                //顺子牛
                if (cards[0].Size != cards[1].Size &&
                    cards[1].Size != cards[2].Size &&
                    cards[2].Size != cards[3].Size &&
                    cards[3].Size != cards[4].Size &&
                    cards[0].Size + 4 == cards[4].Size)
                {
                    return 80;
                }

                if (cards[0].Size == CardSize.C_1 &&
                    cards[1].Size == CardSize.C_10 &&
                    cards[2].Size == CardSize.C_J &&
                    cards[3].Size == CardSize.C_Q &&
                    cards[4].Size == CardSize.C_K)
                {
                    return 81;
                }
            }

            //五小牛
            if (cards.Sum(m => (int)m.Size) <= 10 &&
                cards.TrueForAll(m => m.Size < CardSize.C_5))
            {
                return 70;
            }

            //炸弹
            if (cards[0].Size == cards[3].Size ||
                cards[1].Size == cards[4].Size)
            {
                return 60;
            }

            //葫芦牛
            if ((cards[0].Size == cards[2].Size && cards[3].Size == cards[4].Size) ||
                (cards[0].Size == cards[1].Size && cards[2].Size == cards[4].Size))
            {
                return 50;
            }

            //同花牛
            if (cards.TrueForAll(m => m.Color == cards[0].Color))
            {
                return 40;
            }

            //金牛
            if ((int)cards[0].Size > 10)
            {
                return 30;
            }

            //顺子牛
            if (cards[0].Size != cards[1].Size &&
                cards[1].Size != cards[2].Size &&
                cards[2].Size != cards[3].Size &&
                cards[3].Size != cards[4].Size &&
                cards[0].Size + 4 == cards[4].Size)
            {
                return 20;
            }

            if (cards[0].Size == CardSize.C_1 &&
                cards[1].Size == CardSize.C_10 &&
                cards[2].Size == CardSize.C_J &&
                cards[3].Size == CardSize.C_Q &&
                cards[4].Size == CardSize.C_K)
            {
                return 21;
            }



            //银牛 只有一张牌是10
            if ((int)cards[0].Size == 10 && (int)cards[1].Size > 10)
            {
                return 15;
            }


            int num = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    for (int k = j + 1; k < 5; k++)
                    {
                        if ((nums[i] + nums[j] + nums[k]) % 10 == 0)
                        {
                            num = nums.Sum() % 10;
                            num = num == 0 ? 10 : num;
                        }
                    }
                }
            }
            return num;
        }

        public static List<CardData> SortCards(List<CardData> srcCards, RoomActor roomData)
        {
            var cards = new List<CardData>(srcCards);//防止引用修改顺序

            if (cards.Count != 5)
                return cards;

            cards.Sort((x, y) => GetCardNum(x) - GetCardNum(y));

            List<int> nums = new List<int>();
            foreach (var item in cards)
            {
                nums.Add(GetRealSize(item.Size));
            }

            if (roomData.StraightFlush != 0)
            {
                //同花牛
                if (cards.TrueForAll(m => m.Color == cards[0].Color))
                {
                    //顺子牛
                    if (cards[0].Size != cards[1].Size &&
                        cards[1].Size != cards[2].Size &&
                        cards[2].Size != cards[3].Size &&
                        cards[3].Size != cards[4].Size &&
                        cards[0].Size + 4 == cards[4].Size)
                    {
                        return cards;
                    }

                    if (cards[0].Size == CardSize.C_1 &&
                    cards[1].Size == CardSize.C_10 &&
                    cards[2].Size == CardSize.C_J &&
                    cards[3].Size == CardSize.C_Q &&
                    cards[4].Size == CardSize.C_K)
                    {
                        return new List<CardData>() { cards[1], cards[2], cards[3], cards[4], cards[0] };
                    }
                }
            }

            if (roomData.FiveNiu != 0)
            {
                //五小牛
                if (cards.Sum(m => (int)m.Size) <= 10 &&
                    cards.TrueForAll(m => m.Size < CardSize.C_5))
                {
                    return cards;
                }
            }

            if (roomData.Quad != 0)
            {
                //炸弹
                if (cards[0].Size == cards[3].Size ||
                    cards[1].Size == cards[4].Size)
                {
                    return cards;
                }
            }

            if (roomData.GourdNiu != 0)
            {
                //葫芦牛
                if (cards[0].Size == cards[2].Size && cards[3].Size == cards[4].Size)
                {
                    return cards;
                }
                else if ((cards[0].Size == cards[1].Size && cards[2].Size == cards[4].Size))
                {
                    return new List<CardData>() { cards[2], cards[3], cards[4], cards[0], cards[1] };
                }
            }

            if (roomData.FlushNiu != 0)
            {
                //同花牛
                if (cards.TrueForAll(m => m.Color == cards[0].Color))
                {
                    return cards;
                }
            }

            if (roomData.GoldNiu != 0)
            {
                //金牛
                if ((int)cards[0].Size > 10)
                {
                    return cards;
                }
            }

            if (roomData.StraightNiu != 0)
            {
                //顺子牛
                if (cards[0].Size != cards[1].Size &&
                    cards[1].Size != cards[2].Size &&
                    cards[2].Size != cards[3].Size &&
                    cards[3].Size != cards[4].Size &&
                    cards[0].Size + 4 == cards[4].Size)
                {
                    return cards;
                }

                if (cards[0].Size == CardSize.C_1 &&
                    cards[1].Size == CardSize.C_10 &&
                    cards[2].Size == CardSize.C_J &&
                    cards[3].Size == CardSize.C_Q &&
                    cards[4].Size == CardSize.C_K)
                {
                    return new List<CardData>() { cards[1], cards[2], cards[3], cards[4], cards[0] };
                }
            }

            if (roomData.SilverNiu != 0)
            {
                //银牛 只有一张牌是10
                if ((int)cards[0].Size == 10 && (int)cards[1].Size > 10)
                {
                    return cards;
                }
            }

            int ii = -1;
            int jj = -1;
            int kk = -1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 5; j++)
                {
                    for (int k = j + 1; k < 5; k++)
                    {
                        if ((nums[i] + nums[j] + nums[k]) % 10 == 0)
                        {
                            ii = i;
                            jj = j;
                            kk = k;
                        }
                    }
                }
            }

            List<CardData> newCards = new List<CardData>();
            if (ii != -1 && jj != -1 && kk != -1)
            {
                //解决重复牌型12人
                var tempCard = new List<CardData>();
                for (int i = 0; i < cards.Count; i++)
                {
                    if (ii == i || jj == i || kk == i)
                    {
                        newCards.Add(cards[i]);
                    }
                    else
                    {
                        tempCard.Add(cards[i]);
                    }
                }
                newCards.AddRange(tempCard);
            }
            else
            {
                newCards.AddRange(cards);
            }
            return newCards;
        }

        public static int GetMultiple(int num, RoomActor roomData)
        {
            if (num < 0 || num > 100)
                return 0;

            if (roomData.Rule == 1)
            {
                if (num < 7)
                    return 1;
                else if (num == 7)
                    return 2;
                else if (num == 8)
                    return 2;
                else if (num == 9)
                    return 3;
                else if (num == 10)
                    return 4;
            }
            else if (roomData.Rule == 2)
            {
                if (num < 7)
                    return 1;
                else if (num == 7)
                    return 1;
                else if (num == 8)
                    return 2;
                else if (num == 9)
                    return 2;
                else if (num == 10)
                    return 3;
            }
            else if (roomData.Rule == 3)
            {
                if (num < 7)
                    return 1;
                else if (num == 7)
                    return 2;
                else if (num == 8)
                    return 2;
                else if (num == 9)
                    return 2;
                else if (num == 10)
                    return 3;
            }
            else if (roomData.Rule == 4)
            {
                if (num < 7)
                    return 1;
                else if (num == 7)
                    return 2;
                else if (num == 8)
                    return 3;
                else if (num == 9)
                    return 4;
                else if (num == 10)
                    return 5;
            }
            else if (roomData.Rule == 5)
            {
                if (num <= 1)
                    return 1;
                else if (num == 2)
                    return 2;
                else if (num == 3)
                    return 3;
                else if (num == 4)
                    return 4;
                else if (num == 5)
                    return 5;
                else if (num == 6)
                    return 6;
                else if (num == 7)
                    return 7;
                else if (num == 8)
                    return 8;
                else if (num == 9)
                    return 9;
                else if (num == 10)
                    return 10;
            }

            //特殊牌型
            if (num == 15)
                return roomData.SilverNiu;
            else if (num == 20 || num == 21)
                return roomData.StraightNiu;
            else if (num == 30)
                return roomData.GoldNiu;
            else if (num == 40)
                return roomData.FlushNiu;
            else if (num == 50)
                return roomData.GourdNiu;
            else if (num == 60)
                return roomData.Quad;
            else if (num == 70)
                return roomData.FiveNiu;
            else if (num == 80 || num == 81)
                return roomData.StraightFlush;

            return 0;
        }


        /// <summary>
        /// 输赢多少倍
        /// </summary>
        /// <param name="cards1"></param>
        /// <param name="cards2"></param>
        /// <returns></returns>
        public static int CompareCard(List<CardData> cards1, List<CardData> cards2, RoomActor roomData)
        {
            int num1 = GetCardSize(cards1, roomData);
            int num2 = GetCardSize(cards2, roomData);

            int niu1 = num1 / 1000;
            int niu2 = num2 / 1000;

            if (num1 > num2)
            {
                return GetMultiple(niu1, roomData);
            }
            else
            {
                return -GetMultiple(niu2, roomData);
            }

            //if (cards1.Count == 0)
            //    throw new Exception("cards1 为 null");

            //if (cards2.Count == 0)
            //    throw new Exception("cards2 为 null");

            //int num1 = GetNiuNum(cards1, roomData);
            //int num2 = GetNiuNum(cards2, roomData);

            //bool result = false;

            //if (num1 != num2)
            //{
            //    result = num1 > num2;
            //}
            //else
            //{
            //    var max1 = cards1.OrderByDescending(m => (int)m.Size).First();
            //    var max2 = cards2.OrderByDescending(m => (int)m.Size).First();


            //    if (max1.Size != max2.Size)
            //    {
            //        result = (int)max1.Size > (int)max2.Size;
            //    }
            //    else
            //    {
            //        result = (int)max1.Color > (int)max2.Color;
            //    }
            //}

            //if (num1 > num2)
            //{
            //    return GetMultiple(num1, roomData);
            //}
            //else
            //{
            //    return -GetMultiple(num2, roomData);
            //}
        }

        #endregion
    }
}
