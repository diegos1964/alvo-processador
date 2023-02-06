using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processador.Dtos
{
  public class DadosDepatamentoDto
  {
    public DadosArquivoDto DadosArquivoDto { get; set; } = null!;
    public List<DadosHorasFuncionarioDto> ListaDadosHorasFuncionarioDto { get; set; } = null!;
  }
}