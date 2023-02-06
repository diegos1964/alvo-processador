using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace Processador.Dtos
{
  public class DadosHorasFuncionarioDto
  {
    public DadosHorasFuncionarioDto(
      int codigo,
      string data,
      string entrada,
      string saida,
      string almocoInicio,
      string almocoFim
    )
    {
      Codigo = codigo;
      SetData(data);
      Entrada = SetStringToTimeSpan(entrada);
      Saida = SetStringToTimeSpan(saida);
      AlmocoInicio = SetStringToTimeSpan(almocoInicio);
      AlmocoFim = SetStringToTimeSpan(almocoFim);
    }

    public int Codigo { get; set; }
    public string Nome { get; set; } = null!;
    public double ValorHora { get; private set; }
    public DateTime Data { get; set; }
    public TimeSpan Entrada { get; set; }
    public TimeSpan Saida { get; set; }
    public TimeSpan AlmocoInicio { get; set; }
    public TimeSpan AlmocoFim { get; set; }
    public double QuantidadeDiasTrabalhados { get; set; }
    public double QuantidadeHorasTrabalhadas { get; set; }
    public double QuantidadeDeHorasExtras { get; set; }
    public double QuantidadeDeHorasFaltas { get; set; }
    public double QuantidadeDeHorasAlmoco { get; set; }
    public double QuantidadeDeHorasAlmocoFaltas { get; set; }
    public double QuantidadeDiasFaltas { get; set; }
    public double QuantidadeDiasExtras { get; set; }
    public double ValorTotalHorasTrabalhadas { get; set; }
    public double ValorTotalHorasDescontadas { get; set; }
    public double ValorTotalHorasExtras { get; set; }



    public void SetValorTotalHorasTrabalhadas()
    {
      this.ValorTotalHorasTrabalhadas = QuantidadeHorasTrabalhadas * ValorHora;
    }

    public void SetValorTotalHorasDescontadas()
    {
      this.ValorTotalHorasDescontadas = QuantidadeDeHorasFaltas * ValorHora;
    }
    public void SetValorTotalHorasExtras()
    {
      this.ValorTotalHorasExtras = QuantidadeDeHorasExtras * ValorHora;
    }




    public void SetQuantidadeDeHorasAlmoço()
    {
      DateTime t1 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        AlmocoInicio.Hours,
        AlmocoInicio.Minutes,
        AlmocoInicio.Seconds
      );

      DateTime t2 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        AlmocoFim.Hours,
        AlmocoFim.Minutes,
        AlmocoFim.Seconds
      );

      TimeSpan dt = t2 - t1;

      this.QuantidadeDeHorasAlmoco = ValorHora > 0 ? dt.TotalHours : 0;
      this.QuantidadeDeHorasAlmocoFaltas = ValorHora < 0 ? dt.TotalHours : 0;
    }


    public void SetQuantidadeDeHorasFaltadas()
    {

      DateTime t1 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        Entrada.Hours,
        Entrada.Minutes,
        Entrada.Seconds
      );
      DateTime t2 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        Saida.Hours,
        Saida.Minutes,
        Saida.Seconds
      );

      TimeSpan dt = t2 - t1;

      this.QuantidadeDeHorasFaltas = ValorHora <= 0 ? dt.TotalHours : 0;
      this.QuantidadeDiasFaltas = ValorHora <= 0 ? dt.TotalDays : 0;


    }

    public void SetQuantidadeHorasTrabalhadas()
    {

      DateTime t1 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        Entrada.Hours,
        Entrada.Minutes,
        Entrada.Seconds
      );

      DateTime t2 = new DateTime(
        Data.Year,
        Data.Month,
        Data.Day,
        Saida.Hours,
        Saida.Minutes,
        Saida.Seconds
      );

      TimeSpan dt = t2 - t1;

      this.QuantidadeHorasTrabalhadas = ValorHora > 0 ? dt.TotalHours : 0;
      this.QuantidadeDiasTrabalhados = ValorHora > 0 ? dt.TotalDays : 0;

    }

    public void SetHorasExtras()
    {
      this.QuantidadeDeHorasExtras = QuantidadeHorasTrabalhadas > 8 ? QuantidadeHorasTrabalhadas - 8 : QuantidadeHorasTrabalhadas;
    }


    public void SetValorHora(string valor)
    {
      this.ValorHora = double.Parse(valor.Replace("R$", "").Replace(" ", ""));
    }

    public void SetNome(string nome)
    {
      this.Nome = Encoding.UTF8.GetString(Encoding.Default.GetBytes(nome));
    }

    public void SetData(string data)
    {
      this.Data = DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    private TimeSpan SetStringToTimeSpan(string hora)
    {
      return TimeSpan.Parse(hora);
    }
  }

}