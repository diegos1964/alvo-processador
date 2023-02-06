using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processador.Dtos
{
  public class DadosArquivoDto
  {
    public string NomeDepartamento { get; set; } = null!;
    public string Mes { get; set; } = null!;
    public int Ano { get; set; }

  }
}