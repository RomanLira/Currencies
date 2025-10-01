using AutoMapper;
using Currency.Core.Dto.Mappings;
using Currency.Core.Entities;

namespace Currency.Core.Dto.Dtos;

public class ExchangeRateDto : IMapWith
{
    /// <summary>
    /// Идентификатор в БД
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// День курса
    /// </summary>
    public DateTime RateDate { get; set; }
    
    /// <summary>
    /// Номинал
    /// </summary>
    public int Nominal { get; set; }
    
    /// <summary>
    /// Курс за номинал
    /// </summary>
    public decimal Value { get; set; }
    
    /// <summary>
    /// Курс за единицу валюты
    /// </summary>
    public decimal VunitRate { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ExchangeRate, ExchangeRateDto>()
            .ReverseMap();
    }
}