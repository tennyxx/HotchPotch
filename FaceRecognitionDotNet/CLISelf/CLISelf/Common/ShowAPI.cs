using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Face.CLISelf.Common
{
    public class ReflectModel
    {

        /// <summary>
        　　/// 控制器名称
        　　/// </summary>
        public string ControllerName { set; get; }

        /// <summary>
        　　/// action介绍
        　　/// </summary>
        public List<ReflectActionModel> ActionDesction { set; get; }
    }
    public class ReflectActionModel
    {
        /// 接口请求方式 
        /// </summary> 
        public string ActionType { set; get; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string ActionLink { set; get; }

        /// <summary> 
        /// 接口请求的数据
        /// </summary> 
        public List<ReflectData> ActionData { set; get; }

        /// 接口名称 
        /// </summary> 
        public string ActionName { set; get; }
    }
    public class ReflectData
    {
        /// <summary>
        /// 数据字段名
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 字段简介
        /// </summary>
        public string des { set; get; }
    }
    public class ShowAPI
    {
        /// <summary>
        /// 显示当前控制器的所有开发API
        /// </summary>
        /// <param name="apiController"></param>
        /// <returns></returns>
        public static string ShowWebApi(ApiController apiController) {
            List<ReflectModel> controls = new List<ReflectModel>();
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.Collections.Generic.List<Type> typeList = new List<Type>();
            var types = asm.GetTypes().Where(type => typeof(IHttpController).IsAssignableFrom(type));
            foreach (Type type in types)
            {
                string s = type.FullName.ToLower();
                typeList.Add(type);
            }
            typeList.Sort(delegate (Type type1, Type type2) { return type1.FullName.CompareTo(type2.FullName); });
            foreach (Type type in typeList)
            {
                ReflectModel rm = new ReflectModel();
                rm.ActionDesction = new List<ReflectActionModel>();

                string controller = type.Name.Replace("Controller", "");
                rm.ControllerName = controller;
                System.Reflection.MethodInfo[] memthods = type.GetMethods(
                    System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var m in memthods)
                {
                    if (m.DeclaringType.Attributes.HasFlag(System.Reflection.TypeAttributes.Public) != true)
                        continue;
                    ReflectActionModel ram = new ReflectActionModel();
                    string action = m.Name;
                    if (action.Contains("<") || action.Contains(">")) continue;
                    ram.ActionName = action;
                    foreach (var item in m.CustomAttributes)
                    {
                        string nn = item.AttributeType.Name;
                        if (nn.StartsWith("Http"))
                        {
                            nn = nn.Remove(0, 4).Replace("Attribute", "");
                            if (string.IsNullOrEmpty(ram.ActionType))
                            {
                                ram.ActionType += nn;
                            }
                            else
                            {
                                ram.ActionType += "," + nn;
                            }
                        }
                    }

                    ram.ActionLink = controller + "/" + action;
                    ParameterInfo[] ps = m.GetParameters();
                    List<ReflectData> DataList = new List<ReflectData>();
                    foreach (var par in ps)
                    {
                        DataList.Add(new ReflectData { name = par.Name, des = par.ParameterType.ToString() });
                    }
                    ram.ActionData = DataList;
                    rm.ActionDesction.Add(ram);
                }
                controls.Add(rm);
            }
            //return controls;
            string html = "";
            foreach (var item in apiController.Configuration.Routes)
            {
                if (!string.IsNullOrEmpty(item.RouteTemplate))
                {
                    html += "Route:" + item.RouteTemplate + "<br/>";
                }
            }
            html += "<br/>";
            html += DateTime.Now.ToString();
            html += "<br/>";
            foreach (var item in controls)
            {
                html += "--------------------------------------<br/>";
                html += "Controller:" + item.ControllerName + "<br/>";
                html += "-----------------<br/>";
                foreach (var res in item.ActionDesction)
                {
                    html += "Url:" + res.ActionLink + "  (" + res.ActionType + ")<br/>";
                    foreach (var pro in res.ActionData)
                    {
                        html += "&nbsp;&nbsp;Parameter Name:" + pro.name + "(" + pro.des + ")<br/>";
                    }
                    html += "-----------------<br/>";
                }
            }
            html = "<!DOCTYPE html><html lang=\"zh-CN\" ><head><title>接口清单列表</title><style>*{font-size: 12px;}</style></head><body> " + html + "</body></html>";
            return html;
        }
    }
}
