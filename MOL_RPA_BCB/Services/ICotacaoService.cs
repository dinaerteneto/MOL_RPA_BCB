﻿namespace Services;

using Models;

public interface ICotacaoService
{
    Task<List<Cotacao>> ObterCotacaoesAsync(DateTime inicio, DateTime fim, string moedaBase);
}