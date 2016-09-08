using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Configuration;


namespace iTrackStar.MYHM.Utility
{
    /// <summary>
    /// 功能描述：资源管理类 用于读取相应语言的资源文本
    /// 创建标识：汪桂洋 2012-02-02
    /// 修改标识：
    /// 修改描述：
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// 站点默认语言
        /// </summary>
        private static string defaultLanguage;

        /// <summary>
        /// 获取站点当前语言名称
        /// </summary>
        public static string SiteLanguageName
        {
            get
            {
                return GetSupportedLanguage(defaultLanguage);
            }
        }
        /// <summary>
        /// 获取或设置站点当前语言的键值
        /// </summary>
        public static string SiteLanguageKey
        {
            get
            {
                return defaultLanguage;
            }
            set
            {
                ResourceManager.defaultLanguage = value;
            }
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        enum ResourceManagerType
        {
            String,
            ErrorMessage,
            Template
        }

        /// <summary>
        /// 静态构造,用于实例化上下文引用对象
        /// </summary>
        static ResourceManager()
        {

            defaultLanguage = ConfigurationManager.AppSettings["defaultLanguage"];
        }

        /// <summary>
        /// 从语言声明文件中获取支持的所有语言
        /// </summary>
        /// <returns>支持的所有语言的名称-值对</returns>
        public static NameValueCollection GetSupportedLanguages()
        {
            //CSContext csContext = CSContext.Current;
            HttpContext csContext = HttpContext.Current;
            string cacheKey = "SupportedLanguages";

            NameValueCollection supportedLanguages = CustomCache.Get(cacheKey) as NameValueCollection;
            if (supportedLanguages == null)
            {
                string filePath = csContext.Server.MapPath("~/Languages/languages.xml");
                CacheDependency dp = new CacheDependency(filePath);
                supportedLanguages = new NameValueCollection();

                XmlDocument d = new XmlDocument();
                d.Load(filePath);

                foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        supportedLanguages.Add(n.Attributes["key"].Value, n.Attributes["name"].Value);
                    }
                }
                CustomCache.Max(cacheKey, supportedLanguages, dp);
            }
            return supportedLanguages;
        }

        /// <summary>
        /// 获取语言字符集
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection GetLanguageCharsets()
        {
            HttpContext csContext = HttpContext.Current;
            string cacheKey = "LanguageCharsets";

            NameValueCollection languageCharsets = CustomCache.Get(cacheKey) as NameValueCollection;
            if (languageCharsets == null)
            {
                string filePath = csContext.Server.MapPath("~/Languages/languages.xml");
                CacheDependency dp = new CacheDependency(filePath);
                languageCharsets = new NameValueCollection();

                XmlDocument d = new XmlDocument();
                d.Load(filePath);

                foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        if (n.Attributes["emailCharset"] == null)
                            continue;
                        string codepage = n.Attributes["emailCharset"].Value;
                        if (n.Attributes["emailSubjectCharset"] != null)
                            codepage += "," + n.Attributes["emailSubjectCharset"].Value;

                        languageCharsets.Add(n.Attributes["key"].Value, codepage);
                    }
                }
                CustomCache.Max(cacheKey, languageCharsets, dp);
            }
            return languageCharsets;
        }

        /// <summary>
        /// 根据语言名称获取语言值
        /// 如果不能获取,则使用默认语言
        /// </summary>
        /// <param name="language">语言名称</param>
        /// <returns>语言值</returns>
        public static string GetSupportedLanguage(string language)
        {
            string defaultLanguage = ResourceManager.defaultLanguage; //默认语言
            NameValueCollection supportedLanguages = GetSupportedLanguages();
            string supportedLanguage = supportedLanguages[language];

            if (!string.IsNullOrEmpty(supportedLanguage))
            {
                return language;
            }
            else
            {
                return defaultLanguage;
            }
        }

        /// <summary>
        /// 根据资源名称从资源文件中获取相应资源文本
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns>资源文本</returns>
        public static string GetString(string name)
        {
            return GetString(name, false);
        }

        /// <summary>
        /// 根据资源名称从资源文件中获取相应资源文本
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="defaultOnly">只选择默认语言</param>
        /// <returns>资源文本</returns>
        public static string GetString(string name, bool defaultOnly)
        {
            return GetString(name, "Resources.xml", defaultOnly);
        }

        /// <summary>
        /// 根据资源名称从资源文件中获取相应资源文本
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="fileName">资源文件名</param>
        /// <returns></returns>
        public static string GetString(string name, string fileName)
        {
            return GetString(name, fileName, false);
        }

        /// <summary>
        /// 根据资源名称从资源文件中获取相应资源文本
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="fileName">资源文件名</param>
        /// <param name="defaultOnly">使用默认语言</param>
        /// <returns>资源文本</returns>
        public static string GetString(string name, string fileName, bool defaultOnly)
        {
            HttpContext csContext = HttpContext.Current;
            //使用HashTable装载所有资源
            Hashtable resources = null;
            //用户语言
            //为演示方便使用Session对象保存,更好地解决方案是使用Profile
            string userLanguage;//csContext.Items["AspSession"];
            //if (csContext.Request.Cookies["userLanguage"] != null)
            //    userLanguage = csContext.Request.Cookies["userLanguage"].Value;
            //else
            //    userLanguage = null;
            if (csContext.Session["userLanguage"] != null)
            {
                userLanguage = csContext.Session["userLanguage"].ToString();
            }
            else
            {
                userLanguage = null;
            }
            if (fileName != null && fileName != "")
            {
                resources = GetResource(ResourceManagerType.String, userLanguage, fileName, defaultOnly);
            }
            else
            {
                resources = GetResource(ResourceManagerType.String, userLanguage, "Resources.xml", defaultOnly);
            }
            string text = resources[name] as string;

            //try the standard file if we passed a file that didnt have the key we were looking for
            if (text == null && fileName != null && fileName != "")
            {
                resources = GetResource(ResourceManagerType.String, userLanguage, "Resources.xml", false);
                text = resources[name] as string;
            }
            return text;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="resourceType">资源类型</param>
        /// <param name="userLanguage">用户语言</param>
        /// <param name="fileName">资源文件名</param>
        /// <param name="defaultOnly">只使用默认语言</param>
        /// <returns></returns>
        private static Hashtable GetResource(ResourceManagerType resourceType, string userLanguage, string fileName, bool defaultOnly)
        {
            string defaultLanguage = ResourceManager.defaultLanguage;
            string cacheKey = resourceType.ToString() + defaultLanguage + userLanguage + fileName;

            // 如果用户没有定制语言,则使用默认
            //
            if (string.IsNullOrEmpty(userLanguage) || defaultOnly)
                userLanguage = defaultLanguage;

            // 从缓存中获取资源
            //
            Hashtable resources = CustomCache.Get(cacheKey) as Hashtable;

            if (resources == null)
            {
                resources = new Hashtable();

                resources = LoadResource(resourceType, resources, defaultLanguage, cacheKey, fileName);

                // 如果用户设置了语言则加载用户语言资源
                //
                if (defaultLanguage != userLanguage)
                {
                    resources = LoadResource(resourceType, resources, userLanguage, cacheKey, fileName);
                }
            }
            return resources;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="target"></param>
        /// <param name="language"></param>
        /// <param name="cacheKey"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static Hashtable LoadResource(ResourceManagerType resourceType, Hashtable target, string language, string cacheKey, string fileName)
        {
            HttpContext csContext = HttpContext.Current;

            string filePath = csContext.Server.MapPath("~/Languages/" + language + "/" + fileName);

            //switch (resourceType)
            //{
            //    case ResourceManagerType.ErrorMessage:
            //        filePath = string.Format(filePath, "Messages.xml");
            //        break;

            //    default:
            //        filePath = string.Format(filePath, "Resources.xml");
            //        break;
            //}

            CacheDependency dp = new CacheDependency(filePath);

            XmlDocument d = new XmlDocument();
            try
            {
                d.Load(filePath);
            }
            catch
            {
                return target;
            }
            foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
            {
                if (n.NodeType != XmlNodeType.Comment)
                {
                    if (target[n.Attributes["name"].Value] == null)
                    {
                        target.Add(n.Attributes["name"].Value, n.InnerText);
                    }
                    else
                    {
                        target[n.Attributes["name"].Value] = n.InnerText;
                    }
                }
            }
            if (language == ResourceManager.defaultLanguage)
            {
                CustomCache.Max(cacheKey, target, dp);
            }
            else
            {
                CustomCache.Insert(cacheKey, target, dp, CustomCache.MinuteFactor * 5);
            }
            return target;
        }
    }
}
