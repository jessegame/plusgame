using System;

namespace PlusServer.Logic.Data
{
    /// <summary>
    /// 数据中心加载数据
    /// </summary>
    public class UserInfo
    {
        #region 基本信息

        public int UserId { get; set; }

        public string NickName { get; set; }

        public bool Sex { get; set; }

        public string HeadIcon { get; set; }

        public int UserLevel { get; set; }

        public int ExpNum { get; set; }

        public string FinalIP { get; set; }

        public int Diamond { get; set; }

        public string RegisterDate { get; set; }

        public string Location { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// 是否黑名单
        /// </summary>
        public byte State { get; set; }

        public int PlayNum { get; set; }

        #endregion
    }
}
