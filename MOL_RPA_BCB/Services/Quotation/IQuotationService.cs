namespace Services.Quotation;

using Models;

public interface IQuotationService
{
    Task<List<Quotation>> GetQuotationsAsync(DateTime startDate, DateTime finalDate, string currencyBase);
}
