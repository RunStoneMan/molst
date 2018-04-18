using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Collections;

namespace iTrackStar.MYHM.Utility
{
    public class RuleSelector
    {
        private static XmlDocument Xd;
        private static Hashtable htRes;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename"></param>
        public RuleSelector(string filename)
        {
            htRes = GetResource(filename);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="target"></param>
        /// <param name="cacheKey"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static Hashtable LoadResource(Hashtable target, string cacheKey, string fileName)
        {
            HttpContext csContext = HttpContext.Current;

            string filePath = csContext.Server.MapPath("~/" + fileName);

            CacheDependency dp = new CacheDependency(filePath);

            Xd = new XmlDocument();
            try
            {
                Xd.Load(filePath);
            }
            catch
            {
                return target;
            }
            target = getRuleDateInfo(Xd);
            CustomCache.Max(cacheKey, target, dp);
            return target;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Hashtable GetResource(string fileName)
        {
            string cacheKey = fileName;

            // 从缓存中获取资源
            Hashtable resources = CustomCache.Get(cacheKey) as Hashtable;
            if (resources == null)
            {
                resources = new Hashtable();
                resources = LoadResource(resources, cacheKey, fileName);
            }
            return resources;
        }

        /// <summary>
        /// 获取xml文件节点数据
        /// </summary>
        /// <returns></returns>
        private static Hashtable getRuleDateInfo(XmlDocument d)
        {
            Hashtable htItems = new Hashtable();
            XmlNodeList objXNList = d.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < objXNList.Count; i++)
            {
                if (objXNList[i].NodeType == XmlNodeType.Comment)
                    continue;

                if (objXNList[i].Name == "Default")
                {
                    StringBuilder vals = new StringBuilder();

                    foreach (XmlNode n in objXNList[i].ChildNodes)
                    {
                        if (n.NodeType == XmlNodeType.Comment)
                            continue;

                        if (n.Name == "Page")
                        {
                            vals.Append(n.InnerText);
                            vals.Append("/");

                        }

                        if (n.Name == "Class")
                        {
                            vals.Append(n.InnerText);
                            vals.Append("/");
                        }
                        if (n.Name == "Pic")
                        {
                            vals.Append(n.InnerText);
                            vals.Append("/");
                        }
                        if (n.Name == "AlarmFilter")
                        {
                            vals.Append(n.InnerText);
                        }
                    }
                    htItems.Add(objXNList[i].Attributes["name"].Value, vals.ToString());
                }

                if (objXNList[i].Name == "model")
                {
                    StringBuilder vals = new StringBuilder();

                    foreach (XmlNode n in objXNList[i].ChildNodes)
                    {
                        if (n.NodeType == XmlNodeType.Comment)
                            continue;
                        if (n.Name == "Page")
                        {
                            vals.Append(n.InnerText);
                            vals.Append("/");
                        }

                        if (n.Name == "Class")
                        {
                            string cls = string.Empty;

                            foreach (XmlNode nn in n.ChildNodes)
                            {
                                if (string.Empty.Equals(cls))
                                {
                                    cls = nn.Name + "," + nn.InnerText.Trim();
                                }
                                else
                                {
                                    cls += "#" + nn.Name + "," + nn.InnerText.Trim();
                                }
                            }
                            vals.Append(cls);

                            vals.Append("/");
                        }
                        if (n.Name == "Pic")
                        {
                            vals.Append(n.InnerText);
                            vals.Append("/");
                        }
                        if (n.Name == "AlarmFilter")
                        {
                            vals.Append(n.InnerText);
                        }
                    }
                    htItems.Add(objXNList[i].Attributes["name"].Value, vals.ToString());
                }
            }
            return htItems;
        }

        /// <summary>
        /// 格式化的方法，排除空值异常;
        /// </summary>
        /// <param name="_node"></param>
        /// <param name="_attr"></param>
        /// <returns></returns>
        private static string formatAttr(XmlNode _node, string _attr)
        {
            return _node.Attributes[_attr] == null ? "" : _node.Attributes[_attr].Value;
        }

        /// <summary>
        /// 获取特定节点属性(打印控件)
        /// </summary>
        /// <param name="CarType"></param>
        /// <returns></returns>
        public Hashtable htData(string CarType)
        {
            Hashtable htItems = new Hashtable();
            XmlNodeList objXNList = Xd.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < objXNList.Count; i++)
            {
                if (objXNList[i].NodeType == XmlNodeType.Comment)
                    continue;
                if (objXNList[i].Name == "modul")
                {
                    if (formatAttr(objXNList[i], "name").Trim() == CarType)
                    {
                        foreach (XmlNode n in objXNList[i].ChildNodes)
                        {
                            if (n.NodeType == XmlNodeType.Comment)
                                continue;
                            if (n.Name == "hidCols")
                            {
                                htItems.Add("hidCols", n.InnerText);
                            }
                            if (n.Name == "classname")
                            {
                                htItems.Add("classname", n.InnerText);
                            }
                            if (n.Name == "unitAuthorize")
                            {
                                htItems.Add("unitAuthorize", n.InnerText);
                            }
                            if (n.Name == "ColsName")
                            {
                                htItems.Add("ColsName", n.InnerText);
                            }
                            if (n.Name == "hidCols2")
                            {
                                htItems.Add("hidCols2", n.InnerText);
                            }
                            if (n.Name == "function")
                            {
                                htItems.Add("function", n.InnerText);
                            }
                            if (n.Name == "urlforprint")
                            {
                                htItems.Add("urlforprint", n.InnerText);
                            }
                            if (n.Name == "imglst")
                            {
                                foreach (XmlNode nd in n.ChildNodes)
                                {
                                    if (nd.Name == "val")
                                    {
                                        htItems.Add(formatAttr(nd, "name"), nd.InnerText.Trim());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return htItems;
        }
    }
}
