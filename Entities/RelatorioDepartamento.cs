using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processador.Entities
{
  public class RelatorioDepartamento
  {
    public string Departamento { get; set; } = null!;
    public string MesVigencia { get; set; } = null!;
    public int AnoVigencia { get; set; }
    public double TotalPagar { get; set; }
    public double TotalDescontos { get; set; }
    public double TotalExtras { get; set; }
    public List<RelatorioFuncionario>? Funcionarios { get; set; }

  }

  // public double CalcularTotalPagar(List<RelatorioFuncionario>? funcionarios){

  // }
}