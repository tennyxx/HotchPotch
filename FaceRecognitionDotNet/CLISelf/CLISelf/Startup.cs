using Newtonsoft.Json.Serialization;
using Owin;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Face.CLISelf
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new {  controller = "Home", action = "ApiInfo", id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true };
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "json", "application/json"));
            app.UseWebApi(config);
        }
    }
}
