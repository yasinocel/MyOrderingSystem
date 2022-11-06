
using My.DispatchService;
using My.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

await new DispatchService().Execute();