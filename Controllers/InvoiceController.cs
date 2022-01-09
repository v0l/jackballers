using System.Globalization;
using JackBallers.Api.Models;
using JackBallers.Api.Strike;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace JackBallers.Api.Controllers;

[Route("api/invoice")]
public class InvoiceController : ApiBaseController
{
    private readonly IDatabase _database;
    private readonly JackBallersConfig _config;

    public InvoiceController(IDatabase database, JackBallersConfig config)
    {
        _database = database;
        _config = config;
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
            
            if (invoiceStatus.State != InvoiceState.PAID)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        return Json(itemInvoice);
    }
}