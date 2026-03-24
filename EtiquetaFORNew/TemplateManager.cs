using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EtiquetaFORNew
{
    public class TemplateManager
    {
        private static string pastaTemplates = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SistemaEtiquetas",
            "Templates"
        );

        static TemplateManager()
        {
            // Criar pasta se não existir
            if (!Directory.Exists(pastaTemplates))
            {
                Directory.CreateDirectory(pastaTemplates);
            }
        }

        // Salvar template
        public static bool SalvarTemplate(TemplateEtiqueta template, string nomeArquivo)
        {
            try
            {
                string caminhoCompleto = Path.Combine(pastaTemplates, nomeArquivo + ".json");

                var templateSerializavel = ConverterParaSerializavel(template);

                string json = JsonConvert.SerializeObject(templateSerializavel, Formatting.Indented);
                File.WriteAllText(caminhoCompleto, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Erro ao salvar template: {ex.Message}",
                    "Erro",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        // Carregar template
        public static TemplateEtiqueta CarregarTemplate(string nomeArquivo)
        {
            try
            {
                string caminhoCompleto = Path.Combine(pastaTemplates, nomeArquivo + ".json");

                if (!File.Exists(caminhoCompleto))
                {
                    return null;
                }

                string json = File.ReadAllText(caminhoCompleto);
                var templateSerializavel = JsonConvert.DeserializeObject<TemplateSerializavel>(json);

                return ConverterDeSerializavel(templateSerializavel);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Erro ao carregar template: {ex.Message}",
                    "Erro",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return null;
            }
        }

        // Listar todos os templates salvos
        public static List<string> ListarTemplates()
        {
            var templates = new List<string>();

            try
            {
                if (!Directory.Exists(pastaTemplates))
                    return templates;

                var arquivos = Directory.GetFiles(pastaTemplates, "*.json");

                foreach (var arquivo in arquivos)
                {
                    string nome = Path.GetFileNameWithoutExtension(arquivo);
                    templates.Add(nome);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao listar templates: {ex.Message}");
            }

            return templates.OrderBy(x => x).ToList();
        }

        // Excluir template
        public static bool ExcluirTemplate(string nomeArquivo)
        {
            try
            {
                string caminhoCompleto = Path.Combine(pastaTemplates, nomeArquivo + ".json");

                if (File.Exists(caminhoCompleto))
                {
                    File.Delete(caminhoCompleto);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Erro ao excluir template: {ex.Message}",
                    "Erro",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        // Obter caminho da pasta de templates
        public static string ObterPastaTemplates()
        {
            return pastaTemplates;
        }

        // Salvar último template usado
        public static void SalvarUltimoTemplate(TemplateEtiqueta template)
        {
            SalvarTemplate(template, "_ultimo_template");
        }

        // Carregar último template usado
        public static TemplateEtiqueta CarregarUltimoTemplate()
        {
            return CarregarTemplate("_ultimo_template");
        }

        // Converter para formato serializável
        public static TemplateSerializavel ConverterParaSerializavel(TemplateEtiqueta template)
        {
            var templateSerializavel = new TemplateSerializavel
            {
                Largura = template.Largura,
                Altura = template.Altura,
                Elementos = new List<ElementoSerializavel>()
            };

            foreach (var elem in template.Elementos)
            {
                var elemSerializavel = new ElementoSerializavel
                {
                    Tipo = elem.Tipo.ToString(),
                    Conteudo = elem.Conteudo,
                    X = elem.Bounds.X,
                    Y = elem.Bounds.Y,
                    Largura = elem.Bounds.Width,
                    Altura = elem.Bounds.Height,
                    FonteNome = elem.Fonte.FontFamily.Name,
                    FonteTamanho = elem.Fonte.Size,
                    Negrito = elem.Negrito,
                    Italico = elem.Italico,
                    CorR = elem.Cor.R,
                    CorG = elem.Cor.G,
                    CorB = elem.Cor.B,
                    Alinhamento = elem.Alinhamento.ToString(),
                    Rotacao = elem.Rotacao
                };

                // ⭐ NOVA: Salvar cor de fundo se definida
                if (elem.CorFundo.HasValue)
                {
                    elemSerializavel.CorFundoR = elem.CorFundo.Value.R;
                    elemSerializavel.CorFundoG = elem.CorFundo.Value.G;
                    elemSerializavel.CorFundoB = elem.CorFundo.Value.B;
                }

                // Se for imagem, salvar como Base64
                if (elem.Tipo == TipoElemento.Imagem && elem.Imagem != null)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        elem.Imagem.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        elemSerializavel.ImagemBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                }

                templateSerializavel.Elementos.Add(elemSerializavel);
            }

            return templateSerializavel;
        }

        // Converter de formato serializável
        public static TemplateEtiqueta ConverterDeSerializavel(TemplateSerializavel templateSerializavel)
        {
            var template = new TemplateEtiqueta
            {
                Largura = templateSerializavel.Largura,
                Altura = templateSerializavel.Altura,
                Elementos = new List<ElementoEtiqueta>()
            };

            foreach (var elemSer in templateSerializavel.Elementos)
            {
                var elem = new ElementoEtiqueta
                {
                    Tipo = (TipoElemento)Enum.Parse(typeof(TipoElemento), elemSer.Tipo),
                    Conteudo = elemSer.Conteudo,
                    Bounds = new System.Drawing.Rectangle(elemSer.X, elemSer.Y, elemSer.Largura, elemSer.Altura),
                    Negrito = elemSer.Negrito,
                    Italico = elemSer.Italico,
                    Cor = System.Drawing.Color.FromArgb(elemSer.CorR, elemSer.CorG, elemSer.CorB)
                };

                // Alinhamento
                if (!string.IsNullOrEmpty(elemSer.Alinhamento))
                {
                    try
                    {
                        elem.Alinhamento = (System.Drawing.StringAlignment)Enum.Parse(
                            typeof(System.Drawing.StringAlignment),
                            elemSer.Alinhamento
                        );
                    }
                    catch
                    {
                        elem.Alinhamento = System.Drawing.StringAlignment.Near;
                    }
                }
                else
                {
                    elem.Alinhamento = System.Drawing.StringAlignment.Near;
                }

                // Reconstruir fonte
                System.Drawing.FontStyle estilo = System.Drawing.FontStyle.Regular;
                if (elem.Negrito) estilo |= System.Drawing.FontStyle.Bold;
                if (elem.Italico) estilo |= System.Drawing.FontStyle.Italic;
                elem.Fonte = new System.Drawing.Font(elemSer.FonteNome, elemSer.FonteTamanho, estilo);

                // ⭐ NOVA: Restaurar rotação
                elem.Rotacao = elemSer.Rotacao;

                // ⭐ NOVA: Restaurar cor de fundo se salva
                if (elemSer.CorFundoR.HasValue && elemSer.CorFundoG.HasValue && elemSer.CorFundoB.HasValue)
                {
                    elem.CorFundo = System.Drawing.Color.FromArgb(
                        elemSer.CorFundoR.Value,
                        elemSer.CorFundoG.Value,
                        elemSer.CorFundoB.Value
                    );
                }

                // Se for imagem, converter de Base64
                if (elem.Tipo == TipoElemento.Imagem && !string.IsNullOrEmpty(elemSer.ImagemBase64))
                {
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(elemSer.ImagemBase64);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            elem.Imagem = System.Drawing.Image.FromStream(ms);
                        }
                    }
                    catch { }
                }

                template.Elementos.Add(elem);
            }

            return template;
        }
    }

    // Classes para serialização JSON
    [Serializable]
    public class TemplateSerializavel
    {
        public float Largura { get; set; }
        public float Altura { get; set; }
        public List<ElementoSerializavel> Elementos { get; set; }
    }

    [Serializable]
    public class ElementoSerializavel
    {
        public string Tipo { get; set; }
        public string Conteudo { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public string FonteNome { get; set; }
        public float FonteTamanho { get; set; }
        public bool Negrito { get; set; }
        public bool Italico { get; set; }
        public int CorR { get; set; }
        public int CorG { get; set; }
        public int CorB { get; set; }
        public string Alinhamento { get; set; }
        public string ImagemBase64 { get; set; }

        // ⭐ NOVAS PROPRIEDADES
        public int? CorFundoR { get; set; }
        public int? CorFundoG { get; set; }
        public int? CorFundoB { get; set; }
        public float Rotacao { get; set; }
    }
}