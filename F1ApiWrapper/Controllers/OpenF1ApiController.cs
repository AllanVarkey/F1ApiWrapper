using System.Globalization;
using System.Text.Json;
using AutoMapper;
using F1ApiWrapper.DTOs;
using F1ApiWrapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace F1ApiWrapper.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenF1ApiController(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    IMapper mapper) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public IActionResult Get()
    {
        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available for the current request.");

        return Ok(new
        {
            Message = "OpenF1Api controller is ready.",
            RequestId = httpContext.TraceIdentifier,
            Path = httpContext.Request.Path.Value
        });
    }

    [HttpGet("GetDriverInfo")]
    public async Task<ActionResult<IReadOnlyCollection<Driver>>> GetDriverInfo(
        [FromQuery] int driverNumber,
        CancellationToken cancellationToken)
    {
        if (driverNumber <= 0)
        {
            ModelState.AddModelError(nameof(driverNumber), "driverNumber must be greater than zero.");
            return ValidationProblem(ModelState);
        }

        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available for the current request.");

        var httpClient = httpClientFactory.CreateClient("OpenF1Api");
        var requestUri = $"drivers?driver_number={driverNumber.ToString(CultureInfo.InvariantCulture)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.TryAddWithoutValidation("X-Request-Id", httpContext.TraceIdentifier);

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return new ContentResult
            {
                Content = errorContent,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var drivers = await JsonSerializer.DeserializeAsync<List<OpenF1DriverResponse>>(
            responseStream,
            JsonSerializerOptions,
            cancellationToken)
            ?? throw new JsonException("OpenF1 returned an invalid drivers payload.");

        var uniqueDrivers = drivers
            .DistinctBy(driver => driver.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var result = mapper.Map<IReadOnlyCollection<Driver>>(uniqueDrivers);

        return Ok(result);
    }
}
