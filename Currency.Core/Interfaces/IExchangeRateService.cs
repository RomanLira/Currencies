using Currency.Core.Dto.Dtos;

namespace Currency.Core.Interfaces;

public interface IExchangeRateService
{
    /// <summary>
    /// Импортировать данные за N последних дней
    /// </summary>
    Task ImportRangeAsync(int daysFromToday, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список курсов с фильтрацией
    /// </summary>
    Task<IEnumerable<CurrencyDto>> GetRatesAsync(DateTime? from, DateTime? to, string? charCode, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить последний курс валюты по ID
    /// </summary>
    Task<ExchangeRateDto?> GetLatestForCurrencyAsync(int currencyId, CancellationToken cancellationToken = default);
}