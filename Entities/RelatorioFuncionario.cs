using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processador.Entities
{
  public class RelatorioFuncionario : EntidadeBase
  {
    public string Nome { get; set; } = null!;
    public double TotalReceber { get; set; }
    public double TotalDescontos { get; set; }
    public double TotalExtras { get; set; }
    public double HorasExtras { get; set; }
    public double HorasDebito { get; set; }
    public Int16 DiasFalta { get; set; }
    public Int16 DiasExtras { get; set; }
    public Int16 DiasTrabalhados { get; set; }

  }
}