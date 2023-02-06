using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Processador.Dtos;
using Processador.Entities;
using Processador.Services;
using Processador.Utils;

internal class Program
{

  private static async Task Main(string[] args)
  {
    await ArquivoService.ProcessarArquivo();

  }

}