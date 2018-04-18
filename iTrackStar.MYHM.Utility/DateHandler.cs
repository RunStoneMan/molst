using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace iTrackStar.MYHM.Utility
{

    public class DateHandler
    {
        /// <summary>日期元素转换DateTime
        /// 将日期元素转换为DateTime及其他关于日期的所需转换
        /// </summary>
        /// <param name="prmYear">Year</param>
        /// <param name="prmMonth">Month</param>
        /// <param name="prmDay">Day</param>
        /// <returns></returns>
        public static DateTime FmtToDT(int prmYear, int prmMonth, int prmDay)
        {
            string _month = prmMonth > 9 ? prmMonth.ToString() : "0" + prmMonth.ToString();
            string _day = prmDay > 9 ? prmDay.ToString() : "0" + prmDay.ToString();

            return DateTime.Parse(prmYear.ToString() + "-" + _month + "-" + _day);
        }

        /// <summary>当月第一天
        /// 传入日期当月的第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime getFirstDay(DateTime dt)
        {
            //拆解年月日，替换Day值后重组
            return FmtToDT(dt.Year, dt.Month, 1);
        }

        /// <summary>下月的第一天
        /// 传入日期下个月的第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime getFirstDayNextMonth(DateTime dt)
        {
            //月+1
            dt = dt.AddMonths(1);

            //第一天
            return getFirstDay(dt);
        }

        /// <summary>当月最后一天
        /// 传入日期当月最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime getLastDay(DateTime dt)
        {
            //下月第一天
            dt = getFirstDayNextMonth(dt);

            //天减一
            return dt.AddDays(-1);
        }

        /// <summary>上个月最后一天
        /// 传入日期上个月最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime getLastDayPreMonth(DateTime dt)
        {
            //本月第一天
            dt = getFirstDay(dt);

            //天减一
            return dt.AddDays(-1);
        }

        /// <summary>比较传入日期是否大于当前日期
        /// 比较传入日期是否大于当前日期，是则回传当前日期，否则回传传入日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ComparisonDateNow(DateTime dt)
        {
            return dt > DateTime.Now ? DateTime.Now : dt;
        }

        /// <summary>比较传入年-月的最后一天是否大于当前日期
        /// 比较传入年-月的最后一天是否大于当前日期，
        /// 是则回传当前日期，否则回传传入年-月的最后一天日期
        /// </summary>
        /// <param name="prmYM">YYYY-MM</param>
        /// <returns></returns>
        public static DateTime ComparisonDateNow(string prmYM)
        {
            return ComparisonDateNow(getLastDay(DateTime.Parse(prmYM + "-01")));
        }

        /// <summary>转换为　Year-Month
        /// 将标准日期格式转换为　Year-Month 格式
        /// </summary>
        /// <param name="dt">传入日期</param>
        /// <returns></returns>
        public static string FmtToDT_YM(DateTime dt)
        {
            //1900-01-01 sub 0,
            return dt.ToString("yyyy-MM");
        }

        /// <summary>增加指定的天数
        /// 将传入日期增加指定的天数
        /// </summary>
        /// <param name="dt">传入日期</param>
        /// <param name="days">指定增加的天数</param>
        /// <returns></returns>
        public static DateTime getAddDates(DateTime dt, double days)
        {
            return dt.AddDays(days);
        }

        /// <summary>至月末的天数
        /// 计算从传入日期起至月末的天数
        /// </summary>
        /// <param name="dt">传入日期</param>
        /// <returns></returns>
        public static int getDaysToLast(DateTime dt)
        {
            return getDiffDays(dt, getLastDay(dt));
        }

        /// <summary>至月初的天数
        /// 计算从传入日期起至月初的天数
        /// </summary>
        /// <param name="dt">传入日期</param>
        /// <returns></returns>
        public static int getDaysFromFirst(DateTime dt)
        {
            return dt.Day;
        }

        /// <summary>当月的天数
        /// 计算传入日期当月的天数
        /// </summary>
        /// <param name="dt">传入日期</param>
        /// <returns></returns>
        public static int getDaysByMonth(DateTime dt)
        {
            return getDiffDays(getFirstDay(dt), getLastDay(dt));
        }

        /// <summary>当月的天数
        /// 计算传入日期当月的天数
        /// </summary>
        /// <param name="dtYM">传入的YYYY-MM字符串</param>
        /// <returns></returns>
        public static int getDaysByMonth(string dtYM)
        {
            DateTime _dt = DateTime.Parse(dtYM + "-01");
            return getDiffDays(getFirstDay(_dt), getLastDay(_dt));
        }

        /// <summary>间隔的天数
        /// 获取两个传入日期间隔的天数
        /// </summary>
        /// <param name="bDate">起始日期</param>
        /// <param name="eDate">结束日期</param>
        /// <returns></returns>
        public static int getDiffDays(DateTime bDate, DateTime eDate)
        {
            TimeSpan ts = new TimeSpan();
            ts = eDate - bDate;

            return (int)ts.TotalDays + 1;
        }

        /// <summary>比较 倒推
        /// 比较传入日期是否大于当前日期(-pd)，是则回传当前日期(-pd)，否则回传传入日期
        /// </summary>
        /// <param name="dt">比较的日期</param>
        /// <param name="prvDate">当前日期的回退天数</param>
        /// <returns>比较结果中的较小值</returns>
        public static DateTime ComparisonDate(DateTime dt, int prvDate)
        {
            DateTime prvDt = DateTime.Now.AddDays(-1 * prvDate);

            return dt < prvDt ? dt : prvDt;
        }

        /// <summary>比较 倒推
        /// 比较传入日期[YYYY-MM]是否大于当前日期(-pd)，是则回传当前日期(-pd)，否则回传传入日期
        /// </summary>
        /// <param name="dt">比较的日期[YYYY-MM]字符串</param>
        /// <param name="prvDate">当前日期的回退天数</param>
        /// <param name="flag">置换类型 F：第一天 | L：最后一天</param>
        /// <returns></returns>
        public static DateTime ComparisonDate(string dt, int prvDate, string flag)
        {
            switch (flag)
            {
                case "F":
                    return ComparisonDate(DateTime.Parse(dt + "-01"), prvDate);
                default:    //"L"
                    return ComparisonDate(getLastDay(DateTime.Parse(dt + "-01")), prvDate);
            }
        }

        #region 日期队列补偿
        /// <summary>日期队列补偿
        /// ArrayList Item: string YYYY-MM-DD
        /// </summary>
        /// <param name="_bDate">System.DateTime:起算日期</param>
        /// <param name="_eDate">System.DateTime:截止日期</param>
        /// <returns>System.Collections.ArrayList item:YYYY-MM</returns>
        public static ArrayList DateArrayCompensate_Ds(DateTime _bDate, DateTime _eDate)
        {
            ArrayList _objal = new ArrayList();
            DateTime _tempDT = _bDate;
            while (_tempDT <= _eDate)
            {
                //yyyy-mm-dd
                _objal.Add(_tempDT.ToString("yyyy-MM-dd"));
                _tempDT = _tempDT.AddDays(1);
            }

            return _objal;
        }

        /// <summary>日期队列补偿
        /// ArrayList Item: string YYYY-MM
        /// </summary>
        /// <param name="_bDate">System.DateTime:起算日期</param>
        /// <param name="_eDate">System.DateTime:截止日期</param>
        /// <returns>System.Collections.ArrayList item:YYYY-MM</returns>
        public static ArrayList DateArrayCompensate_Ms(DateTime _bDate, DateTime _eDate)
        {

            ArrayList _objal = new ArrayList();
            DateTime _tempDT = _bDate;

            while (_tempDT <= _eDate)
            {
                //yyyy-mm
                _objal.Add(_tempDT.ToString("yyyy-MM-dd").Substring(0, 7));
                _tempDT = _tempDT.AddMonths(1);
            }

            return _objal;
        }

        /// <summary>日期队列补偿
        /// ArrayList Item: string YYYY-MM  +1次重载
        /// </summary>
        /// <param name="_bDate">System.String : YYYY-MM 起算日期</param>
        /// <param name="_eDate">System.String : YYYY-MM 截止日期</param>
        /// <returns>System.Collections.ArrayList item:YYYY-MM</returns>
        public static ArrayList DateArrayCompensate_Ms(string _bDate, string _eDate)
        {
            return DateArrayCompensate_Ms(DateTime.Parse(_bDate + "-01"), DateTime.Parse(_eDate + "-01"));
        }
        /// <summary>日期队列补偿
        /// ArrayList Item: string YYYY-MM
        /// </summary>
        /// <param name="_bDate">System.DateTime:起算日期</param>
        /// <param name="_eDate">System.DateTime:截止日期</param>
        /// <returns>System.Collections.ArrayList item:YYYY-MM</returns>
        public static string DateArrayCompensate_Mstr(DateTime _bDate, DateTime _eDate)
        {

            string _objal = string.Empty;
            DateTime _tempDT = _bDate;

            while (_tempDT <= _eDate)
            {
                //yyyy-mm
                _objal += _tempDT.ToString("yyyy-MM-dd").Substring(0, 7) + ",";
                _tempDT = _tempDT.AddMonths(1);
            }
            if (_objal.Length > 0)
            {
                _objal = _objal.TrimEnd(new char[] { ',' });
            }
            return _objal;
        }

        #endregion

        #region 转换
        /// <summary>将传入的时间转换为小数值[返回两位小数]
        /// 将传入日期的时间部分传换为数值
        /// </summary>
        /// <param name="_date">DateTime</param>
        /// <returns>Double(DateTime hh:mm:ss to Number)</returns>
        public static Double DateTimeToNumber(DateTime _date)
        {
            double _time = _date.Hour + Double.Parse(_date.Minute.ToString()) / 60 + Double.Parse(_date.Second.ToString()) / 3600;
            return Math.Round(_time, 2);
        }

        /// <summary>秒数转为小时数
        /// 将传入的秒数转为对应的小时数，并保留两位小数
        /// </summary>
        /// <param name="_s"></param>
        /// <returns>_s/3600,round:2</returns>
        public static Double SecondToHour(int _s)
        {
            double _hour = Double.Parse(_s.ToString()) / 3600;
            return Math.Round(_hour, 2);
        }
        #endregion

        #region Class Methot test
        private static void testCM()
        {
        }

        #endregion

        #region 业务 处理HashTable
        /// <summary>
        /// 处理已封装入HashTable的条件，年-月处理为年-月-日，并对起始日期以当天为条件收缩
        /// </summary>
        /// <param name="htFilter">已封装入HashTable的条件</param>
        /// <param name="bKey">起始日期的KEY</param>
        /// <param name="eKey">截止日期的KEY</param>
        /// <returns>处理完毕的HashTable</returns>
        public static Hashtable FilterHashDate(Hashtable htFilter, string bKey, string eKey)
        {
            htFilter[bKey] = htFilter[bKey].ToString().Split('-').Length == 2 ? ComparisonDateNow(DateTime.Parse(htFilter[bKey].ToString() + "-01")).ToString("yyyy-MM-dd") : ComparisonDateNow(DateTime.Parse(htFilter[bKey].ToString())).ToString("yyyy-MM-dd");
            htFilter[eKey] = htFilter[eKey].ToString().Split('-').Length == 2 ? ComparisonDateNow(htFilter[eKey].ToString()).ToString("yyyy-MM-dd") : ComparisonDateNow(DateTime.Parse(htFilter[eKey].ToString())).ToString("yyyy-MM-dd");
            return htFilter;
        }
        #endregion
    }
}
