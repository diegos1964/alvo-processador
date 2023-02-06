using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Processador.Utils
{
  public static class JsonFileUtils
  {
    private static readonly JsonSerializerOptions _options =
       new()
       {
         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
         WriteIndented = true

       };



    public static void SimpleWrite(object obj, string fileName)
    {
      var jsonString = JsonSerializer.Serialize(obj, _options);
      File.WriteAllText(fileName, jsonString);
    }
  }
}