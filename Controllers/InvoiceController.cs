using JackBallers.Api.Models;
using JackBallers.Api.Strike;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace JackBallers.Api.Controllers;

[Route("api/invoice")]
public class InvoiceController : ApiBaseController
{
    private readonly IDatabase _database;

    public InvoiceController(IDatabase database)
    {
        _database = database;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoiceForItem([FromServices]PartnerApi api, Guid id)
    {
        var item = await _database.GetJson<CollectionItem>(CollectionItem.FormatKey(id));
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
                Currency = Currencies.USD,
                Amount = item.FiatPrice
            },
            Description = item?.Name
        };
        var invoice = await api.GenerateInvoice(invoiceRequest);
        var quote = await api.GetInvoiceQuote(invoice.InvoiceId);
        
        var itemInvoice = new ItemInvoice(invoiceId, id)
        {
            Invoice = invoice,
            Quote = quote
        };
        await itemInvoice.Save(_database);

        return Json(itemInvoice);
    }
}