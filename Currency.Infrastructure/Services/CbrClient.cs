using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Currency.Core.Dto.Dtos;
using Currency.Core.Interfaces;
using Currency.Infrastructure.XmlModels;

namespace Currency.Infrastructure.Services;

public class CbrClient : ICbrClient
{
    private readonly HttpClient _httpClient;
    
    public CbrClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<CbrCurrencyDto>> GetRatesForDateAsync(DateTime date,
        CancellationToken cancellationToken = default)
    {
        var url = $"https://www.cbr.ru/scripts/XML_daily.asp?date_req={date:dd/MM/yyyy}";
        
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251"));
        
        var serializer = new XmlSerializer(typeof(ValCurs));
        var data = (ValCurs)serializer.Deserialize(reader)!;

        var result = new List<ExchangeRateDto>();

        return data.Valutes.Select(v => new CbrCurrencyDto
        {
            CbrId = v.Id,
            NumCode = v.NumCode,
            CharCode = v.CharCode,
            Name = v.Name,
            Nominal = v.Nominal,
            Value = decimal.Parse(v.ValueRaw.Replace(",", "."), CultureInfo.InvariantCulture),
            VunitRate = decimal.Parse(v.VunitRateRaw.Replace(",", "."), CultureInfo.InvariantCulture),
            RateDate = DateTime.SpecifyKind(date, DateTimeKind.Utc)
        }).ToList();
    }
}