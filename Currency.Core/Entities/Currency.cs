namespace Currency.Core.Entities;

/// <summary>
/// Валюта
/// </summary>
public class Currency
{
    /// <summary>
    /// Идентификатор в БД
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор в CBR
    /// </summary>
    public string CbrId { get; set; }
    
    /// <summary>
    /// Цифровой код
    /// </summary>
    public string NumCode { get; set; }
    
    /// <summary>
    /// Символьный код
    /// </summary>
    public string CharCode { get; set; }
    
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Список курса валюты по дням
    /// </summary>
    public ICollection<ExchangeRate> ExchangeRates { get; set; } = new List<ExchangeRate>();
}