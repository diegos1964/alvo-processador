using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Processador.Dtos;
using Processador.Entities;
using Processador.Utils;

namespace Processador.Services
{
  public static class ArquivoService
  {
    static readonly TaskCompletionSource<bool> s_tcs = new TaskCompletionSource<bool>();
    public static async Task ProcessarArquivo()
    {
      try
      {
        Console.WriteLine("Olá, Seja bem-vindo!");

        Console.WriteLine("Digite a pasta raiz dos arquivos: ");
        string? pastarParaProcessar = Console.ReadLine();

        if (String.IsNullOrEmpty(pastarParaProcessar))
        {
          Console.WriteLine("O Caminho da pasta raiz deve ser informado para processar");

        }

        if (!String.IsNullOrEmpty(pastarParaProcessar))
        {

          var stopWatch = new Stopwatch();
          string[] arquivosParaProcessar = Directory.GetFiles(pastarParaProcessar);

          stopWatch.Start();
          List<DadosDepatamentoDto> listaDadosDasPlanilhas = new List<DadosDepatamentoDto>();
          await Parallel.ForEachAsync(arquivosParaProcessar, async (arquivo, cancellationToken) =>
          {
            var ultimaBarra = arquivo.LastIndexOf("\\") + 1;
            var nomeArquivo = arquivo.Substring(ultimaBarra);
            var nomeSemExtensao = nomeArquivo.Split(".")[0];
            var arrayDadosDoArquivo = nomeSemExtensao.Split("-");
            await LerArquivo(arquivo, arrayDadosDoArquivo, listaDadosDasPlanilhas);
          });
          stopWatch.Stop();

          await GerarJson(listaDadosDasPlanilhas);
          Console.WriteLine(" Arquivo Json Gerado Com Sucesso!");
        }
      }
      catch (System.Exception ex)
      {
        Console.WriteLine("Ocorreu uma falha no processar os arquivos!");
      }
    }


    private static async Task<List<DadosDepatamentoDto>> LerArquivo(
      string arquivo,
      string[] arrayDadosDoArquivo,
      List<DadosDepatamentoDto> listaDadosDasPlanilhas

      )
    {

      var dadosArquivoDto = new DadosArquivoDto()
      {
        Ano = Convert.ToInt32(arrayDadosDoArquivo[2]),
        Mes = arrayDadosDoArquivo[1],
        NomeDepartamento = arrayDadosDoArquivo[0],
      };

      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        HasHeaderRecord = true,
        PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", ""),
      };



      List<DadosHorasFuncionarioDto> ListaDadosHorasFuncionarioDto = new List<DadosHorasFuncionarioDto>();
      using (var reader = new StreamReader(arquivo))

      using (var csv = new CsvReader(reader, config))
      {
        await csv.ReadAsync();
        csv.ReadHeader();
        while (await csv.ReadAsync())
        {
          var funcionario = csv.GetRecord<DadosHorasFuncionarioLeituraDto>();

          if (funcionario != null)
          {
            var novoFuncionario = await SetDadosHorasFuncionarioDto(funcionario);

            ListaDadosHorasFuncionarioDto.Add(novoFuncionario);
          }
        }

        listaDadosDasPlanilhas.Add(
          new DadosDepatamentoDto()
          {
            DadosArquivoDto = dadosArquivoDto,
            ListaDadosHorasFuncionarioDto = ListaDadosHorasFuncionarioDto
          }
        );


        return await Task.FromResult(listaDadosDasPlanilhas);

      }

    }
    private static async Task<DadosHorasFuncionarioDto> SetDadosHorasFuncionarioDto(DadosHorasFuncionarioLeituraDto funcionario)
    {
      var novoFuncionario = new DadosHorasFuncionarioDto(
               Convert.ToInt32(funcionario.Codigo),
               funcionario.Data,
               funcionario.Entrada,
               funcionario.Saida,
               funcionario.Almoco.Split("-")[0],
               funcionario.Almoco.Split("-")[1]
             );

      novoFuncionario.SetValorHora(funcionario.ValorHora);
      novoFuncionario.SetNome(funcionario.Nome);
      novoFuncionario.SetQuantidadeDeHorasAlmoço();
      novoFuncionario.SetQuantidadeDeHorasFaltadas();
      novoFuncionario.SetQuantidadeHorasTrabalhadas();
      novoFuncionario.SetHorasExtras();
      novoFuncionario.SetValorTotalHorasTrabalhadas();
      novoFuncionario.SetValorTotalHorasDescontadas();
      novoFuncionario.SetValorTotalHorasExtras();


      return await Task.FromResult(novoFuncionario);
    }

    private static async Task GerarJson(List<DadosDepatamentoDto> listaDadosDasPlanilhas)
    {
      Task<bool> secondHandlerFinished = s_tcs.Task;
      List<RelatorioDepartamento> listaDepartamentos = new List<RelatorioDepartamento>();
      List<RelatorioDepartamento> listaDepartamentosFinal = new List<RelatorioDepartamento>();


      foreach (var dep in listaDadosDasPlanilhas)
      {
        RelatorioDepartamento departamento = new RelatorioDepartamento()
        {
          Departamento = dep.DadosArquivoDto.NomeDepartamento,
          AnoVigencia = dep.DadosArquivoDto.Ano,
          MesVigencia = dep.DadosArquivoDto.Mes,
          Funcionarios = new List<RelatorioFuncionario>(),
          TotalDescontos = 0,
          TotalExtras = 0,
          TotalPagar = 0
        };
        listaDepartamentos.Add(departamento);
      }

      foreach (var planilha in listaDadosDasPlanilhas)
      {
        foreach (var departamento in listaDepartamentos)
        {


          if (departamento.Departamento == planilha.DadosArquivoDto.NomeDepartamento
             && departamento.AnoVigencia == planilha.DadosArquivoDto.Ano
             && departamento.MesVigencia == planilha.DadosArquivoDto.Mes
          )
          {
            var planilhaFiltrada = listaDadosDasPlanilhas.Where(x =>
              departamento.Departamento == x.DadosArquivoDto.NomeDepartamento
             && departamento.AnoVigencia == x.DadosArquivoDto.Ano
             && departamento.MesVigencia == x.DadosArquivoDto.Mes
            ).ToList();

            var listaDadosFuncionario = await DefinirListaDeFuncionarios(planilhaFiltrada[0].ListaDadosHorasFuncionarioDto);
            RelatorioDepartamento departamentoFinal = new RelatorioDepartamento()
            {
              Departamento = planilha.DadosArquivoDto.NomeDepartamento,
              AnoVigencia = planilha.DadosArquivoDto.Ano,
              MesVigencia = planilha.DadosArquivoDto.Mes,
              Funcionarios = listaDadosFuncionario,
              TotalDescontos = FormatoDecimal.FormatarNumero(listaDadosFuncionario.Sum(c => c.TotalDescontos)),
              TotalExtras = FormatoDecimal.FormatarNumero(listaDadosFuncionario.Sum(c => c.TotalExtras)),
              TotalPagar = FormatoDecimal.FormatarNumero(listaDadosFuncionario.Sum(c => c.TotalReceber)),
            };

            listaDepartamentosFinal.Add(departamentoFinal);
          }


        }

      }

      JsonFileUtils.SimpleWrite(listaDepartamentosFinal, "C:\\Auvo\\ArquivosGerados\\RelatorioFinal.json");
      await secondHandlerFinished;



    }

    private static async Task<List<RelatorioFuncionario>> DefinirListaDeFuncionarios(List<DadosHorasFuncionarioDto> ListaDadosHorasFuncionarioDto)
    {
      var listaDadosFuncionario = ListaDadosHorasFuncionarioDto
        .GroupBy(f => f.Codigo)
        .Select(lf => new RelatorioFuncionario
        {
          Codigo = lf.First().Codigo,
          Nome = lf.First().Nome,
          TotalReceber = FormatoDecimal.FormatarNumero(lf.Sum(fu => fu.ValorTotalHorasTrabalhadas)),
          HorasExtras = FormatoDecimal.FormatarNumero(lf.Sum(fu => fu.QuantidadeDeHorasExtras)),
          HorasDebito = FormatoDecimal.FormatarNumero(lf.Sum(fu => fu.QuantidadeDeHorasFaltas)),
          DiasExtras = Convert.ToInt16((lf.Sum(fu => fu.QuantidadeDeHorasExtras) / 24)),
          DiasFalta = Convert.ToInt16((lf.Sum(fu => fu.QuantidadeDeHorasFaltas) / 24)),
          DiasTrabalhados = Convert.ToInt16(lf.Sum(fu => fu.QuantidadeHorasTrabalhadas) / 9),
          TotalDescontos = FormatoDecimal.FormatarNumero(lf.Sum(fu => fu.ValorTotalHorasDescontadas)),
          TotalExtras = FormatoDecimal.FormatarNumero(lf.Sum(fu => fu.ValorTotalHorasExtras)),
        }).ToList();

      return await Task.FromResult(listaDadosFuncionario);

    }
  }
}