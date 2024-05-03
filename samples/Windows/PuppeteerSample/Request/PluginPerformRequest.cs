using Tenon.Puppeteer.Extensions.Models;

namespace PuppeteerSample.Request;

public class PluginPerformRequest<T> : PerformRequest<T>
{
    public override string GenerateScript()
    {
        return FunctionParameter != null
            ? "async()=>{const pluginResult =await tenon_call_plugin(\"" + FunctionName +
              "\"," + Serializer.SerializeObject(FunctionParameter) + ");return pluginResult;}"
            : "async()=>{const pluginResult =await tenon_call_plugin(\"" + FunctionName +
              "\");return pluginResult;}";
    }
}