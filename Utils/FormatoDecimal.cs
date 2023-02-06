using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processador.Utils
{
  public static class FormatoDecimal
  {
    public static double FormatarNumero(double numero)
    {
      double x = Math.Truncate(numero * 100) / 100;

      return x;
    }
  }
}