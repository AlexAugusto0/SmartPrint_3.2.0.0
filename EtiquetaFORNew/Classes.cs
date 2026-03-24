using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EtiquetaFORNew
{
    // Classe de Produto
    public class Produto
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string CodFabricante { get; set; }
        public string CodBarras { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal VendaA { get; set; }
        public decimal VendaB { get; set; }
        public decimal VendaC { get; set; }
        public decimal VendaD { get; set; }
        public decimal VendaE { get; set; }
        public string Fornecedor { get; set; }
        public string Fabricante { get; set; }
        public string Grupo { get; set; }
        public string Prateleira { get; set; }
        public string Garantia { get; set; }
        public string Tam { get; set; }
        public string Cores { get; set; }
        public string CodBarras_Grade { get; set; }
        public decimal? PrecoOriginal { get; set; }      // Preço antes da promoção
        public decimal? PrecoPromocional { get; set; }   // Preço com desconto aplicado
        public bool EmPromocao => PrecoOriginal.HasValue && PrecoPromocional.HasValue;

    }

    // Enum de tipos de elementos
    public enum TipoElemento
    {
        Texto,
        Campo,
        CodigoBarras,
        Imagem
    }

    // Classe de Elemento da Etiqueta
    public class ElementoEtiqueta
    {
        public TipoElemento Tipo { get; set; }
        public string Conteudo { get; set; }
        public Rectangle Bounds { get; set; }
        public Font Fonte { get; set; }
        public Color Cor { get; set; }
        public Image Imagem { get; set; }
        public bool Negrito { get; set; }
        public bool Italico { get; set; }

        // ✅ NOVA PROPRIEDADE: Alinhamento do texto
        public StringAlignment Alinhamento { get; set; }

        public float Rotacao { get; set; }

        // ✅ NOVA PROPRIEDADE: Nome da família da fonte escolhida
        public string NomeFonte { get; set; }

        // ⭐ NOVA PROPRIEDADE: Cor de fundo do elemento (espaço ao redor do texto)
        public Color? CorFundo { get; set; }

        public ElementoEtiqueta()
        {
            Fonte = new Font("Arial", 10);
            Cor = Color.Black;
            Alinhamento = StringAlignment.Near;  // Padrão: esquerda
            Rotacao = 0f;
        }
    }

    // Classe do Template de Etiqueta
    public class TemplateEtiqueta
    {
        public float Largura { get; set; } = 50;
        public float Altura { get; set; } = 30;
        public List<ElementoEtiqueta> Elementos { get; set; } = new List<ElementoEtiqueta>();
        public string CaminhoArquivo { get; set; }

        public TemplateEtiqueta Clone()
        {
            return new TemplateEtiqueta
            {
                Largura = this.Largura,
                Altura = this.Altura,
                CaminhoArquivo = this.CaminhoArquivo,
                Elementos = new List<ElementoEtiqueta>(this.Elementos)
            };
        }
        public string SalvarParaXml()
        {
            // Usa o Manager que você já criou para transformar o objeto em algo "salvável"
            var serializavel = TemplateManager.ConverterParaSerializavel(this);
            // Transforma em texto (snapshot)
            return JsonConvert.SerializeObject(serializavel);
        }

        public static TemplateEtiqueta CarregarDeSnapshot(string json)
        {
            // Transforma o texto de volta no objeto serializável
            var serializavel = JsonConvert.DeserializeObject<TemplateSerializavel>(json);
            // Usa o Manager para reconstruir a classe com Fontes e Imagens reais
            return TemplateManager.ConverterDeSerializavel(serializavel);
        }
    }
}