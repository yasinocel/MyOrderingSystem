
using My.Domain.Models;
using My.PaymentService;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

await new PaymentSerivce().Execute();