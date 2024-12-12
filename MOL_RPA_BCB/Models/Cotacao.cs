/// <summary>
/// Representa os dados de cotação de uma moeda
/// </summary>
/// 
namespace Models;
public class Cotacao
{
    public DateTime Data {  get; set; }
    public required string NomeMoeda {  get; set; }
    public decimal CotacaoCompra {  get; set; }
    public decimal CotacaoVenda { get; set; }
    public decimal ParidadeCompra { get; set; }
    public decimal ParidadeVenda {  get; set; }
}
