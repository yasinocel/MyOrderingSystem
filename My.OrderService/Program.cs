using My.OrderService;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

await new OrderService().Execute();