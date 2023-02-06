using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Processador.Dtos
{
  public class DadosHorasFuncionarioLeituraDto
  {
    [Index(0)]
    public string Codigo { get; set; } = null!;
    [Index(1)]
    public string Nome { get; set; } = null!;
    [Index(2)]
    public string ValorHora { get; set; } = null!;
    [Index(3)]
    public string Data { get; set; } = null!;
    [Index(4)]
    public string Entrada { get; set; } = null!;
    [Index(5)]
    public string Saida { get; set; } = null!;
    [Index(6)]
    public string Almoco { get; set; } = null!;
  }
}