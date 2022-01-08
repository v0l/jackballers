using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JackBallers.Api.Strike;

public class PartnerApi
{
    private readonly HttpClient _client;
    private readonly PartnerApiSettings _settings;

    public PartnerApi(PartnerApiSettings settings)
    {
        _client = new HttpClient
        {
            BaseAddress = settings.Uri ?? new Uri("https://api.strike.me/")
        };
        _settings = settings;

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
    }

    public Task<Invoice> GenerateInvoice(CreateInvoiceRequest invoiceRequest)
    {
        var path = !string.IsNullOrEmpty(invoiceRequest.Handle)
            ? $"/v1/invoices/handle/{invoiceRequest.Handle}"
            : "/v1/invoices";
        return SendRequest<Invoice>(HttpMethod.Post, path, invoiceRequest);
    }

    public Task<Invoice> GetInvoice(Guid id)
    {
        return SendRequest<Invoice>(HttpMethod.Get, $"/v1/invoices/{id}");
    }

    public Task<InvoiceQuote> GetInvoiceQuote(Guid id)
    {
        return SendRequest<InvoiceQuote>(HttpMethod.Post, $"/v1/invoices/{id}/quote");
    }
    
    private async Task<TReturn> SendRequest<TReturn>(HttpMethod method, string path, object? bodyObj = default)
        where TReturn : class
    {
        var request = new HttpRequestMessage(method, path);
        if (bodyObj != default)
            request.Content = new StringContent(JsonSerializer.Serialize(bodyObj), Encoding.UTF8, "application/json");

        var rsp = await _client.SendAsync(request);
        var okResponse = method.Method switch
        {
            "POST" => HttpStatusCode.Accepted,
            _ => HttpStatusCode.OK
        };

        if (rsp.StatusCode != okResponse) throw new ErrorResponse();
        var inv = JsonSerializer.Deserialize<TReturn>(await rsp.Content.ReadAsStreamAsync());
        if (inv != default) return inv;

        throw new ErrorResponse();
    }
}

public class InvoiceQuote
{
    [JsonPropertyName("quoteId")]
    public Guid QuoteId { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("lnInvoice")]
    public string? LnInvoice { get; init; }
    
    [JsonPropertyName("onchainAddress")]
    public string? OnChainAddress { get; init; }
    
    [JsonPropertyName("expiration")]
    public DateTimeOffset Expiration { get; init; }
    
    [JsonPropertyName("expirationInSec")]
    public ulong ExpirationSec { get; init; }
    
    [JsonPropertyName("targetAmount")]
    public CurrencyAmount? TargetAmount { get; init; }
    
    [JsonPropertyName("sourceAmount")]
    public CurrencyAmount? SourceAmount { get; init; }
    
    [JsonPropertyName("conversionRate")]
    public ConversionRate? ConversionRate { get; init; }
}

public class ConversionRate
{
    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Amount { get; init; }
    
    [JsonPropertyName("sourceCurrency")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currencies Source { get; init; }
    
    [JsonPropertyName("targetCurrency")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currencies Target { get; init; }
}

public class ErrorResponse : Exception
{
}

public class CreateInvoiceRequest
{
    public string? CorrelationId { get; init; }
    public string? Description { get; init; }
    public CurrencyAmount? Amount { get; init; }
    public string? Handle { get; init; }
}

public class CurrencyAmount
{
    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Currencies? Currency { get; init; }
}

public enum Currencies
{
    BTC,
    USD,
    EUR,
    GBP,
    USDT
}

public class Invoice
{
    [JsonPropertyName("invoiceId")]
    public Guid InvoiceId { get; init; }
    
    [JsonPropertyName("amount")]
    public CurrencyAmount? Amount { get; init; }

    [JsonPropertyName("state")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InvoiceState State { get; init; }

    [JsonPropertyName("created")]
    public DateTimeOffset? Created { get; init; }
    
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("issuerId")]
    public string? IssuerId { get; init; }
    
    [JsonPropertyName("receiverId")]
    public string? ReceiverId { get; init; }
    
    [JsonPropertyName("payerId")]
    public string? PayerId { get; init; }
}

public enum InvoiceState
{
    UNPAID,
    PENDING,
    PAID,
    CANCELLED
}

public class PartnerApiSettings
{
    public Uri? Uri { get; init; }
    public string ApiKey { get; init; }
}