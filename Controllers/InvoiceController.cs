using System.Globalization;
using JackBallers.Api.Models;
using JackBallers.Api.Strike;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JackBallers.Api.Controllers;

[Route("api/invoice")]
public class InvoiceController : ApiBaseController
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly IDatabase _database;
    private readonly JackBallersConfig _config;

    public InvoiceController(IDatabase database, JackBallersConfig config, ILogger<InvoiceController> logger)
    {
        _database = database;
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoiceForItem([FromServices]PartnerApi api, [FromQuery]Guid itemId)
    {
        var item = await _database.GetJson<CollectionItem>(CollectionItem.FormatKey(itemId));
        if (item == default)
        {
            return NotFound();
        }

        var invoiceId = Guid.NewGuid();
        var invoiceRequest = new CreateInvoiceRequest()
        {
            CorrelationId = invoiceId.ToString(),
            Amount = new()
            {
                Currency = _config.ReceiverCurrency ?? Currencies.USD,
                Amount = Math.Round(item.FiatPrice, 2).ToString(CultureInfo.InvariantCulture)
            },
            Description = $"{item?.Name} #{item?.Number}",
            Handle = _config.ReceiverHandle
        };
        var invoice = await api.GenerateInvoice(invoiceRequest);
        var quote = await api.GetInvoiceQuote(invoice.InvoiceId);
        
        var itemInvoice = new ItemInvoice(invoiceId, itemId)
        {
            Invoice = invoice,
            Quote = quote
        };
        await itemInvoice.Save(_database);

        return Json(itemInvoice);
    }

    [Route("{invoiceId:guid}/wait")]
    [HttpGet]
    public async Task<IActionResult> WaitForPayment([FromServices] PartnerApi api, [FromRoute] Guid invoiceId)
    {
        var itemInvoice = await _database.GetJson<ItemInvoice>(ItemInvoice.FormatKey(invoiceId));
        if (itemInvoice?.Invoice == default)
        {
            return NotFound();
        }

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            var invoiceStatus = await api.GetInvoice(itemInvoice.Invoice.InvoiceId);
            itemInvoice.Invoice = invoiceStatus;
            await itemInvoice.Save(_database);

            //test
            //invoiceStatus.State = InvoiceState.PAID;
            
            if (invoiceStatus.State != InvoiceState.PAID
                && itemInvoice.Quote?.Expiration > DateTimeOffset.UtcNow)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            else
            {
                break;
            }
        }

        if (itemInvoice.Invoice.State == InvoiceState.PAID)
        {
            //remove from collection
            var item = await _database.GetJson<CollectionItem>(CollectionItem.FormatKey(itemInvoice.ItemId));
            if (item != default)
            {
                _logger.LogInformation("Sold item '{name} #{number}'", item.Name, item.Number);
                await _database.SetRemoveAsync(Collection.ItemsKey(item.CollectionId), item.Id.ToString());
            }
        }
        return Content(JsonConvert.SerializeObject(itemInvoice), "application/json");
    }
}