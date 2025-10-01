using Currency.Core.Dto.Dtos;

namespace Currency.Core.Interfaces;

public interface ICbrClient
{
    /// <summary>
    /// Получить курсы валют за указанную дату
    /// </summary>
    Task<IReadOnlyCollection<CbrCurrencyDto>> GetRatesForDateAsync(DateTime date, CancellationToken cancellationToken = default);
}