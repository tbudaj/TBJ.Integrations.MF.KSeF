using TBJ.Integrations.MF.KSeF;
using TBJ.Integrations.MF.KSeF.Abstractions;
using TBJ.Integrations.MF.KSeF.Models.Sessions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKSeFApi(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TBJ.Integrations.MF.KSeF WebApi",
        Version = "v1",
        Description = "Przykładowe API demonstrujące użycie pakietu TBJ.Integrations.MF.KSeF."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// <summary>Otwiera sesję online KSeF dla wskazanego NIP.</summary>
app.MapPost("/api/sessions/online", async (ISessionsClient sessions, OpenSessionRequest request, string? accessToken) =>
{
    var session = await sessions.OpenOnlineSessionAsync(request, accessToken);
    return Results.Ok(session);
})
.WithName("OpenOnlineSession")
.WithOpenApi();

/// <summary>Wysyła fakturę w sesji online.</summary>
app.MapPost("/api/sessions/online/{sessionId}/invoices", async (ISessionsClient sessions, string sessionId, IFormFile invoice, string? accessToken) =>
{
    await using var stream = invoice.OpenReadStream();
    var result = await sessions.SendInvoiceAsync(
        sessionId,
        stream,
        new InvoiceMetadata { Format = "FA_V2" },
        accessToken);
    return Results.Ok(result);
})
.WithName("SendInvoice")
.WithOpenApi();

/// <summary>Wyszukuje faktury w KSeF.</summary>
app.MapGet("/api/invoices", async (IInvoicesClient invoices, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, string? accessToken) =>
{
    var page = await invoices.QueryInvoicesAsync(
        dateFrom: dateFrom,
        dateTo: dateTo,
        accessToken: accessToken);
    return Results.Ok(page);
})
.WithName("QueryInvoices")
.WithOpenApi();

app.Run();
