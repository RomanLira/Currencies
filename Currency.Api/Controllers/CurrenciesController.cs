using Currency.Core.Dto.Dtos;
using Currency.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Api.Controllers;

[ApiController]
[Route("api/currencies")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CurrenciesController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public CurrenciesController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }
    
    /// <summary>
    /// Возвращает список курсов валют (с возможностью фильтрации и пагинации)
    /// </summary>
    /// <param name="from">Дата от</param>
    /// <param name="to">Дата до</param>
    /// <param name="charCode">Код валюты</param>
    /// <param name="page">Страница</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200"> Успешный возврат списка курсов валют</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(DateTime? from, DateTime? to, string? charCode, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _exchangeRateService.GetRatesAsync(from, to, charCode, page, pageSize, cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Возвращает последний курс валюты (на текущий или последний имеющийся день)
    /// </summary>
    /// <param name="id">Id валюты</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200"> Успешный возврат курса валюты</response>
    /// <response code="404"> Курс валюты не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExchangeRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatest(int id, CancellationToken cancellationToken = default)
    {
        var result = await _exchangeRateService.GetLatestForCurrencyAsync(id, cancellationToken);

        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    /// <summary>
    /// Возвращает qr-код
    /// </summary>
    /// <response code="200"> Успешный возврат изображения с qr-кодом</response>
    [HttpGet("qrcbr")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetCbrQr()
    {
        var date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
        var url = $"https://www.cbr.ru/scripts/XML_daily.asp?date_req={date}";
        
        using var qrGenerator = new QRCoder.QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(url, QRCoder.QRCodeGenerator.ECCLevel.Q);
        using var qr = new QRCoder.PngByteQRCode(qrData);
        var bytes = qr.GetGraphic(20); 

        return File(bytes, "image/png");
    }
}