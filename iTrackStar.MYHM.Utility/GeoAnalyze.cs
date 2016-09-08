using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Net;

namespace iTrackStar.MYHM.Utility
{
    public class GeoAnalyze
    {
        /// <summary>
        /// 根据经纬度获取详细地理位置
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public string GetGeoDetailInfo(string longitude, string latitude)
        {
            string returnVal = string.Empty;
            try
            {
                string strKey = "08dce414fcd1f8d0b11c51c38f8449014f8b528494940e534aedd3f6d9bb77c6322f6641dbb7432a";
                StringBuilder strUrl = new StringBuilder();
                strUrl.AppendFormat("http://mapabc.zlzk.com:8081/rgeocode/simple?key={2}&resType=json&encode=utf-8&range=1000&roadnum=3&crossnum=2&poinum=1&retvalue=1&sid=7001&region={0},{1}&rid=967188", longitude, latitude, strKey);
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(strUrl.ToString());
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                string mystr = streamRead.ReadToEnd().ToLower();
                string[] str1 = mystr.Split('=')[1].Split('[');
                string json1 = "";
                for (int i = 1; i < str1.Length; i++)
                {
                    json1 += str1[i] + "[";
                }
                string strjson2 = json1.Substring(0, json1.Length - 5);
                MemoryStream ms0 = new MemoryStream(Encoding.UTF8.GetBytes(strjson2));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MapABCObject));
                MapABCObject mapABCResult = (MapABCObject)ser.ReadObject(ms0);

                StringBuilder sbResult = new StringBuilder();
                sbResult.Append(mapABCResult.province.name);
                sbResult.Append(mapABCResult.city.name);
                sbResult.Append(mapABCResult.district.name);
                if (mapABCResult.roadlist.Length < 1)
                {
                    if (mapABCResult.poilist.Length > 0)
                    {
                        sbResult.Append(mapABCResult.poilist[0].name);
                    }
                }
                else
                {
                    sbResult.Append(mapABCResult.roadlist[0].name);
                    string direction = getDirectionbyKey(mapABCResult.roadlist[0].direction);
                    sbResult.Append(direction);
                    sbResult.Append(Convert.ToDouble(mapABCResult.roadlist[0].distance).ToString("N2"));//四舍五入保留两位小数
                    sbResult.Append("m");
                }
                returnVal = sbResult.ToString();
            }
            catch
            { }

            return returnVal;
        }

        public string getDirectionbyKey(string key) {
            switch (key) {
                case "north":
                    return "正北方";
                case "east":
                    return "正东方";
                case "south":
                    return "正南方";
                case "west":
                    return "正西方";
                case "eastnorth":
                    return "东北方";
                case "eastsouth":
                    return "东南方";
                case "westsouth":
                    return "西南方";
                case "westnorth":
                    return "西北方";
                case "northeast":
                    return "东偏北"; 
                case "southeast":
                    return "东偏南";
                case "southwest":
                    return "西偏南";
                case "northwest":
                    return "西偏北";
                default :
                    return "未知方向";
            }
        }
    }

    [DataContract]
    class MapABCObject
    {
        [DataMember]
        public crossPoiList[] crosslist { get; set; }
        [DataMember]
        public poiList[] poilist { get; set; }
        [DataMember]
        public Province province { get; set; }
        [DataMember]
        public City city { get; set; }
        [DataMember]
        public District district { get; set; }
        [DataMember]
        public roadList[] roadlist { get; set; }
    }

    [DataContract]
    class City
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string citycode { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string enname { get; set; }
        [DataMember]
        public string tel { get; set; }
    }
    [DataContract]
    class Province
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string enname { get; set; }

    }
    [DataContract]
    class crossPoiList
    {
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public Road1 road1 { get; set; }
        [DataMember]
        public Road2 road2 { get; set; }
        [DataMember]
        public string x { get; set; }
        [DataMember]
        public string y { get; set; }

    }
    [DataContract]
    class Road1
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string level { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string enname { get; set; }
    }
    [DataContract]
    class Road2
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string level { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string enname { get; set; }
    }
    [DataContract]
    class District
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string enname { get; set; }
    }
    [DataContract]
    class poiList
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string x { get; set; }
        [DataMember]
        public string y { get; set; }
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string typecode { get; set; }
        [DataMember]
        public string pguid { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string tel { get; set; }
        [DataMember]
        public string type { get; set; }

    }
    [DataContract]
    class roadList
    {
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string ename { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string level { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string x { get; set; }
        [DataMember]
        public string y { get; set; }
    }

    
}
