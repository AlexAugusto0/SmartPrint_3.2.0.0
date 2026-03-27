using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing; // Necessário para a classe Image
using System.IO;
using System.Linq;
using System.Reflection;

public class CalibracaoInfo
{
    [JsonProperty("nome")]
    public string Nome { get; set; }

    [JsonProperty("imagemRecurso")]
    public string ImagemRecurso { get; set; }

    [JsonProperty("youtubeUrl")]
    public string YoutubeUrl { get; set; }

    public string ComandoCalibracao { get; set; }
    public string Descricao { get; set; }

    /// <summary>
    /// Método para obter a imagem da calibração a partir dos recursos ou arquivos
    /// </summary>
    public Image ObterImagem()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            // 1. Tenta carregar dos recursos embarcados (mesma lógica que você já usa)
            string[] possiveisNomes = new string[]
            {
                $"EtiquetaFORNew.Resources.Impressoras.{ImagemRecurso}",
                $"EtiquetaFORNew.Resources.{ImagemRecurso}",
                $"{ImagemRecurso}"
            };

            foreach (string nomeTentativa in possiveisNomes)
            {
                using (Stream stream = assembly.GetManifestResourceStream(nomeTentativa))
                {
                    if (stream != null) return Image.FromStream(stream);
                }
            }

            // 2. Fallback: Se não encontrar nos recursos, tenta buscar pelo nome do arquivo
            string[] recursos = assembly.GetManifestResourceNames();
            string recursoEncontrado = recursos.FirstOrDefault(r => r.EndsWith(ImagemRecurso, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(recursoEncontrado))
            {
                using (Stream stream = assembly.GetManifestResourceStream(recursoEncontrado))
                {
                    if (stream != null) return Image.FromStream(stream);
                }
            }

            // 3. Fallback: Pasta física (caso o arquivo esteja na pasta do programa)
            string caminhoExterno = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Impressoras", ImagemRecurso);
            if (File.Exists(caminhoExterno)) return Image.FromFile(caminhoExterno);

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar imagem: {ex.Message}");
            return null;
        }
    }

}

public class CalibracaoConfig
{
    [JsonProperty("calibracoes")]
    public List<CalibracaoInfo> Calibracoes { get; set; }
}

public static class CalibracaoManager
{
    private static List<CalibracaoInfo> _calibracoes;

    public static List<CalibracaoInfo> CarregarCalibracoes()
    {
        if (_calibracoes != null) return _calibracoes;

        try
        {
            string json = CarregarJsonDoRecurso("calibracao.json");

            if (string.IsNullOrEmpty(json))
            {
                string[] caminhos = {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "calibracao.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "calibracao.json")
                };

                foreach (var caminho in caminhos)
                {
                    if (File.Exists(caminho))
                    {
                        json = File.ReadAllText(caminho);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(json))
            {
                var config = JsonConvert.DeserializeObject<CalibracaoConfig>(json);
                _calibracoes = config?.Calibracoes ?? new List<CalibracaoInfo>();
            }
        }
        catch (Exception ex)
        {
            _calibracoes = new List<CalibracaoInfo>();
            System.Diagnostics.Debug.WriteLine("Erro Calibracao: " + ex.Message);
        }

        return _calibracoes;
    }

    private static string CarregarJsonDoRecurso(string nomeArquivo)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string[] tentativas = {
            $"EtiquetaFORNew.Resources.{nomeArquivo}",
            $"EtiquetaFORNew.{nomeArquivo}",
            nomeArquivo
        };

        foreach (string nome in tentativas)
        {
            using (Stream stream = assembly.GetManifestResourceStream(nome))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                }
            }
        }
        return null;
    }

}