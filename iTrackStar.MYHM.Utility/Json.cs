using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using Newtonsoft.Json;
using System.IO;

namespace iTrackStar.MYHM.Utility
{
    #region Json处理
    /// <summary>
    /// 在服务器端生成JSON格式的文档,方便客户端Javascript解析
    /// </summary>
    public class Json
    {
        /// <summary>
        /// 操作成功标记
        /// </summary>
        public bool success
        {
            get
            {
                return _success;
            }
            set
            {
                //如设置为true,则清空error
                if (success) _error = string.Empty;
                _success = value;
            }
        }

        /// <summary>
        /// 操作失败时的错误原因
        /// </summary>
        public string error
        {
            get
            {
                return _error;
            }
            set
            {
                //如果设置error,则自动设置success为false
                if (value != "") _success = false;
                _error = value;
            }
        }

        /// <summary>
        /// 返回简单信息
        /// </summary>
        public string singleInfo = string.Empty;

        protected bool _success = true;
        protected string _error = string.Empty;
        protected System.Collections.ArrayList arrData = new ArrayList();
        protected System.Collections.ArrayList arrDataItem = new ArrayList();

        public Json()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 每次新生成一个json对象时必须重置
        /// </summary>
        public void Reset()
        {
            _success = true;
            _error = string.Empty;
            singleInfo = string.Empty;
            arrData.Clear();
            arrDataItem.Clear();
        }

        /// <summary>
        /// json对象的名/值对
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_value"></param>
        public void AddItem(string name, string _value)
        {
            arrDataItem.Add(name);
            arrDataItem.Add(_value);
        }

        /// <summary>
        /// 添加data数组
        /// </summary>
        public void ItemOk()
        {
            arrData.Add(arrDataItem);
            arrDataItem = new ArrayList();
        }

        /// <summary>
        /// 序列化JSON对象，返回JSON代码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("success:" + _success.ToString().ToLower() + ",");
            sb.Append("error:\"" + _error.Replace("\"", "\\\"") + "\",");
            sb.Append("singleInfo:\"" + singleInfo.Replace("\"", "\\\"") + "\",");
            sb.Append("data:[");
            for (int i = 0; i < arrData.Count; i++)
            {
                ArrayList arr = (ArrayList)arrData[i];
                sb.Append("{");
                for (int j = 0; j < arr.Count; j += 2)
                {
                    if (j == arr.Count) break;
                    sb.Append((string)arr[j]);
                    sb.Append(":");
                    sb.Append("\"");
                    sb.Append(arr[j + 1].ToString().Replace("\"", "\\\""));
                    sb.Append("\"");
                    if (j < arr.Count - 2) sb.Append(",");
                }
                sb.Append("}");
                if (i < arrData.Count - 1) sb.Append(",");
            }
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// json转hashtable
        /// </summary>
        /// <param name="json">string</param>
        /// <returns></returns>
        public static Hashtable Json2Hashtable(string json)
        {
            Hashtable ht = new Hashtable();

            if (json.TrimStart('[').TrimEnd(']') != "")
            {
                string[] arr = json.TrimStart('[').TrimEnd(']').Split(',');
                foreach (string clos in arr)
                {
                    string[] strClos = clos.Split('=');
                    if (strClos.Length == 2)
                    {
                        ht.Add(strClos[0], strClos[1]);
                    }
                }
            }
            return ht;
        }
    }

    #endregion

    #region JsonHelper
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 序列化数据为Json数据格式.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            Type type = value.GetType();
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
            json.NullValueHandling = NullValueHandling.Ignore;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            if (type == typeof(DataRow))
                json.Converters.Add(new DataRowConverter());
            else if (type == typeof(DataTable))
                json.Converters.Add(new DataTableConverter());
            else if (type == typeof(DataSet))
                json.Converters.Add(new DataSetConverter());
            StringWriter sw = new StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);

            //writer.Formatting = Formatting.Indented;
            writer.Formatting = Formatting.None;
            writer.QuoteChar = '"';
            json.Serialize(writer, value);
            string output = sw.ToString();
            writer.Close();
            sw.Close();
            return output;
        }

        /// <summary>
        /// 反序列化Json数据格式.
        /// </summary>
        /// <param name="jsonText">The json text.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns></returns>
        public static object Deserialize(string jsonText, Type valueType)
        {
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
            json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            StringReader sr = new StringReader(jsonText);
            Newtonsoft.Json.JsonTextReader reader = new JsonTextReader(sr);
            object result = json.Deserialize(reader, valueType);
            reader.Close();
            return result;
        }

        /// <summary>
        /// 遍历DataTable的行和列生成Json，可控制性较差
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append(dt.TableName);
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 遍历DataTable的行和列生成Json，控制数据列，专门为FlexiGrid提供便捷的数据源生成
        /// </summary>
        /// <param name="dt">DataTable对象</param>
        /// <param name="cols">Json中的数据列</param>
        /// <returns></returns>
        public static string Json4FlexiGrid(DataTable dt, string cols)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string[] colarr = cols.Split(',');
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"id\":");
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Rows[i]["ID"].ToString());
                    jsonBuilder.Append("\",\"cell\":[");
                    foreach (string col in colarr)
                    {
                        jsonBuilder.Append("\"");
                        jsonBuilder.Append(dt.Rows[i][col].ToString());
                        jsonBuilder.Append("\",");
                    }
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("]},");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// json转hashtable
        /// </summary>
        /// <param name="json">string</param>
        /// <returns></returns>
        public static Hashtable Json2Hashtable(string json)
        {
            Hashtable ht = new Hashtable();

            if (json.TrimStart('[').TrimEnd(']') != "")
            {
                string[] arr = json.TrimStart('[').TrimEnd(']').Split(',');
                foreach (string clos in arr)
                {
                    string[] strClos = clos.Split('=');
                    if (strClos.Length == 2)
                    {
                        ht.Add(strClos[0], strClos[1]);
                    }
                }
            }
            return ht;
        }
    }

    #endregion

    #region 常用JSON转换

    //-------------------------------------------------------------------------------//
    /// <summary>
    /// Converts a <see cref="DataRow"/> object to and from JSON.
    /// </summary>
    public class DataRowConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataRow">The data row.</param>
        /// <param name="ser">The JsonSerializer 对象.</param>
        public override void WriteJson(JsonWriter writer, object dataRow, JsonSerializer ser)
        {
            DataRow row = dataRow as DataRow;
            writer.WriteStartObject();
            foreach (DataColumn column in row.Table.Columns)
            {
                writer.WritePropertyName(column.ColumnName);
                ser.Serialize(writer, row[column]);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataRow).IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object obj, JsonSerializer ser)
        {
            throw new NotImplementedException();
        }
    }


    //-------------------------------------------------------------------------//
    /// <summary>
    /// Converts a DataTable to JSON. Note no support for deserialization
    /// </summary>
    public class DataTableConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataTable">The data table.</param>
        /// <param name="ser">The JsonSerializer Object.</param>
        public override void WriteJson(JsonWriter writer, object dataTable, JsonSerializer ser)
        {
            DataTable table = dataTable as DataTable;
            DataRowConverter converter = new DataRowConverter();
            writer.WriteStartObject();
            writer.WritePropertyName("Rows");
            writer.WriteStartArray();
            foreach (DataRow row in table.Rows)
            {
                converter.WriteJson(writer, row, ser);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataTable).IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object obj, JsonSerializer ser)
        {
            throw new NotImplementedException();
        }
    }

    //-------------------------------------------------------------------------//
    /// <summary>
    /// Converts a <see cref="DataSet"/> object to JSON. No support for reading.
    /// </summary>
    public class DataSetConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="ser">The JsonSerializer Object.</param>
        public override void WriteJson(JsonWriter writer, object dataset, JsonSerializer ser)
        {
            DataSet dataSet = dataset as DataSet;
            DataTableConverter converter = new DataTableConverter();
            writer.WriteStartObject();
            writer.WritePropertyName("Tables");
            writer.WriteStartArray();
            foreach (DataTable table in dataSet.Tables)
            {
                converter.WriteJson(writer, table, ser);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataSet).IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object obj, JsonSerializer ser)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
    