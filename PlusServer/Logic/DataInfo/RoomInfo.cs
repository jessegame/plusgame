using PlusServer.Logic.Enum;
using PlusServerCommon.Enum;

/// <summary>
/// 数据中心加载数据
/// </summary>
public class RoomInfo
{
    public RoomInfo()
    {
    }

    /// <summary>
    /// 邀请码
    /// </summary>
    public string InviteCode { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; }

    public int ServerId { get; set; }

    public int CreateUserId { get; set; }

    /// <summary>
    /// 底注 1/2 - 2/4 - 4/8
    /// </summary>  
    public int MinBet { get; set; }

    /// <summary>
    /// 游戏局数
    /// </summary>
    public int GameNum { get; set; }

    /// <summary>
    /// 付费模式 1房主/2玩家均摊
    /// </summary>
    public CostType CostType { get; set; }

    /// <summary>
    /// 规则1 牛牛3 牛九2 牛八2
    /// 规则2 牛牛4 牛九3 牛八2 牛七2
    /// </summary>
    public int Rule { get; set; }

    /// <summary>
    /// 金牛
    /// </summary>
    public int GoldNiu { get; set; }

    /// <summary>
    /// 四炸
    /// </summary>
    public int Quad { get; set; }

    /// <summary>
    /// 五小牛
    /// </summary>
    public int FiveNiu { get; set; }

    /// <summary>
    /// 闲家推注
    /// </summary>
    public bool FeeIncrease { get; set; }

    /// <summary>
    /// 游戏开始后是否进人 true禁止加入，false允许加入
    /// </summary>
    public bool StartedNoComeIn { get; set; }

    /// <summary>
    /// 上庄分数
    /// </summary>
    public int HolderScore { get; set; }

    /// <summary>
    /// 最大抢庄倍数
    /// </summary>
    public int GrabMultiple { get; set; }

    /// <summary>
    /// 玩家个数
    /// </summary>
    public int PlayerNum { get; set; }

    /// <summary>
    /// 玩法
    /// </summary>
    public NiuniuPlayType PlayType { get; set; }

    /// <summary>
    /// 银牛
    /// </summary>
    public int SilverNiu { get; set; }

    /// <summary>
    /// 当前是第几局
    /// </summary>
    public int GameCount { get; set; }

    /// <summary>
    /// 是否开启白名单
    /// </summary>
    public bool IsOpenWhiteName { get; set; }

    /// <summary>
    /// 是否开启搓牌
    /// </summary>
    public bool IsOpenShuffling { get; set; }

    /// <summary>
    /// 是否开启4人自动开始
    /// </summary>
    public bool IsOpenAutoStart { get; set; }

    /// <summary>
    /// 顺子牛
    /// </summary>
    public int StraightNiu { get; set; }

    /// <summary>
    /// 同花牛
    /// </summary>
    public int FlushNiu { get; set; }

    /// <summary>
    /// 葫芦牛
    /// </summary>
    public int GourdNiu { get; set; }

    /// <summary>
    /// 同花顺
    /// </summary>
    public int StraightFlush { get; set; }

    /// <summary>
    /// 是否下注限制
    /// </summary>
    public bool IsBetLimit { get; set; }

    /// <summary>
    /// 推注最大倍数
    /// </summary>
    public int PushBet { get; set; }

    /// <summary>
    /// 俱乐部Id
    /// </summary>
    public int HouseId { get; set; }

    /// <summary>
    /// 是否俱乐部默认牌桌
    /// </summary>
    public int HousePosition { get; set; }
}