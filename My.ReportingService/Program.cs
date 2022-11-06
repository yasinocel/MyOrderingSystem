
using My.Domain.Models;
using My.ReportingService;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

new ReportingService().Execute((Report report) =>
{
    Console.WriteLine(
             $"[Report Status: {report!.Status}] => " +
             $"[Created Date: {report.CreatedDate}," +
             $" Report Id :{report.Id}," +
             $" Report Details: {report.Details}]");
});

