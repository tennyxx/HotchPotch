using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Face.CLISelf.Controllers
{
    /// <summary>
    /// 重写实现处理授权失败时返回json,避免跳转登录页
    /// </summary>
    public class TokenAuthorize : AuthorizeAttribute
    {
        public static string TokenKey = System.Configuration.ConfigurationManager.AppSettings["TokenKey"].ToString();
        public TokenAuthorize():base() {
            
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            List<string> str = new List<string>();
            IEnumerable<string> methodOverrideHeader;
            if (actionContext.Request.Headers.TryGetValues("Token", out methodOverrideHeader))
            {
                string token = methodOverrideHeader.First();
                if (token != TokenKey)
                {
                    var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                    response.StatusCode = HttpStatusCode.Forbidden;
                    //response.Content = new StringContent("你没有权限", Encoding.UTF8, "text/html");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Forbidden;
                //response.Content = new StringContent("你没有权限", Encoding.UTF8, "text/html");
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            var response = actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
            response.Content = new StringContent("你没有权限", Encoding.UTF8, "text/html");
        }
    }
}