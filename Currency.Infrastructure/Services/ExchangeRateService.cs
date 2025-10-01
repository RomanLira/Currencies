using AutoMapper;
using AutoMapper.QueryableExtensions;
using Currency.Core.Dto.Dtos;
using Currency.Core.Entities;
using Currency.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Currency.Infrastructure.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly AppDbContext _context;
    private readonly ICbrClient _cbrClient;
    private readonly IMapper _mapper;

    public ExchangeRateService(AppDbContext context, ICbrClient cbrClient, IMapper mapper)
    {
        _context = context;
        _cbrClient = cbrClient;
        _mapper = mapper;
    }

    public async Task ImportRangeAsync(int daysFromToday, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < daysFromToday; i++)
        {
            var date = DateTime.Today.AddDays(-i);
            var rates = await _cbrClient.GetRatesForDateAsync(date, cancellationToken);

            foreach (var rate in rates)
            {
                var currency =
                    await _context.Currencies.FirstOrDefaultAsync(_ => _.CharCode == rate.CharCode, cancellationToken);

                if (currency == null)
                {
                    currency = new Core.Entities.Currency
                    {
                        CbrId = rate.CbrId,
                        NumCode = rate.NumCode,
                        CharCode = rate.CharCode,
                        Name = rate.Name
                    };

                    _context.Currencies.Add(currency);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var exists = await _context.ExchangeRates
                    .AnyAsync(_ => _.CurrencyId == currency.Id && _.RateDate == rate.RateDate, cancellationToken);

                if (!exists)
                {
                    _context.ExchangeRates.Add(new ExchangeRate
                    {
                        CurrencyId = currency.Id,
                        RateDate = rate.RateDate,
                        Nominal = rate.Nominal,
                        Value = rate.Value,
                        VunitRate = rate.VunitRate
                    });
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<CurrencyDto>> GetRatesAsync(DateTime? from, DateTime? to, string? charCode,
        int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.Currencies.AsQueryable();

        if (from.HasValue)
            query = query.Where(_ => _.ExchangeRates.Any(r => r.RateDate >= from.Value));
        
        if (to.HasValue)
            query = query.Where(_ => _.ExchangeRates.Any(r => r.RateDate <= to.Value));
        
        if (!string.IsNullOrEmpty(charCode))
            query = query.Where(_ => _.CharCode == charCode);

        return await query
            .OrderBy(_ => _.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<CurrencyDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExchangeRateDto?> GetLatestForCurrencyAsync(int currencyId, CancellationToken cancellationToken = default)
    {
        var rate = await _context.ExchangeRates
            .Where(_ => _.CurrencyId == currencyId)
            .OrderByDescending(_ => _.RateDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (rate == null) 
            return null;

        return _mapper.Map<ExchangeRateDto>(rate);
    }
}