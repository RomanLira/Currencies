using AutoMapper;
using Currency.Core.Dto.Mappings;

namespace Currency.Core.Dto.Dtos;

public class CurrencyDto : IMapWith
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
    public List<ExchangeRateDto> ExchangeRates { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Entities.Currency, CurrencyDto>()
            .ForMember(dest => dest.ExchangeRates,
                opt => opt.MapFrom(src => src.ExchangeRates.OrderByDescending(_ => _.RateDate)))
            .ReverseMap();
    }
}