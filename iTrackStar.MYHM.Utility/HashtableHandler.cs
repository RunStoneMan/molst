using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTrackStar.MYHM.Utility
{
    /// <summary>
    /// 功能描述：Hashtable处理类
    /// 创建标识：汪桂洋 2012-02-02
    /// 修改标识：
    /// 修改描述：
    /// </summary>
    public class HashtableHandler
    {
        /// <summary>
        /// 合并hash
        /// </summary>
        public static Hashtable MergeHash(Hashtable ht1, Hashtable ht2)
        {
            foreach (string key in ht2.Keys)
            {
                if (!ht1.ContainsKey(key))
                {
                    ht1.Add(key, ht2[key].ToString());
                }
            }
            return ht1;
        }

        /// <summary>
        /// 删除hashtable部分
        /// </summary>
        public static Hashtable TruncateHash(Hashtable ht1, Hashtable ht2)
        {
            Hashtable ht = new Hashtable();
            foreach (string key in ht1.Keys)
            {
                if (ht2.ContainsKey(key))
                {
                    ht.Add(key, ht1[key]);
                }
            }
            return ht;
        }

        /// <summary>
        ///根据Hashtable的key截断datatable列
        /// </summary>
        public static DataTable TruncateDataTable(DataTable dt, Hashtable ht)
        {
            ArrayList arrlist = new ArrayList();
            DataTable dst = new DataTable();
            foreach (DataColumn dc in dt.Columns)
            {
                if (ht.ContainsKey(dc.ColumnName.ToUpper()) || ht.ContainsKey(dc.ColumnName.ToLower()))
                {
                    dst.Columns.Add(dc.ColumnName.ToString(), dc.DataType);
                    arrlist.Add(dc.ColumnName.ToString());
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dst.NewRow();
                if (arrlist.Count > 0)
                {
                    for (int j = 0; j < arrlist.Count; j++)
                    {
                        string str = arrlist[j].ToString();
                        //dst.Rows[i][str] = dt.Rows[i][str];
                        dr[str] = dt.Rows[i][str];
                    }
                }
                dst.Rows.Add(dr);
            }
            return dst;
        }
    }
}
