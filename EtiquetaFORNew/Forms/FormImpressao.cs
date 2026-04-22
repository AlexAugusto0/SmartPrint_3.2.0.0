using BarcodeStandard;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq; // Adicionado para uso potencial de LINQ
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class FormImpressao : Form
    {
        private List<Produto> produtos;
        private TemplateEtiqueta template;
        private ConfiguracaoEtiqueta configuracaoEtiqueta;

        private List<List<Produto>> produtosPorPagina;
        private int paginaAtual = 0;

        // ⭐ NOVO: O objeto responsável pela impressão
        private PrintDocument printDocument1;

        private float zoomEscala = 1.0f; // Escala base (1.0 = 100%)
        private const float FATOR_ZOOM = 1.2f; // Multiplicador de zoom

        // =================================================================
        // ⭐ NOVO MÉTODO DE CONVERSÃO - CRÍTICO PARA IMPRESSÃO CORRETA
        // =================================================================

        /// <summary>
        /// Converte milímetros (MM) para centésimos de polegada (1/100"), 
        /// que é a unidade exigida pela classe System.Drawing.Printing.PaperSize.
        /// </summary>
        private int MmToHundredthsOfInch(float mm)
        {
            // Fator de conversão: 1 polegada = 25.4 mm
            // Centésimos de polegada = (mm / 25.4) * 100
            return (int)Math.Round((mm / 25.4f) * 100f);
        }

        public FormImpressao(List<Produto> produtos, TemplateEtiqueta template, ConfiguracaoEtiqueta configuracao = null)
        {
            InitializeComponent();
            VersaoHelper.DefinirTituloComVersao(this, "Visualização de Impressão");
            this.produtos = produtos;
            this.template = template;
            this.configuracaoEtiqueta = configuracao ?? CriarConfiguracaoPadrao();

            // ⭐ CORREÇÃO: Inicializa o PrintDocument e anexa o evento PrintPage
            printDocument1 = new PrintDocument();
            printDocument1.PrintPage += PrintDoc_PrintPage;

            this.Shown += FormImpressao_Shown;
            this.Resize += (sender, e) => DesenharVisualizacao();
            panelVisualizacao.AutoScroll = true;
            panelVisualizacao.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(panelVisualizacao, true, null);
            this.MouseWheel += FormImpressao_MouseWheel;
        }

        private ConfiguracaoEtiqueta CriarConfiguracaoPadrao()
        {
            return new ConfiguracaoEtiqueta
            {
                NumColunas = 1,
                NumLinhas = 1,
                EspacamentoColunas = 0,
                EspacamentoLinhas = 0,
                MargemEsquerda = 0,
                MargemSuperior = 0,
                MargemDireita = 0,
                MargemInferior = 0,
                LarguraEtiqueta = template.Largura,
                AlturaEtiqueta = template.Altura
            };
        }

        // ... (Seu código original FormImpressao_Shown, CalcularPaginacao, etc.)

        private void FormImpressao_Shown(object sender, EventArgs e)
        {
            try
            {
                CalcularPaginacao();
                DesenharVisualizacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao montar visualização: {ex.Message}\n\n{ex.StackTrace}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // PAGINAÇÃO E VISUALIZAÇÃO (Sem alterações nas funções auxiliares)
        // ==========================================
        private void CalcularPaginacao()
        {
            produtosPorPagina = new List<List<Produto>>();

            if (template == null)
            {
                MessageBox.Show("Template não definido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int numColunas = Math.Max(1, configuracaoEtiqueta.NumColunas);
            int numLinhas = Math.Max(1, configuracaoEtiqueta.NumLinhas);
            int etiquetasPorPagina = numColunas * numLinhas;

            // Expande produtos baseado em quantidade
            List<Produto> etiquetas = new List<Produto>();
            foreach (var produto in produtos)
            {
                for (int i = 0; i < Math.Max(1, produto.Quantidade); i++)
                    etiquetas.Add(produto);
            }

            // Divide em páginas
            for (int i = 0; i < etiquetas.Count; i += etiquetasPorPagina)
            {
                int count = Math.Min(etiquetasPorPagina, etiquetasPorPagina); // Corrigido para usar etiquetasPorPagina
                if (i + count > etiquetas.Count)
                {
                    count = etiquetas.Count - i;
                }
                produtosPorPagina.Add(etiquetas.GetRange(i, count));
            }

            if (produtosPorPagina.Count == 0)
                produtosPorPagina.Add(new List<Produto>());
        }

        private void btnAnterior_Click(object sender, EventArgs e) => MudarPagina(-1);
        private void btnProxima_Click(object sender, EventArgs e) => MudarPagina(1);
        private void btnFechar_Click(object sender, EventArgs e) => this.Close();

        private void MudarPagina(int direcao)
        {
            paginaAtual += direcao;
            paginaAtual = Math.Max(0, Math.Min(paginaAtual, produtosPorPagina.Count - 1));
            DesenharVisualizacao();
            AtualizarBotoes();
        }

        private void AtualizarBotoes()
        {
            btnAnterior.Enabled = paginaAtual > 0;
            btnProxima.Enabled = paginaAtual < produtosPorPagina.Count - 1;
            lblInfo.Text = $"Página {paginaAtual + 1} de {produtosPorPagina.Count}";
        }

        //private void DesenharVisualizacao()
        //{
        //    panelVisualizacao.Controls.Clear();

        //    // Escala para visualização (pixels por mm)
        //    float escala = 3.78f;

        //    // Calcula dimensões totais da página
        //    float larguraTotalMm = (configuracaoEtiqueta.NumColunas * template.Largura) +
        //                           ((configuracaoEtiqueta.NumColunas - 1) * configuracaoEtiqueta.EspacamentoColunas) +
        //                           configuracaoEtiqueta.MargemEsquerda + configuracaoEtiqueta.MargemDireita;

        //    float alturaTotalMm = (configuracaoEtiqueta.NumLinhas * template.Altura) +
        //                          ((configuracaoEtiqueta.NumLinhas - 1) * configuracaoEtiqueta.EspacamentoLinhas) +
        //                          configuracaoEtiqueta.MargemSuperior + configuracaoEtiqueta.MargemInferior;

        //    int larguraPagina = (int)(larguraTotalMm * escala);
        //    int alturaPagina = (int)(alturaTotalMm * escala);

        //    Bitmap bmp = new Bitmap(larguraPagina, alturaPagina);

        //    using (Graphics g = Graphics.FromImage(bmp))
        //    {
        //        g.Clear(Color.White);
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        //        DesenharPaginaEtiquetas(g, escala);
        //    }
        //    int xCentralizado = Math.Max(0, (panelVisualizacao.Width - bmp.Width) / 2);

        //    // ⭐ NOVO: Centralização vertical
        //    int yCentralizado = Math.Max(0, (panelVisualizacao.Height - bmp.Height) / 2);

        //    // ... (PictureBox code)
        //    PictureBox pic = new PictureBox
        //    {
        //        Image = bmp,
        //        SizeMode = PictureBoxSizeMode.AutoSize,
        //        Location = new Point(
        //            //Math.Max(0, (panelVisualizacao.Width - bmp.Width) / 2),
        //            //20
        //            xCentralizado,
        //            yCentralizado
        //        )
        //    };

        //    panelVisualizacao.Controls.Add(pic);
        //    AtualizarBotoes();
        //}

        private void DesenharVisualizacao()
        {
            panelVisualizacao.Controls.Clear();

            // Escala base (96 DPI aprox.) multiplicada pelo fator de Zoom definido pelo usuário
            // zoomEscala deve ser uma variável global na classe (ex: private float zoomEscala = 1.0f;)
            float escalaBase = 3.78f;
            float escalaComZoom = escalaBase * zoomEscala;

            // Calcula dimensões totais da página em milímetros
            float larguraTotalMm = (configuracaoEtiqueta.NumColunas * template.Largura) +
                                   ((configuracaoEtiqueta.NumColunas - 1) * configuracaoEtiqueta.EspacamentoColunas) +
                                   configuracaoEtiqueta.MargemEsquerda + configuracaoEtiqueta.MargemDireita;

            float alturaTotalMm = (configuracaoEtiqueta.NumLinhas * template.Altura) +
                                  ((configuracaoEtiqueta.NumLinhas - 1) * configuracaoEtiqueta.EspacamentoLinhas) +
                                  configuracaoEtiqueta.MargemSuperior + configuracaoEtiqueta.MargemInferior;

            // Converte MM para Pixels considerando o Zoom
            int larguraPaginaPx = (int)Math.Ceiling(larguraTotalMm * escalaComZoom);
            int alturaPaginaPx = (int)Math.Ceiling(alturaTotalMm * escalaComZoom);

            // Cria o bitmap com a nova resolução
            Bitmap bmp = new Bitmap(larguraPaginaPx, alturaPaginaPx);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                // Configurações de alta qualidade para visualização e bipagem
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Desenha as etiquetas usando a escala com zoom
                DesenharPaginaEtiquetas(g, escalaComZoom);
            }

            // Centralização lógica
            int xPos = Math.Max(0, (panelVisualizacao.Width - bmp.Width) / 2);
            int yPos = Math.Max(0, (panelVisualizacao.Height - bmp.Height) / 2);

            PictureBox pic = new PictureBox
            {
                Image = bmp,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(xPos, yPos)
            };

            panelVisualizacao.Controls.Add(pic);

            // Atualiza label de informações (ex: Página 1 de 5 - Zoom 120%)
            lblInfo.Text = $"Página {paginaAtual + 1} de {produtosPorPagina.Count} ({Math.Round(zoomEscala * 100)}%)";
            AtualizarBotoes();
        }



        // Renomeado para DesenharPaginaEtiquetas para diferenciar da impressão
        private void DesenharPaginaEtiquetas(Graphics g, float escala)
        {
            float margemEsquerda = configuracaoEtiqueta.MargemEsquerda * escala;
            float margemSuperior = configuracaoEtiqueta.MargemSuperior * escala;
            float espacamentoColunas = configuracaoEtiqueta.EspacamentoColunas * escala;
            float espacamentoLinhas = configuracaoEtiqueta.EspacamentoLinhas * escala;

            int numColunas = Math.Max(1, configuracaoEtiqueta.NumColunas);
            int numLinhas = Math.Max(1, configuracaoEtiqueta.NumLinhas);

            float larguraEtiqueta = template.Largura * escala;
            float alturaEtiqueta = template.Altura * escala;

            var produtosDaPagina = produtosPorPagina[paginaAtual];
            int produtoIndex = 0;

            // Desenha grid de etiquetas
            for (int linha = 0; linha < numLinhas && produtoIndex < produtosDaPagina.Count; linha++)
            {
                for (int coluna = 0; coluna < numColunas && produtoIndex < produtosDaPagina.Count; coluna++)
                {
                    float x = margemEsquerda + coluna * (larguraEtiqueta + espacamentoColunas);
                    float y = margemSuperior + linha * (alturaEtiqueta + espacamentoLinhas);

                    // Reusa a função de desenho da etiqueta (para visualização)
                    DesenharEtiqueta(g, produtosDaPagina[produtoIndex], x, y, escala);
                    produtoIndex++;
                }
            }
        }

        // ==========================================
        // ⭐ CONFIGURAÇÃO DE IMPRESSÃO (CORREÇÃO 1)
        // ==========================================

        private void ConfigurarDocumentoImpressao(PrintDocument printDoc)
        {
            // 1. Calcule a dimensão total necessária do papel (em MM)
            float larguraTotalMm = (configuracaoEtiqueta.NumColunas * configuracaoEtiqueta.LarguraEtiqueta) +
                                   ((configuracaoEtiqueta.NumColunas - 1) * configuracaoEtiqueta.EspacamentoColunas) +
                                   configuracaoEtiqueta.MargemEsquerda + configuracaoEtiqueta.MargemDireita;

            float alturaTotalMm = (configuracaoEtiqueta.NumLinhas * configuracaoEtiqueta.AlturaEtiqueta) +
                                  ((configuracaoEtiqueta.NumLinhas - 1) * configuracaoEtiqueta.EspacamentoLinhas) +
                                  configuracaoEtiqueta.MargemSuperior + configuracaoEtiqueta.MargemInferior;

            // 2. Converta para centésimos de polegada usando a função CRÍTICA
            int largura100thInch = MmToHundredthsOfInch(larguraTotalMm);
            int altura100thInch = MmToHundredthsOfInch(alturaTotalMm);

            // 3. Crie o PaperSize Customizado (Custom)
            PaperSize customSize = new PaperSize("Etiqueta Personalizada", largura100thInch, altura100thInch);

            // 4. Aplique as configurações no documento
            try
            {
                // Tenta definir a impressora padrão salva na configuração
                printDoc.PrinterSettings.PrinterName = configuracaoEtiqueta.ImpressoraPadrao;
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show($"A impressora '{configuracaoEtiqueta.ImpressoraPadrao}' não foi encontrada. Usando impressora padrão do sistema.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Aplica o tamanho de papel customizado
            printDoc.DefaultPageSettings.PaperSize = customSize;

            // Zere as margens! É CRÍTICO para impressoras térmicas.
            printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        }

        // ==========================================
        // ⭐ MÉTODO DE IMPRESSÃO (Revisado - Chamando a configuração)
        // ==========================================
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (produtosPorPagina == null || produtosPorPagina.Count == 0 || configuracaoEtiqueta == null)
            {
                MessageBox.Show("Não há dados ou configurações para imprimir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // PASSO 1: Configurar tamanho do papel e margens
                ConfigurarDocumentoImpressao(printDocument1);

                // Reinicia a paginação para começar do zero na impressão
                paginaAtual = 0;

                // PASSO 2: Chama a impressão, que irá acionar o evento PrintDoc_PrintPage
                printDocument1.Print();

                MessageBox.Show("Impressão enviada com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a impressão: {ex.Message}", "Erro de Impressão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // ⭐ EVENTO PRINTPAGE (CORREÇÃO 2)
        // ==========================================
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // CRÍTICO: Força o objeto Graphics a usar Milímetros como unidade de medida.
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;

            // Chamamos a função de desenho com escala = 1.0f para usar as dimensões em MM diretamente.
            DesenharPaginaEtiquetas(e.Graphics, 1.0f);

            // Lógica de Paginação
            paginaAtual++;
            e.HasMorePages = (paginaAtual < produtosPorPagina.Count);
        }

        // ==========================================
        // DESENHO DE ETIQUETA (REVISÃO PARA FONTES)
        // ==========================================
        private void DesenharEtiqueta(Graphics g, Produto produto, float offsetX, float offsetY, float escala)
        {
            // Área da etiqueta
            RectangleF areaEtiqueta = new RectangleF(
        offsetX,
        offsetY,
        template.Largura * escala,
        template.Altura * escala
    );

            // ⭐ CORREÇÃO APLICADA AQUI:
            // Desenha borda da etiqueta APENAS na visualização de tela (escala > 1.0f).
            // Remove a borda para impressão (escala = 1.0f).
            if (escala > 1.0f)
            {
                g.DrawRectangle(Pens.LightGray,
                    areaEtiqueta.X, areaEtiqueta.Y,
                    areaEtiqueta.Width, areaEtiqueta.Height);
            }

            // Desenha cada elemento
            foreach (var elem in template.Elementos)
            {
                RectangleF bounds = new RectangleF(
                    offsetX + (elem.Bounds.X * escala),
                    offsetY + (elem.Bounds.Y * escala),
                    elem.Bounds.Width * escala,
                    elem.Bounds.Height * escala
                );

                // Garante que não desenha fora da etiqueta
                bounds.Intersect(areaEtiqueta);

                if (bounds.Width <= 0 || bounds.Height <= 0)
                    continue;

                // Reusa a função, ela agora deve lidar com a diferença de escala
                DesenharElemento(g, elem, produto, bounds, escala);
            }
        }

        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, Produto produto, RectangleF bounds, float escala)
        {
            // ⭐ APLICAR ROTAÇÃO se elemento tiver rotação
            GraphicsState state = null;
            if (elem.Rotacao != 0)
            {
                state = g.Save();  // Salvar estado

                // Transladar para o centro
                PointF centro = new PointF(
                    bounds.X + bounds.Width / 2f,
                    bounds.Y + bounds.Height / 2f
                );
                g.TranslateTransform(centro.X, centro.Y);

                // Rotacionar
                g.RotateTransform(elem.Rotacao);

                // Voltar
                g.TranslateTransform(-centro.X, -centro.Y);
            }

            // ⭐ CORREÇÃO AQUI: Tamanho da Fonte.
            // Para impressão (escala=1.0f), usamos o tamanho original (em Points).
            // Para visualização, usamos a escala.
            float tamanhoFonte;
            if (escala == 1.0f)
            {
                // Impressão: Usamos o tamanho original da fonte em Points.
                tamanhoFonte = elem.Fonte.Size;
            }
            else
            {
                // Visualização: Aplicamos a escala visual (mantendo a aproximação original)
                tamanhoFonte = elem.Fonte.Size;
            }

            // ⭐ NOVO: Desenhar cor de fundo se definida
            if (elem.CorFundo.HasValue && elem.CorFundo.Value != Color.Transparent)
            {
                using (SolidBrush fundoBrush = new SolidBrush(elem.CorFundo.Value))
                {
                    g.FillRectangle(fundoBrush, bounds);
                }
            }

            // A unidade GraphicsUnit.Point é a correta para fontes.
            using (Font fonte = new Font(elem.Fonte.FontFamily, tamanhoFonte, elem.Fonte.Style, GraphicsUnit.Point))
            using (SolidBrush brush = new SolidBrush(elem.Cor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = elem.Alinhamento,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                switch (elem.Tipo)
                {
                    case TipoElemento.Texto:
                        g.DrawString(elem.Conteudo ?? "Texto", fonte, brush, bounds, sf);
                        break;

                    case TipoElemento.Campo:
                        string valor = ObterValorCampo(elem.Conteudo, produto);
                        g.DrawString(valor, fonte, brush, bounds, sf);
                        break;

                    case TipoElemento.CodigoBarras:
                        // ⭐ ATUALIZADO: Agora suporta diferentes campos de código
                        string codigoBarras = ObterCodigoBarras(elem.Conteudo, produto);
                        DesenharCodigoBarras(g, codigoBarras, bounds, escala);
                        break;

                    case TipoElemento.Imagem:
                        if (elem.Imagem != null)
                            g.DrawImage(elem.Imagem, bounds);
                        break;
                }
            }

            // ⭐ RESTAURAR estado gráfico após desenhar
            if (state != null)
            {
                g.Restore(state);
            }
        }

        // ⭐ NOVO MÉTODO: Obtém o valor correto para o código de barras
        private string ObterCodigoBarras(string campo, Produto produto)
        {
            if (produto == null) return "";

            switch (campo)
            {
                case "CodigoMercadoria":
                    return produto.Codigo ?? "";
                case "CodFabricante":
                    return produto.CodFabricante ?? "";
                case "CodBarras":
                    return produto.CodBarras ?? "";
                case "CodBarras_Grade":
                    return produto.CodBarras_Grade ?? "";
                default:
                    return produto.Codigo ?? "";
            }
        }

        // ... (Seu código original ObterValorCampo)
        private string ObterValorCampo(string campo, Produto produto)
        {
            if (produto == null) return $"[{campo}]";

            switch (campo)
            {
                // Campos originais
                case "Nome":
                    return produto.Nome ?? "";
                case "Codigo":
                    return produto.Codigo ?? "";
                case "Preco":
                    return produto.Preco.ToString("F2");
                case "Quantidade":
                    return produto.Quantidade.ToString();
                case "CodFabricante":
                    return produto.CodFabricante ?? "";

                // ⭐ NOVOS CAMPOS - Usando propriedades reais da classe Produto
                case "Mercadoria":
                    return produto.Nome ?? "";
                case "CodigoMercadoria":
                    return produto.Codigo ?? "";
                case "CodBarras":
                    return produto.CodBarras ?? "";
                case "PrecoVenda":
                    return produto.PrecoVenda > 0 ? produto.PrecoVenda.ToString("F2") : produto.Preco.ToString("F2");
                case "VendaA":
                    return produto.VendaA > 0 ? produto.VendaA.ToString("F2") : "-";
                case "VendaB":
                    return produto.VendaB > 0 ? produto.VendaB.ToString("F2") : "-";
                case "VendaC":
                    return produto.VendaC > 0 ? produto.VendaC.ToString("F2") : "-";
                case "VendaD":
                    return produto.VendaD > 0 ? produto.VendaD.ToString("F2") : "-";
                case "VendaE":
                    return produto.VendaE > 0 ? produto.VendaE.ToString("F2") : "-";
                case "Fornecedor":
                    return produto.Fornecedor ?? "";
                case "Fabricante":
                    return produto.Fabricante ?? "";
                case "Grupo":
                    return produto.Grupo ?? "";
                case "Prateleira":
                    return produto.Prateleira ?? "";
                case "Garantia":
                    return produto.Garantia ?? "";
                case "Tam":
                    return produto.Tam ?? "";
                case "Cores":
                    return produto.Cores ?? "";
                case "CodBarras_Grade":
                    return produto.CodBarras_Grade ?? "";

                // ⭐ CAMPOS DE PROMOÇÃO
                case "PrecoOriginal":
                    return produto.PrecoOriginal.HasValue ? produto.PrecoOriginal.Value.ToString("F2") : "";
                case "PrecoPromocional":
                    return produto.PrecoPromocional.HasValue ? produto.PrecoPromocional.Value.ToString("F2") : "";



                default:
                    return "";
            }
        }

        // ... (Seu código original DesenharCodigoBarras)
        //private void DesenharCodigoBarras(Graphics g, string codigo, RectangleF bounds, float escala)
        //{
        //    // Limpa o código (remove caracteres não numéricos)
        //    string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));

        //    if (string.IsNullOrEmpty(codigoLimpo))
        //    {
        //        g.DrawString("[SEM CÓDIGO]", new Font("Arial", bounds.Height * 0.15f), Brushes.Gray, bounds);
        //        return;
        //    }

        //    try
        //    {
        //        Barcode b = new Barcode();

        //        int larguraPixels, alturaPixels;

        //        if (escala == 1.0f)
        //        {
        //            // IMPRESSÃO: bounds está em MM, usa DPI da impressora
        //            float dpiX = g.DpiX;
        //            float dpiY = g.DpiY;

        //            larguraPixels = (int)Math.Round((bounds.Width / 25.4f) * dpiX);
        //            alturaPixels = (int)Math.Round((bounds.Height / 25.4f) * dpiY);
        //        }
        //        else
        //        {
        //            // VISUALIZAÇÃO: bounds JÁ está em pixels, usa direto
        //            float qualityFactor = 2.0f;

        //            larguraPixels = (int)Math.Round(bounds.Width * qualityFactor);
        //            alturaPixels = (int)Math.Round(bounds.Height * qualityFactor);
        //        }

        //        if (larguraPixels <= 1 || alturaPixels <= 1)
        //        {
        //            throw new Exception("Dimensões de código de barras inválidas.");
        //        }

        //        // Configurações do código de barras
        //        b.Width = larguraPixels;
        //        b.Height = alturaPixels;

        //        //Aparecer o digito do código de barras na impressão
        //        b.IncludeLabel = false;
        //        //
        //        b.Alignment = AlignmentPositions.Center;
        //        b.ForeColor = SKColors.Black;
        //        b.BackColor = SKColors.White;

        //        // Gera o código de barras
        //        using (SKImage skImage = b.Encode(BarcodeStandard.Type.Code128, codigoLimpo))
        //        {
        //            if (skImage == null)
        //            {
        //                throw new Exception("Falha ao gerar o SKImage do código de barras.");
        //            }

        //            // Converte para System.Drawing.Image
        //            using (SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
        //            {
        //                if (skData == null)
        //                {
        //                    throw new Exception("Falha ao codificar SKImage para SKData.");
        //                }

        //                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        //                {
        //                    skData.SaveTo(ms);
        //                    ms.Seek(0, System.IO.SeekOrigin.Begin);

        //                    using (System.Drawing.Image barcodeImage = System.Drawing.Image.FromStream(ms))
        //                    {
        //                        // Desenha a imagem no retângulo especificado
        //                        g.DrawImage(barcodeImage, bounds);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Tratamento de erro
        //        using (Font fontErro = new Font("Arial", bounds.Height * 0.10f))
        //        {
        //            StringFormat sf = new StringFormat
        //            {
        //                Alignment = StringAlignment.Center,
        //                LineAlignment = StringAlignment.Center
        //            };
        //            g.DrawString($"ERRO BARCODE: {codigoLimpo} - {ex.Message}",
        //                         fontErro, Brushes.Red, bounds, sf);
        //        }
        //    }
        //}

        private void DesenharCodigoBarras(Graphics g, string codigo, RectangleF bounds, float escala)
        {
            // Limpa o código (mantém apenas o que for essencial para o padrão Code128/EAN)
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => !char.IsControl(c)));

            if (string.IsNullOrEmpty(codigoLimpo))
            {
                g.DrawString("[SEM CÓDIGO]", new Font("Arial", 7), Brushes.Gray, bounds);
                return;
            }

            try
            {
                Barcode b = new Barcode();
                int larguraPixels, alturaPixels;

                if (escala == 1.0f)
                {
                    // MODO IMPRESSÃO: Usa o DPI real da impressora para precisão milimétrica
                    larguraPixels = (int)Math.Round((bounds.Width / 25.4f) * g.DpiX);
                    alturaPixels = (int)Math.Round((bounds.Height / 25.4f) * g.DpiY);
                }
                else
                {
                    // MODO VISUALIZAÇÃO: Usa os pixels da tela + fator de qualidade para bipagem
                    // Multiplicamos por 1.5f para garantir densidade de pixels ao bipar o monitor
                    float qualityMultiplier = 1.5f;
                    larguraPixels = (int)Math.Round(bounds.Width * qualityMultiplier);
                    alturaPixels = (int)Math.Round(bounds.Height * qualityMultiplier);
                }

                // Evita erro de GDI+ com dimensões zeradas
                larguraPixels = Math.Max(10, larguraPixels);
                alturaPixels = Math.Max(10, alturaPixels);

                b.Width = larguraPixels;
                b.Height = alturaPixels;
                b.IncludeLabel = false; // Geralmente o texto vai em um elemento separado no seu template
                b.Alignment = AlignmentPositions.Center;
                b.ForeColor = SKColors.Black;
                b.BackColor = SKColors.White;

                // Gera o código de barras usando SkiaSharp
                using (SKImage skImage = b.Encode(BarcodeStandard.Type.Code128, codigoLimpo))
                {
                    if (skImage == null) throw new Exception("Falha ao gerar SKImage.");

                    using (SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                    using (MemoryStream ms = new MemoryStream(skData.ToArray()))
                    {
                        using (System.Drawing.Image barcodeImage = System.Drawing.Image.FromStream(ms))
                        {
                            // Se for visualização, desativamos o AntiAlias momentaneamente para as barras ficarem nítidas
                            var prevSmoothing = g.SmoothingMode;
                            if (escala > 1.0f) g.SmoothingMode = SmoothingMode.None;

                            g.DrawImage(barcodeImage, bounds);

                            g.SmoothingMode = prevSmoothing;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (Font fontErro = new Font("Arial", 6))
                {
                    g.DrawString("ERR BARCODE", fontErro, Brushes.Red, bounds);
                }
            }
        }


        private void FormImpressao_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                if (e.Delta > 0) zoomEscala *= FATOR_ZOOM;
                else zoomEscala /= FATOR_ZOOM;

                zoomEscala = Math.Max(0.1f, Math.Min(zoomEscala, 10.0f));
                DesenharVisualizacao();
                ((HandledMouseEventArgs)e).Handled = true; // Impede que o scroll mova a barra enquanto faz zoom
            }
        }
    }
}