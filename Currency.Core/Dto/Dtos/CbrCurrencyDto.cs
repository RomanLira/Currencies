namespace Currency.Core.Dto.Dtos;

public class CbrCurrencyDto
{
    public string CbrId { get; set; }
    public string NumCode { get; set; }
    public string CharCode { get; set; }
    public string Name { get; set; }
    public int Nominal { get; set; }
    public decimal Value { get; set; }
    public decimal VunitRate { get; set; }
    public DateTime RateDate { get; set; }
}