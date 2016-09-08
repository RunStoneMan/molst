using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace iTrackStar.MYHM.Utility
{
    /// <summary>
    /// 功能描述：获取配置文件的值
    /// 创建标识：余帝海 2012-06-27
    /// 修改标识：
    /// 修改描述：
    /// </summary>
    public class ConfigHelper
    {

        /// <summary>
        /// 获取锁车渗透率，时间天数配置
        /// </summary>
        public static string LockDay
        {
            get
            {
                return ConfigurationManager.AppSettings["LockDay"].ToString();
            }
        }

        public static string OnlineDay
        {
            get
            {
                return ConfigurationManager.AppSettings["OnlineDay"].ToString();
            }
        }
        /// <summary>
        /// 获取维保查询，保养策略泵车设备类型ID
        /// </summary>
        public static string PumperVehType
        {
            get
            {
                return ConfigurationManager.AppSettings["PumperVehType"].ToString();
            }
        }

        /// <summary>
        /// 获取维保查询，保养策略搅泵站设备类型ID
        /// </summary>
        public static string MixingPlantVehType
        {
            get
            {
                return ConfigurationManager.AppSettings["MixingPlantVehType"].ToString();
            }
        }

        /// <summary>
        /// 终端故障统计,终端型号
        /// </summary>
        public static string TerminalModel
        {
            get
            {
                return ConfigurationManager.AppSettings["TerminalModeL"].ToString();
            }

        }

        ///// <summary>
        ///// 工作时间分析，工作累计时间
        ///// </summary>
        //public static string AccounTime
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["ACCOUNTIME"].ToString();
        //    }
        //}

        ///// <summary>
        ///// 工作方量分析，工作累计方量
        ///// </summary>
        //public static string PumpTotal
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["PUMPTOTAL"].ToString();
        //    }
        //}
        /// <summary>
        /// 位置分布--泵车设备类型
        /// </summary>
        public static string Type_BangChe
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_BangChe"].ToString();
            }
        }
        /// <summary>
        /// 车载泵外型图
        /// </summary>
        public static string Type_CheZaiBangWai
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_CheZaiBangWai"].ToString();
            }
        }
        /// <summary>
        /// 混凝土搅拌运输车
        /// </summary>
        public static string Type_HunNiTuJiaoBanYunShuChe
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_HunNiTuJiaoBanYunShuChe"].ToString();
            }
        }
        /// <summary>
        /// 混凝土拖泵
        /// </summary>
        public static string Type_HunNiTuTuoBang
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_HunNiTuTuoBang"].ToString();
            }
        }
        /// <summary>
        /// 搅拌站
        /// </summary>
        public static string Type_JiaoBanZhan
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_JiaoBanZhan"].ToString();
            }
        }
        public static string Type_Service
        {
            get {
                return ConfigurationManager.AppSettings["Type_Service"].ToString();
            }
        }
        public static string Type_JPZUnit
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_JPZUnit"].ToString();
            }
        }
        public static string Type_JCSG
        {
            get
            {
                return ConfigurationManager.AppSettings["Type_JCSG"].ToString();
            }
        }
        public static int Plat_JCSG
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["JCSGUnit"].ToString());
            }
        }

        /// <summary>
        /// 获取待更换终端录入页面中指定机构
        /// </summary>
        public static string[] allowSearchTerminalOfUnitNames
        {
            get
            {
                string unitNames = ConfigurationManager.AppSettings["allowSearchUnitNames"].ToString();

                if (string.IsNullOrEmpty(unitNames))
                {
                    return null;
                }

                return unitNames.Split(',');
            }
        }

    }
}
