using Newtonsoft.Json;
using System.Xml.Linq;

namespace Common.Service
{
    public static class NewtonsoftHelper
    {
        /// <summary>
        /// Json格式字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T JConvertObj<T>(string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        /// <summary>
        /// 对象转换成json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OConvertJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// xml字符串转换成json字符串
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static string XConvertJson(string xmlStr)
        {
            return JsonConvert.SerializeXNode(XDocument.Load(xmlStr));
        }

        /// <summary>
        /// Xml字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static T XConvertObj<T>(string xmlStr)
        {
            return JConvertObj<T>(XConvertJson(xmlStr));
        }

        /// <summary>
        /// json字符串转换成xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static string JConvertXml(string Json)
        {
            //判断是否是数组json
            if (Json.StartsWith("["))
                Json = @"{""xmlArray"":" + Json + @"}";
            //  DeserializeXNode 第一个参数是json字符串 第二个参数是转换成xml的头文件
            return JsonConvert.DeserializeXNode(Json, "xmlMessage").ToString();
        }

        /// <summary>
        /// 对象转换成xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OConvertXml<T>(T value)
        {
            return JConvertXml(OConvertJson(value));
        }
    }
}
