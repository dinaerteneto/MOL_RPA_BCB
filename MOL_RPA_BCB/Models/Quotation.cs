/// <summary>
/// Representa os dados de cotação de uma moeda
/// </summary>
/// 
namespace Models;
public class Quotation
{
    public DateTime Date {  get; set; }
    public required string CurrencyName {  get; set; }
    public decimal QuotationBuy {  get; set; }
    public decimal QuotationSell { get; set; }
    public decimal ParityBuy { get; set; }
    public decimal ParitySell {  get; set; }
}
