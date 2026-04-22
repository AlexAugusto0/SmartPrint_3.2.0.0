using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EtiquetaFORNew.Forms
{
    /// <summary>
    /// FormDesign modernizado com configuração de página integrada e funcionalidade completa de elementos
    /// </summary>
    public partial class FormDesignNovo : Form
    {
        #region Campos Privados

        private TemplateEtiqueta template;
        private ConfiguracaoEtiqueta configuracao;
        private string nomeTemplateAtual;

        // Controles do canvas
        private Panel panelCanvas;
        private PictureBox pbCanvas;

        // Controles do painel de configuração
        private Panel panelConfiguracao;
        private Button btnToggleConfig;
        private NumericUpDown numLargura;
        private NumericUpDown numAltura;
        private ComboBox cmbImpressora;
        private ComboBox cmbPapel;
        private NumericUpDown numColunas;
        private NumericUpDown numLinhas;
        private NumericUpDown numEspacamentoColunas;
        private NumericUpDown numEspacamentoLinhas;
        private NumericUpDown numMargemSuperior;
        private NumericUpDown numMargemInferior;
        private NumericUpDown numMargemEsquerda;
        private NumericUpDown numMargemDireita;
        private CheckBox chkPadraoDesativar;
        private Panel panelPropriedades;
        private Button btnAlinharEsquerda;
        private Button btnAlinharCentro;
        private Button btnAlinharDireita;
        private NumericUpDown numTamanhoFonte;
        private CheckBox chkNegrito;
        private CheckBox chkItalico;
        private Button btnCor;
        private Button btnCorFundo;
        private Button btnFundoPreto;
        private Button btnFundoBranco;
        private Button btnFundoTransparente;
        private Label lblPropriedadesElemento;
        private ComboBox cmbFonte;
        //private CheckBox chkBarrasGuarda;

        // Toolbox de elementos
        private Panel panelToolbox;
        private RectangleF boundsIniciaisEmMM;

        private bool rotacionando = false;
        private float anguloInicial = 0f;
        private PointF centroRotacao;

        // Controles de elementos e seleção
        private ElementoEtiqueta elementoSelecionado;
        private bool arrastando = false;
        private bool redimensionando = false;
        private Point pontoInicialMouse;
        private Rectangle boundsIniciais;
        private Point deltaArrasto;
        private int handleSelecionado = -1;
        private int handleSobMouse = -1;

        private List<ElementoEtiqueta> elementosSelecionados = new List<ElementoEtiqueta>();
        private bool selecionandoComRetangulo = false;
        private Point pontoInicialSelecao;
        private Rectangle retanguloSelecao;

        // =========================================================
        // RÉGUA DE ALINHAMENTO (Snap Lines)
        // =========================================================
        /// <summary>
        /// Linhas guia ativas durante o arrasto.
        /// bool = isVertical (true = linha vertical / alinhamento em X)
        /// float = posição em pixels no canvas
        /// </summary>
        private List<(bool isVertical, float posicaoPx)> linhasGuiaAtivas
            = new List<(bool, float)>();

        /// <summary>
        /// Tolerância em pixels para ativar o snap
        /// </summary>
        private const float SNAP_THRESHOLD_PX = 6f;

        /// <summary>
        /// Habilita/desabilita as linhas de alinhamento
        /// </summary>
        private bool snapAtivo = true;
        // =========================================================

        // Constantes
        private const float MM_PARA_PIXEL = 3.78f;
        private float zoom = 1.0f;

        //Pilha que armazena os estados para desfazer do template
        private Stack<string> historicoUndo = new Stack<string>();

        //Limite para não consumir memória infinita
        private const int Max_Undo_Steps = 50;

        #endregion

        #region Construtor e Inicialização

        public FormDesignNovo(TemplateEtiqueta templateInicial, string nomeTemplate = null)
        {
            InitializeComponent();

            this.template = templateInicial ?? new TemplateEtiqueta();
            this.nomeTemplateAtual = nomeTemplate;
            //this.chkBarrasGuarda.CheckedChanged += new System.EventHandler(this.chkBarrasGuarda_CheckedChanged);
            if (this.chkBarrasGuarda != null && this.panelPropriedades != null)
            {
                if (!this.panelPropriedades.Controls.Contains(this.chkBarrasGuarda))
                {
                    this.panelPropriedades.Controls.Add(this.chkBarrasGuarda);
                    // Defina a posição manualmente se necessário para não ficar sobreposto
                    this.chkBarrasGuarda.Location = new Point(10, 450); // Ajuste conforme seu layout
                }
            }

            if (!string.IsNullOrEmpty(nomeTemplate))
            {
                configuracao = ConfiguracaoManager.CarregarConfiguracao(nomeTemplate);
            }

            if (configuracao == null)
            {
                configuracao = new ConfiguracaoEtiqueta
                {
                    NomeEtiqueta = nomeTemplate ?? "Novo Template",
                    LarguraEtiqueta = template.Largura > 0 ? template.Largura : 100,
                    AlturaEtiqueta = template.Altura > 0 ? template.Altura : 30,
                    ImpressoraPadrao = "BTP-L42(D)",
                    NumColunas = 1,
                    NumLinhas = 1,
                    EspacamentoColunas = 0,
                    EspacamentoLinhas = 0,
                    MargemSuperior = 0,
                    MargemInferior = 0,
                    MargemEsquerda = 0,
                    MargemDireita = 0
                };
            }

            template.Largura = configuracao.LarguraEtiqueta;
            template.Altura = configuracao.AlturaEtiqueta;

            ConfigurarFormulario();
        }

        private void FormDesignNovo_Load(object sender, EventArgs e)
        {
            VersaoHelper.DefinirTituloComVersao(this, "Designer de Etiquetas");
            CriarInterface();
            CarregarDadosNaInterface();

            if (btnToggleConfig != null && panelConfiguracao != null)
            {
                btnToggleConfig.Location = new Point(
                    this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                    (this.ClientSize.Height - btnToggleConfig.Height) / 2
                );
            }
        }

        private void ConfigurarFormulario()
        {
            SalvarEstadoHistorico();
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 700);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;
            this.KeyDown += FormDesignNovo_KeyDown;
            
        }

        #endregion

        #region Criação de Interface

        private void CriarInterface()
        {
            // ==================== BARRA SUPERIOR ====================
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(94, 97, 99),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelTop);

            Label lblEmoji = new Label
            {
                Text = "🎨",
                Location = new Point(20, 15),
                Size = new Size(30, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 129, 39)
            };
            panelTop.Controls.Add(lblEmoji);

            Label lblTitulo = new Label
            {
                Text = "DESIGNER DE ETIQUETAS",
                Location = new Point(50, 15),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold | FontStyle.Underline),
                ForeColor = Color.FromArgb(231, 129, 39)
            };
            panelTop.Controls.Add(lblTitulo);

            // ==================== BOTÕES DE AÇÃO ====================
            Panel panelBotoes = new Panel
            {
                Dock = DockStyle.Right,
                Width = 560,
                Height = 60,
                BackColor = Color.Transparent
            };
            panelTop.Controls.Add(panelBotoes);

            Button btnFechar = new Button
            {
                Text = "✕ Fechar",
                Location = new Point(450, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFechar.FlatAppearance.BorderSize = 1;
            btnFechar.FlatAppearance.BorderColor = Color.Black;
            btnFechar.Click += BtnFechar_Click;
            panelBotoes.Controls.Add(btnFechar);

            Button btnPreview = new Button
            {
                Text = "👁 Preview",
                Location = new Point(340, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 1;
            btnPreview.FlatAppearance.BorderColor = Color.Black;
            btnPreview.Click += BtnPreview_Click;
            panelBotoes.Controls.Add(btnPreview);

            Button btnNovo = new Button
            {
                Text = "📄 Novo",
                Location = new Point(230, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 1;
            btnNovo.FlatAppearance.BorderColor = Color.Black;
            btnNovo.Click += BtnNovo_Click;
            panelBotoes.Controls.Add(btnNovo);

            Button btnSalvar = new Button
            {
                Text = "💾 Salvar",
                Location = new Point(120, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 1;
            btnSalvar.FlatAppearance.BorderColor = Color.Black;
            btnSalvar.Click += BtnSalvar_Click;
            panelBotoes.Controls.Add(btnSalvar);

            // =========================================================
            // BOTÃO TOGGLE SNAP LINES
            // =========================================================
            Button btnToggleSnap = new Button
            {
                Text = "📐 Guias ON",
                Location = new Point(10, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
                
            };
            btnToggleSnap.FlatAppearance.BorderSize = 1;
            btnToggleSnap.FlatAppearance.BorderColor = Color.Black;
            btnToggleSnap.Click += (s, ev) =>            
            {
                snapAtivo = !snapAtivo;
                btnToggleSnap.Text = snapAtivo ? "📐 Guias ON" : "📐 Guias OFF";
                btnToggleSnap.BackColor = snapAtivo
                    ? Color.FromArgb(39, 174, 96)
                    : Color.FromArgb(149, 165, 166);
                if (!snapAtivo) linhasGuiaAtivas.Clear();
                pbCanvas?.Invalidate();
            };
            btnToggleSnap.Visible = false;
            panelBotoes.Controls.Add(btnToggleSnap);
            // =========================================================

            // ==================== PAINEL LATERAL DIREITO - CONFIGURAÇÃO ====================
            panelConfiguracao = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(10),
                AutoScroll = true
            };
            this.Controls.Add(panelConfiguracao);

            CriarPainelConfiguracao();

            btnToggleConfig = new Button
            {
                Text = "▶",
                Size = new Size(30, 80),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggleConfig.FlatAppearance.BorderSize = 0;

            this.Controls.Add(btnToggleConfig);
            btnToggleConfig.BringToFront();

            this.Resize += (s, e) =>
            {
                if (btnToggleConfig == null) return;

                if (panelConfiguracao.Width > 0)
                {
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                        (this.ClientSize.Height - btnToggleConfig.Height) / 2
                    );
                }
                else
                {
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - btnToggleConfig.Width - 5,
                        (this.ClientSize.Height - btnToggleConfig.Height) / 2
                    );
                }
            };

            btnToggleConfig.Click += (s, e) =>
            {
                if (panelConfiguracao.Width > 0)
                {
                    panelConfiguracao.Width = 0;
                    btnToggleConfig.Text = "◀";
                    btnToggleConfig.BackColor = Color.FromArgb(52, 152, 219);
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - btnToggleConfig.Width - 5,
                        btnToggleConfig.Location.Y
                    );
                }
                else
                {
                    panelConfiguracao.Width = 350;
                    btnToggleConfig.Text = "▶";
                    btnToggleConfig.BackColor = Color.FromArgb(46, 204, 113);
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                        btnToggleConfig.Location.Y
                    );
                }
            };

            // ==================== PAINEL LATERAL ESQUERDO - TOOLBOX ====================
            panelToolbox = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(10),
                AutoScroll = true,
                AutoScrollMinSize = new Size(0, 800)
            };
            this.Controls.Add(panelToolbox);

            CriarToolbox();

            // ==================== CANVAS CENTRAL ====================
            panelCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(189, 195, 199),
                AutoScroll = true
            };
            panelCanvas.Resize += (s, e) => AtualizarTamanhoCanvas();
            this.Controls.Add(panelCanvas);

            pbCanvas = new PictureBox
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 50),
                Size = new Size(400, 300)
            };
            pbCanvas.Paint += PbCanvas_Paint;
            pbCanvas.MouseDown += PbCanvas_MouseDown;
            pbCanvas.MouseMove += PbCanvas_MouseMove;
            pbCanvas.MouseUp += PbCanvas_MouseUp;
            pbCanvas.MouseWheel += PbCanvas_MouseWheel;

            panelCanvas.Controls.Add(pbCanvas);

            AtualizarTamanhoCanvas();
        }

        #endregion

        #region Painel de Configuração

        private void CriarPainelConfiguracao()
        {
            int yPos = 10;

            Label lblTituloConfig = new Label
            {
                Text = "⚙ CONFIGURAÇÕES DA PÁGINA",
                Location = new Point(10, yPos),
                Size = new Size(320, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelConfiguracao.Controls.Add(lblTituloConfig);
            yPos += 40;

            Panel linha1 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha1);
            yPos += 15;

            Label lblDimensoes = CriarLabelSecao("📐 Dimensões da Etiqueta", yPos);
            panelConfiguracao.Controls.Add(lblDimensoes);
            yPos += 25;

            yPos = CriarCampoNumerico("Largura (mm):", out numLargura, yPos, 1, 500, (decimal)configuracao.LarguraEtiqueta);
            numLargura.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Altura (mm):", out numAltura, yPos, 1, 500, (decimal)configuracao.AlturaEtiqueta);
            numAltura.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos += 10;

            Panel linha2 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha2);
            yPos += 15;

            Label lblImpressao = CriarLabelSecao("🖨️ Impressão", yPos);
            panelConfiguracao.Controls.Add(lblImpressao);
            yPos += 25;

            Label lblImpressora = new Label
            {
                Text = "Impressora:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lblImpressora);

            cmbImpressora = new ComboBox
            {
                Location = new Point(120, yPos - 2),
                Size = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbImpressora.SelectedIndexChanged += CmbImpressora_SelectedIndexChanged;
            panelConfiguracao.Controls.Add(cmbImpressora);
            yPos += 30;

            Label lblPapel = new Label
            {
                Text = "Papel:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lblPapel);

            cmbPapel = new ComboBox
            {
                Location = new Point(120, yPos - 2),
                Size = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPapel.SelectedIndexChanged += CmbPapel_SelectedIndexChanged;
            panelConfiguracao.Controls.Add(cmbPapel);
            yPos += 35;

            Panel linha3 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha3);
            yPos += 15;

            Label lblLayout = CriarLabelSecao("📊 Layout da Página", yPos);
            panelConfiguracao.Controls.Add(lblLayout);
            yPos += 25;

            Label lblColunas = new Label
            {
                Text = "Colunas:",
                Location = new Point(15, yPos),
                Size = new Size(60, 20)
            };
            panelConfiguracao.Controls.Add(lblColunas);

            numColunas = new NumericUpDown
            {
                Location = new Point(80, yPos - 2),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 10,
                Value = configuracao.NumColunas
            };
            numColunas.ValueChanged += (s, e) => AtualizarConfiguracao();
            panelConfiguracao.Controls.Add(numColunas);

            Label lblLinhas = new Label
            {
                Text = "Linhas:",
                Location = new Point(175, yPos),
                Size = new Size(50, 20)
            };
            panelConfiguracao.Controls.Add(lblLinhas);

            numLinhas = new NumericUpDown
            {
                Location = new Point(230, yPos - 2),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 20,
                Value = configuracao.NumLinhas
            };
            numLinhas.ValueChanged += (s, e) => AtualizarConfiguracao();
            panelConfiguracao.Controls.Add(numLinhas);
            yPos += 30;

            Panel linha4 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha4);
            yPos += 15;

            Label lblEspacamento = CriarLabelSecao("↔️ Espaçamentos", yPos);
            panelConfiguracao.Controls.Add(lblEspacamento);
            yPos += 25;

            yPos = CriarCampoNumerico("Entre Colunas (mm):", out numEspacamentoColunas, yPos, 0, 50,
                (decimal)configuracao.EspacamentoColunas, 0.1m);
            numEspacamentoColunas.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Entre Linhas (mm):", out numEspacamentoLinhas, yPos, 0, 50,
                (decimal)configuracao.EspacamentoLinhas, 0.1m);
            numEspacamentoLinhas.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos += 10;

            Panel linha5 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha5);
            yPos += 15;

            Label lblMargens = CriarLabelSecao("📏 Margens da Página", yPos);
            panelConfiguracao.Controls.Add(lblMargens);
            yPos += 25;

            yPos = CriarCampoNumerico("Superior (mm):", out numMargemSuperior, yPos, 0, 50,
                (decimal)configuracao.MargemSuperior, 0.1m);
            numMargemSuperior.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Inferior (mm):", out numMargemInferior, yPos, 0, 50,
                (decimal)configuracao.MargemInferior, 0.1m);
            numMargemInferior.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Esquerda (mm):", out numMargemEsquerda, yPos, 0, 50,
                (decimal)configuracao.MargemEsquerda, 0.1m);
            numMargemEsquerda.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Direita (mm):", out numMargemDireita, yPos, 0, 50,
                (decimal)configuracao.MargemDireita, 0.1m);
            numMargemDireita.ValueChanged += (s, e) => AtualizarConfiguracao();

            Panel linha6 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha6);
            yPos += 15;
        }

        private Label CriarLabelSecao(string texto, int yPos)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(10, yPos),
                Size = new Size(320, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        private int CriarCampoNumerico(string label, out NumericUpDown control, int yPos,
            decimal min, decimal max, decimal valor, decimal increment = 1)
        {
            Label lbl = new Label
            {
                Text = label,
                Location = new Point(15, yPos),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lbl);

            control = new NumericUpDown
            {
                Location = new Point(160, yPos - 2),
                Size = new Size(100, 23),
                Minimum = min,
                Maximum = max,
                Value = valor,
                DecimalPlaces = increment < 1 ? 2 : 0,
                Increment = increment
            };
            panelConfiguracao.Controls.Add(control);

            Label lblUnidade = new Label
            {
                Text = "mm",
                Location = new Point(270, yPos),
                Size = new Size(30, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            panelConfiguracao.Controls.Add(lblUnidade);

            return yPos + 30;
        }

        #endregion

        #region Toolbox

        private void CriarToolbox()
        {
            Label lblTitulo = new Label
            {
                Text = "🧰 ELEMENTOS",
                Location = new Point(10, 10),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelToolbox.Controls.Add(lblTitulo);

            int yPos = 45;

            Label lblCampos = new Label
            {
                Text = "Campos Dinâmicos:",
                Location = new Point(10, yPos),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelToolbox.Controls.Add(lblCampos);
            yPos += 25;

            ComboBox cmbCampos = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            cmbCampos.Items.AddRange(new object[] {
                "Mercadoria", "CodigoMercadoria", "CodFabricante", "CodBarras",
                "PrecoVenda", "VendaA", "VendaB", "VendaC", "VendaD", "VendaE",
                "Fornecedor", "Fabricante", "Grupo", "Prateleira", "Garantia",
                "Tam", "Cores", "CodBarras_Grade", "PrecoOriginal", "PrecoPromocional"
            });
            cmbCampos.SelectedIndexChanged += (s, e) => {
                if (cmbCampos.SelectedItem != null)
                {
                    AdicionarCampo(cmbCampos.SelectedItem.ToString());
                    cmbCampos.SelectedIndex = -1;
                }
            };
            panelToolbox.Controls.Add(cmbCampos);
            yPos += 35;

            Label lblCodigoBarras = new Label
            {
                Text = "Códigos de Barras:",
                Location = new Point(10, yPos),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelToolbox.Controls.Add(lblCodigoBarras);
            yPos += 25;

            ComboBox cmbCodigoBarras = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            cmbCodigoBarras.Items.AddRange(new object[] {
                "CodigoMercadoria", "CodFabricante", "CodBarras", "CodBarras_Grade"
            });
            cmbCodigoBarras.SelectedIndexChanged += (s, e) => {
                if (cmbCodigoBarras.SelectedItem != null)
                {
                    AdicionarCodigoBarras(cmbCodigoBarras.SelectedItem.ToString());
                    cmbCodigoBarras.SelectedIndex = -1;
                }
            };
            panelToolbox.Controls.Add(cmbCodigoBarras);
            yPos += 35;

            Button btnTexto = CriarBotaoElemento("📝 Texto", yPos, () => AdicionarElemento(TipoElemento.Texto));
            yPos += 40;

            Button btnImagem = CriarBotaoElemento("🖼️ Imagem", yPos, () => AdicionarImagem());
            yPos += 40;

            Button btnRemover = CriarBotaoElemento("🗑️ Remover", yPos, () => RemoverElementoSelecionado());
            btnRemover.BackColor = Color.FromArgb(231, 76, 60);
            CriarPainelPropriedades();
        }

        private void CriarPainelPropriedades()
        {
            panelPropriedades = new Panel
            {
                Location = new Point(10, 350),
                Size = new Size(200, 650),
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            panelToolbox.Controls.Add(panelPropriedades);

            int yPos = 10;

            lblPropriedadesElemento = new Label
            {
                Text = "⚙ PROPRIEDADES",
                Location = new Point(10, yPos),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelPropriedades.Controls.Add(lblPropriedadesElemento);
            yPos += 35;

            Label lblNomeElemento = new Label
            {
                Name = "lblNomeElementoAtual",
                Text = "",
                Location = new Point(10, yPos),
                Size = new Size(160, 34),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(230, 126, 34),
                BackColor = Color.FromArgb(255, 243, 224),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(3, 2, 3, 2),
                AutoEllipsis = true,
                Visible = false
            };
            panelPropriedades.Controls.Add(lblNomeElemento);
            yPos += 42;

            Label lblConteudo = new Label
            {
                Name = "lblConteudoTexto",
                Text = "Conteúdo:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray,
                Visible = false
            };
            panelPropriedades.Controls.Add(lblConteudo);
            yPos += 25;

            TextBox txtConteudo = new TextBox
            {
                Name = "txtConteudoElemento",
                Location = new Point(10, yPos),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 9),
                Visible = false
            };
            txtConteudo.TextChanged += (s, e) =>
            {
                if (elementoSelecionado != null && elementoSelecionado.Tipo == TipoElemento.Texto)
                {
                    elementoSelecionado.Conteudo = txtConteudo.Text;
                    pbCanvas.Invalidate();
                }
            };
            panelPropriedades.Controls.Add(txtConteudo);
            yPos += 35;

            Label lblAlinhamento = new Label
            {
                Text = "Alinhamento:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblAlinhamento);
            yPos += 25;

            btnAlinharEsquerda = new Button
            {
                Text = "←",
                Location = new Point(10, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharEsquerda.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharEsquerda.Click += (s, e) => AlterarAlinhamento(StringAlignment.Near);
            panelPropriedades.Controls.Add(btnAlinharEsquerda);

            btnAlinharCentro = new Button
            {
                Text = "←→",
                Location = new Point(65, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharCentro.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharCentro.Click += (s, e) => AlterarAlinhamento(StringAlignment.Center);
            panelPropriedades.Controls.Add(btnAlinharCentro);

            btnAlinharDireita = new Button
            {
                Text = "→",
                Location = new Point(120, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharDireita.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharDireita.Click += (s, e) => AlterarAlinhamento(StringAlignment.Far);
            panelPropriedades.Controls.Add(btnAlinharDireita);
            yPos += 45;

            Label lblFamilia = new Label
            {
                Text = "Família da Fonte:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblFamilia);
            yPos += 25;

            cmbFonte = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            foreach (var fontFamily in FontFamily.Families)
            {
                cmbFonte.Items.Add(fontFamily.Name);
            }
            cmbFonte.SelectedIndexChanged += CmbFonte_SelectedIndexChanged;
            panelPropriedades.Controls.Add(cmbFonte);
            yPos += 35;

            Label lblFonte = new Label
            {
                Text = "Tamanho da Fonte:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblFonte);
            yPos += 25;

            numTamanhoFonte = new NumericUpDown
            {
                Location = new Point(10, yPos),
                Size = new Size(70, 23),
                Minimum = 6,
                Maximum = 72,
                Value = 10
            };
            numTamanhoFonte.ValueChanged += (s, e) => AlterarTamanhoFonte();
            panelPropriedades.Controls.Add(numTamanhoFonte);
            yPos += 35;

            Label lblEstilo = new Label
            {
                Text = "Estilo:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblEstilo);
            yPos += 25;

            chkNegrito = new CheckBox
            {
                Text = "Negrito",
                Location = new Point(10, yPos),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            chkNegrito.CheckedChanged += (s, e) => AlterarEstiloFonte();
            panelPropriedades.Controls.Add(chkNegrito);

            chkItalico = new CheckBox
            {
                Text = "Itálico",
                Location = new Point(95, yPos),
                Size = new Size(75, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };
            chkItalico.CheckedChanged += (s, e) => AlterarEstiloFonte();
            panelPropriedades.Controls.Add(chkItalico);
            yPos += 35;

            Label lblCor = new Label
            {
                Text = "Cor do Texto:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblCor);
            yPos += 25;

            btnCor = new Button
            {
                Text = "Escolher Cor",
                Location = new Point(10, yPos),
                Size = new Size(160, 30),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCor.Click += BtnCor_Click;
            panelPropriedades.Controls.Add(btnCor);
            yPos += 35;

            Label lblCoresRapidas = new Label
            {
                Text = "Atalhos:",
                Location = new Point(10, yPos),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblCoresRapidas);

            Button btnTextoPreto = new Button
            {
                Text = "T▓",
                Location = new Point(85, yPos),
                Size = new Size(40, 25),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnTextoPreto.Click += (s, e) => AplicarCorTexto(Color.Black);
            panelPropriedades.Controls.Add(btnTextoPreto);

            Button btnTextoBranco = new Button
            {
                Text = "T▓",
                Location = new Point(130, yPos),
                Size = new Size(40, 25),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnTextoBranco.Click += (s, e) => AplicarCorTexto(Color.White);
            panelPropriedades.Controls.Add(btnTextoBranco);
            yPos += 35;

            Label lblCorFundo = new Label
            {
                Text = "Cor de Fundo:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblCorFundo);
            yPos += 25;

            btnCorFundo = new Button
            {
                Text = "Escolher Fundo",
                Location = new Point(10, yPos),
                Size = new Size(160, 30),
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCorFundo.Click += BtnCorFundo_Click;
            panelPropriedades.Controls.Add(btnCorFundo);
            yPos += 35;

            chkBarrasGuarda = new CheckBox
            {
                Name = "chkBarrasGuarda",
                Text = "Barras de Guarda",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185), // Azul discreto para destacar funcionalidade
                Visible = false
            };
            chkBarrasGuarda.CheckedChanged += (s, e) =>
            {
                if (elementoSelecionado != null && elementoSelecionado.Tipo == TipoElemento.CodigoBarras)
                {
                    elementoSelecionado.UsarBarrasDeGuarda = chkBarrasGuarda.Checked;
                    pbCanvas.Invalidate();
                }
            };
            panelPropriedades.Controls.Add(chkBarrasGuarda);
            yPos += 30; // Incrementa para não encavalar se você adicionar mais coisas no futuro

            Label lblFundoRapido = new Label
            {
                Text = "Atalhos:",
                Location = new Point(10, yPos),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblFundoRapido);

            btnFundoPreto = new Button
            {
                Location = new Point(65, yPos),
                Size = new Size(30, 25),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnFundoPreto.Click += (s, e) => AplicarCorFundo(Color.Black);
            panelPropriedades.Controls.Add(btnFundoPreto);

            btnFundoBranco = new Button
            {
                Location = new Point(100, yPos),
                Size = new Size(30, 25),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnFundoBranco.Click += (s, e) => AplicarCorFundo(Color.White);
            panelPropriedades.Controls.Add(btnFundoBranco);

            btnFundoTransparente = new Button
            {
                Text = "Ø",
                Location = new Point(135, yPos),
                Size = new Size(30, 25),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnFundoTransparente.Click += (s, e) => AplicarCorFundo(null);
            panelPropriedades.Controls.Add(btnFundoTransparente);
        }

        private Button CriarBotaoElemento(string texto, int yPos, Action onClick)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(10, yPos),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 143, 0),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            panelToolbox.Controls.Add(btn);
            return btn;
        }

        #endregion

        #region Adicionar Elementos

        private void AdicionarElemento(TipoElemento tipo)
        {
            SalvarEstadoHistorico();
            var elemento = new ElementoEtiqueta
            {
                Tipo = tipo,
                NomeFonte = "Arial",
                Fonte = new Font("Arial", 10),
                Cor = Color.Black
            };

            if (tipo == TipoElemento.Texto)
            {
                elemento.Conteudo = "Texto";

                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF tamanhoTexto = g.MeasureString(elemento.Conteudo, elemento.Fonte);
                    int largura = Math.Min((int)(tamanhoTexto.Width / MM_PARA_PIXEL) + 2, (int)template.Largura - 2);
                    int altura = Math.Min((int)(tamanhoTexto.Height / MM_PARA_PIXEL) + 2, (int)template.Altura - 2);
                    elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
                }
            }

            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
            
        }

        private void AdicionarCampo(string campo)
        {
            SalvarEstadoHistorico();
            var elemento = new ElementoEtiqueta
            {
                Tipo = TipoElemento.Campo,
                Conteudo = campo,
                Fonte = new Font("Arial", 8),
                Cor = Color.Black
            };

            string textoExemplo = "[" + campo + "]";
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                SizeF tamanhoTexto = g.MeasureString(textoExemplo, elemento.Fonte);
                int largura = Math.Min((int)(tamanhoTexto.Width / MM_PARA_PIXEL) + 2, (int)template.Largura - 2);
                int altura = Math.Min((int)(tamanhoTexto.Height / MM_PARA_PIXEL) + 2, (int)template.Altura - 2);
                elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
            }

            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
            
        }

        private void AdicionarCodigoBarras(string campoCodigo)
        {
            SalvarEstadoHistorico();
            var elemento = new ElementoEtiqueta
            {
                Tipo = TipoElemento.CodigoBarras,
                Conteudo = campoCodigo,
                Fonte = new Font("Arial", 8),
                Cor = Color.Black
            };

            int largura = Math.Max(10, Math.Min((int)(template.Largura * 0.8f), (int)template.Largura - 2));
            int altura = Math.Max(5, Math.Min((int)(template.Altura * 0.4f), (int)template.Altura - 2));

            elemento.Bounds = new Rectangle(1, 1, largura, altura);
            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
            
        }

        private void AdicionarImagem()
        {
            SalvarEstadoHistorico();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var elemento = new ElementoEtiqueta
                    {
                        Tipo = TipoElemento.Imagem,
                        Imagem = Image.FromFile(ofd.FileName),
                        Bounds = new Rectangle(1, 1, 20, 20)
                    };

                    template.Elementos.Add(elemento);
                    elementoSelecionado = elemento;
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                }
            }
           
        }

        private void RemoverElementoSelecionado()
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado != null)
            {
                var resultado = MessageBox.Show(
                    "Deseja remover o elemento selecionado?",
                    "Confirmar Remoção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    template.Elementos.Remove(elementoSelecionado);
                    elementoSelecionado = null;
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                }
            }
            else
            {
                MessageBox.Show("Nenhum elemento selecionado!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AtualizarPainelPropriedades()
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null)
            {
                panelPropriedades.Visible = false;
                return;
            }

            panelPropriedades.Visible = true;

            var lblNomeAtual = panelPropriedades.Controls.Find("lblNomeElementoAtual", false).FirstOrDefault() as Label;
            if (lblNomeAtual != null)
            {
                string tipoNome;
                switch (elementoSelecionado.Tipo)
                {
                    case TipoElemento.Texto:
                        string textoResumido = elementoSelecionado.Conteudo ?? "";
                        if (textoResumido.Length > 18) textoResumido = textoResumido.Substring(0, 18) + "…";
                        tipoNome = "📝 Texto: \"" + textoResumido + "\"";
                        break;
                    case TipoElemento.Campo:
                        tipoNome = "🏷️ Campo: " + (elementoSelecionado.Conteudo ?? "");
                        break;
                    case TipoElemento.CodigoBarras:
                        tipoNome = "▌ ▌ ▌ Cód.Barras: " + (elementoSelecionado.Conteudo ?? "");
                        break;
                    case TipoElemento.Imagem:
                        tipoNome = "🖼️ Imagem";
                        break;
                    default:
                        tipoNome = elementoSelecionado.Tipo.ToString();
                        break;
                }
                lblNomeAtual.Text = tipoNome;
                lblNomeAtual.Visible = true;
            }

            if (elementoSelecionado.Fonte != null)
            {
                numTamanhoFonte.Value = (decimal)elementoSelecionado.Fonte.Size;
                chkNegrito.Checked = elementoSelecionado.Fonte.Bold;
                chkItalico.Checked = elementoSelecionado.Fonte.Italic;

                string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
                if (cmbFonte.Items.Contains(nomeFonte))
                    cmbFonte.SelectedItem = nomeFonte;
                else
                    cmbFonte.SelectedIndex = -1;
            }

            btnCor.BackColor = elementoSelecionado.Cor;
            btnCor.ForeColor = elementoSelecionado.Cor.GetBrightness() > 0.5 ? Color.Black : Color.White;

            if (elementoSelecionado.CorFundo.HasValue)
            {
                btnCorFundo.BackColor = elementoSelecionado.CorFundo.Value;
                btnCorFundo.ForeColor = elementoSelecionado.CorFundo.Value.GetBrightness() > 0.5 ? Color.Black : Color.White;
            }
            else
            {
                btnCorFundo.BackColor = Color.Transparent;
                btnCorFundo.ForeColor = Color.Black;
            }

            if (chkBarrasGuarda != null)
            {
                // Só aparece se for Código de Barras
                bool ehBarcode = (elementoSelecionado.Tipo == TipoElemento.CodigoBarras);
                chkBarrasGuarda.Visible = ehBarcode;

                if (ehBarcode)
                {
                    // Sincroniza o estado sem disparar o evento de loop se necessário
                    chkBarrasGuarda.Checked = elementoSelecionado.UsarBarrasDeGuarda;
                }
            }


            AtualizarBotoesAlinhamento();
            panelToolbox.ScrollControlIntoView(panelPropriedades);

            var txtConteudo = panelPropriedades.Controls.Find("txtConteudoElemento", false).FirstOrDefault() as TextBox;
            var lblConteudo = panelPropriedades.Controls.Find("lblConteudoTexto", false).FirstOrDefault() as Label;

            if (elementoSelecionado.Tipo == TipoElemento.Texto)
            {
                if (txtConteudo != null) { txtConteudo.Visible = true; txtConteudo.Text = elementoSelecionado.Conteudo ?? "Texto"; }
                if (lblConteudo != null) lblConteudo.Visible = true;
            }
            else
            {
                if (txtConteudo != null) txtConteudo.Visible = false;
                if (lblConteudo != null) lblConteudo.Visible = false;
            }

            if (elementoSelecionado.Tipo == TipoElemento.CodigoBarras)
            {
                chkBarrasGuarda.Visible = true;

                // CORREÇÃO CRÍTICA: Faz a CheckBox refletir o que está salvo no elemento
                chkBarrasGuarda.Checked = elementoSelecionado.UsarBarrasDeGuarda;

                //lblAtalhosBarcode.Visible = true;
                //pnlAtalhosBarcode.Visible = true;
            }

        }

        private void AtualizarBotoesAlinhamento()
        {
            if (elementoSelecionado == null) return;

            btnAlinharEsquerda.BackColor = Color.FromArgb(236, 240, 241);
            btnAlinharCentro.BackColor = Color.FromArgb(236, 240, 241);
            btnAlinharDireita.BackColor = Color.FromArgb(236, 240, 241);

            StringAlignment alinhamento = elementoSelecionado.Alinhamento;
            if (alinhamento == StringAlignment.Near)
                btnAlinharEsquerda.BackColor = Color.FromArgb(52, 152, 219);
            else if (alinhamento == StringAlignment.Center)
                btnAlinharCentro.BackColor = Color.FromArgb(52, 152, 219);
            else if (alinhamento == StringAlignment.Far)
                btnAlinharDireita.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void AlterarAlinhamento(StringAlignment novoAlinhamento)
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null) return;
            elementoSelecionado.Alinhamento = novoAlinhamento;
            AtualizarBotoesAlinhamento();
            pbCanvas.Invalidate();
            
        }

        private void CmbFonte_SelectedIndexChanged(object sender, EventArgs e)
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;
            if (cmbFonte.SelectedItem == null) return;

            string nomeFonte = cmbFonte.SelectedItem.ToString();
            float tamanho = elementoSelecionado.Fonte.Size;
            FontStyle estilo = elementoSelecionado.Fonte.Style;

            try
            {
                elementoSelecionado.NomeFonte = nomeFonte;
                elementoSelecionado.Fonte = new Font(nomeFonte, tamanho, estilo);
                pbCanvas.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao aplicar fonte: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void AlterarTamanhoFonte()
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;
            float novoTamanho = (float)numTamanhoFonte.Value;
            FontStyle estilo = elementoSelecionado.Fonte.Style;
            string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
            elementoSelecionado.Fonte = new Font(nomeFonte, novoTamanho, estilo);
            pbCanvas.Invalidate();            
        }

        private void AlterarEstiloFonte()
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;

            FontStyle estilo = FontStyle.Regular;
            if (chkNegrito.Checked) estilo |= FontStyle.Bold;
            if (chkItalico.Checked) estilo |= FontStyle.Italic;

            string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
            elementoSelecionado.Fonte = new Font(nomeFonte, elementoSelecionado.Fonte.Size, estilo);
            elementoSelecionado.Negrito = chkNegrito.Checked;
            elementoSelecionado.Italico = chkItalico.Checked;
            pbCanvas.Invalidate();            
        }

        private void BtnCor_Click(object sender, EventArgs e)
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null && elementosSelecionados.Count == 0) return;
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = elementoSelecionado?.Cor ?? Color.Black;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    AplicarCorTexto(colorDialog.Color);
            }
            
        }

        private void BtnCorFundo_Click(object sender, EventArgs e)
        {
            SalvarEstadoHistorico();
            if (elementoSelecionado == null && elementosSelecionados.Count == 0) return;
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = elementoSelecionado?.CorFundo ?? Color.Transparent;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    AplicarCorFundo(colorDialog.Color);
            }
            
        }

        private void AplicarCorTexto(Color cor)
        {
            SalvarEstadoHistorico();
            if (elementosSelecionados.Count > 0)
                foreach (var elem in elementosSelecionados) elem.Cor = cor;
            else if (elementoSelecionado != null)
                elementoSelecionado.Cor = cor;

            btnCor.BackColor = cor;
            btnCor.ForeColor = cor.GetBrightness() > 0.5 ? Color.Black : Color.White;
            pbCanvas.Invalidate();
            
        }

        private void AplicarCorFundo(Color? cor)
        {
            SalvarEstadoHistorico();
            if (elementosSelecionados.Count > 0)
                foreach (var elem in elementosSelecionados) elem.CorFundo = cor;
            else if (elementoSelecionado != null)
                elementoSelecionado.CorFundo = cor;

            if (cor.HasValue)
            {
                btnCorFundo.BackColor = cor.Value;
                btnCorFundo.ForeColor = cor.Value.GetBrightness() > 0.5 ? Color.Black : Color.White;
            }
            else
            {
                btnCorFundo.BackColor = Color.Transparent;
                btnCorFundo.ForeColor = Color.Black;
            }
            pbCanvas.Invalidate();            
        }

        #endregion

        #region Régua de Alinhamento (Snap Lines)

        /// <summary>
        /// Calcula as linhas guia de alinhamento durante o arrasto de um elemento.
        /// Compara bordas/centros do elemento arrastado com bordas/centros dos demais
        /// elementos e das bordas da etiqueta. Quando dentro do limiar SNAP_THRESHOLD_PX,
        /// adiciona a linha guia à lista para ser desenhada no Paint.
        /// </summary>
        //private void AtualizarLinhasGuia(RectangleF rectEtiqueta)
        //{
        //    linhasGuiaAtivas.Clear();

        //    if (!snapAtivo) return;
        //    if (elementoSelecionado == null && elementosSelecionados.Count == 0) return;

        //    float scale = MM_PARA_PIXEL * zoom;
        //    float thresholdMM = SNAP_THRESHOLD_PX / scale;

        //    // ------------------------------------------------------------------
        //    // Calcular bounds atual do elemento sendo arrastado (em MM, com delta)
        //    // ------------------------------------------------------------------
        //    float deltaXMM = deltaArrasto.X / scale;
        //    float deltaYMM = deltaArrasto.Y / scale;

        //    RectangleF b;
        //    if (elementoSelecionado != null)
        //    {
        //        b = new RectangleF(
        //            elementoSelecionado.Bounds.X + deltaXMM,
        //            elementoSelecionado.Bounds.Y + deltaYMM,
        //            elementoSelecionado.Bounds.Width,
        //            elementoSelecionado.Bounds.Height);
        //    }
        //    else
        //    {
        //        // seleção múltipla: usar o bounding box do conjunto
        //        float minX = elementosSelecionados.Min(el => el.Bounds.X) + deltaXMM;
        //        float minY = elementosSelecionados.Min(el => el.Bounds.Y) + deltaYMM;
        //        float maxX = elementosSelecionados.Max(el => el.Bounds.Right) + deltaXMM;
        //        float maxY = elementosSelecionados.Max(el => el.Bounds.Bottom) + deltaYMM;
        //        b = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        //    }

        //    // Pontos de interesse do elemento arrastado (em MM)
        //    float[] bX = { b.Left, b.Left + b.Width / 2f, b.Right };
        //    float[] bY = { b.Top,  b.Top  + b.Height / 2f, b.Bottom };

        //    // ------------------------------------------------------------------
        //    // Referências: bordas da etiqueta + bordas dos outros elementos
        //    // ------------------------------------------------------------------
        //    var refX = new List<float> { 0f, configuracao.LarguraEtiqueta / 2f, configuracao.LarguraEtiqueta };
        //    var refY = new List<float> { 0f, configuracao.AlturaEtiqueta / 2f,  configuracao.AlturaEtiqueta };

        //    foreach (var outro in template.Elementos)
        //    {
        //        // Ignorar o próprio elemento (ou os elementos da seleção múltipla)
        //        if (outro == elementoSelecionado) continue;
        //        if (elementosSelecionados.Contains(outro)) continue;

        //        refX.Add(outro.Bounds.X);
        //        refX.Add(outro.Bounds.X + outro.Bounds.Width / 2f);
        //        refX.Add(outro.Bounds.Right);

        //        refY.Add(outro.Bounds.Y);
        //        refY.Add(outro.Bounds.Y + outro.Bounds.Height / 2f);
        //        refY.Add(outro.Bounds.Bottom);
        //    }

        //    // ------------------------------------------------------------------
        //    // Detectar alinhamentos e converter para pixels para desenho
        //    // ------------------------------------------------------------------
        //    var adicionadasX = new HashSet<float>();
        //    var adicionadasY = new HashSet<float>();

        //    foreach (float bx in bX)
        //    {
        //        foreach (float rx in refX)
        //        {
        //            if (Math.Abs(bx - rx) <= thresholdMM)
        //            {
        //                float px = rectEtiqueta.X + rx * scale;
        //                // Evitar linhas duplicadas (arredondado a 1px)
        //                float chave = (float)Math.Round(px);
        //                if (adicionadasX.Add(chave))
        //                    linhasGuiaAtivas.Add((true, px));
        //            }
        //        }
        //    }

        //    foreach (float by in bY)
        //    {
        //        foreach (float ry in refY)
        //        {
        //            if (Math.Abs(by - ry) <= thresholdMM)
        //            {
        //                float py = rectEtiqueta.Y + ry * scale;
        //                float chave = (float)Math.Round(py);
        //                if (adicionadasY.Add(chave))
        //                    linhasGuiaAtivas.Add((false, py));
        //            }
        //        }
        //    }
        //}
        private void AtualizarLinhasGuia(RectangleF rectEtiqueta)
        {
            linhasGuiaAtivas.Clear();

            if (!snapAtivo) return;
            if (elementoSelecionado == null && elementosSelecionados.Count == 0) return;

            float scale = MM_PARA_PIXEL * zoom;
            float thresholdMM = SNAP_THRESHOLD_PX / scale;

            float deltaXMM = deltaArrasto.X / scale;
            float deltaYMM = deltaArrasto.Y / scale;

            // Calculamos o bounding box atual (com o movimento do mouse)
            RectangleF b;
            if (elementoSelecionado != null)
            {
                b = new RectangleF(
                    elementoSelecionado.Bounds.X + deltaXMM,
                    elementoSelecionado.Bounds.Y + deltaYMM,
                    elementoSelecionado.Bounds.Width,
                    elementoSelecionado.Bounds.Height);
            }
            else
            {
                float minX = elementosSelecionados.Min(el => el.Bounds.X) + deltaXMM;
                float minY = elementosSelecionados.Min(el => el.Bounds.Y) + deltaYMM;
                float maxX = elementosSelecionados.Max(el => el.Bounds.Right) + deltaXMM;
                float maxY = elementosSelecionados.Max(el => el.Bounds.Bottom) + deltaYMM;
                b = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }

            // Referências da etiqueta e outros elementos
            var refX = new List<float> { 0f, configuracao.LarguraEtiqueta / 2f, configuracao.LarguraEtiqueta };
            var refY = new List<float> { 0f, configuracao.AlturaEtiqueta / 2f, configuracao.AlturaEtiqueta };
            // ... (seu foreach que popula refX e refY continua igual)

            // VARIÁVEIS PARA O SNAP (MAGNÉTICO)
            float melhorDeltaX = float.MaxValue;
            float melhorDeltaY = float.MaxValue;
            bool snapX = false;
            bool snapY = false;

            // Pontos do elemento arrastado
            float[] bX = { b.Left, b.Left + b.Width / 2f, b.Right };
            float[] bY = { b.Top, b.Top + b.Height / 2f, b.Bottom };

            // --- SNAP VERTICAL (Eixo X) ---
            foreach (float bx in bX)
            {
                foreach (float rx in refX)
                {
                    float diff = rx - bx;
                    if (Math.Abs(diff) <= thresholdMM)
                    {
                        if (Math.Abs(diff) < Math.Abs(melhorDeltaX))
                        {
                            melhorDeltaX = diff;
                            snapX = true;
                            // Guardamos para desenhar a linha depois
                            float px = rectEtiqueta.X + rx * scale;
                            linhasGuiaAtivas.Add((true, px));
                        }
                    }
                }
            }

            // --- SNAP HORIZONTAL (Eixo Y) ---
            foreach (float by in bY)
            {
                foreach (float ry in refY)
                {
                    float diff = ry - by;
                    if (Math.Abs(diff) <= thresholdMM)
                    {
                        if (Math.Abs(diff) < Math.Abs(melhorDeltaY))
                        {
                            melhorDeltaY = diff;
                            snapY = true;
                            float py = rectEtiqueta.Y + ry * scale;
                            linhasGuiaAtivas.Add((false, py));
                        }
                    }
                }
            }

            // APLICAR O MAGNETISMO NO DELTA
            if (snapX)
            {
                // Ajustamos o deltaArrasto para compensar a distância até a linha
                deltaArrasto.X += (int)(melhorDeltaX * scale);
            }
            if (snapY)
            {
                deltaArrasto.Y += (int)(melhorDeltaY * scale);
            }
        }

        /// <summary>
        /// Desenha as linhas guia ativas sobre o canvas.
        /// Chamado no final do PbCanvas_Paint.
        /// </summary>
        private void DesenharLinhasGuia(Graphics g, RectangleF rectEtiqueta)
        {
            if (!snapAtivo || linhasGuiaAtivas.Count == 0) return;

            // Cor magenta semitransparente — visível sobre qualquer fundo
            using (Pen penGuia = new Pen(Color.FromArgb(210, 255, 0, 128), 1f))
            {
                penGuia.DashStyle = DashStyle.Dash;

                foreach (var (isVertical, posPx) in linhasGuiaAtivas)
                {
                    if (isVertical)
                    {
                        // Linha vertical — percorre toda a altura da etiqueta
                        g.DrawLine(penGuia,
                            posPx, rectEtiqueta.Top - 4,
                            posPx, rectEtiqueta.Bottom + 4);
                    }
                    else
                    {
                        // Linha horizontal — percorre toda a largura da etiqueta
                        g.DrawLine(penGuia,
                            rectEtiqueta.Left - 4, posPx,
                            rectEtiqueta.Right + 4, posPx);
                    }
                }
            }

            // Círculos nos cruzamentos de linhas guia para destaque extra
            var verticais = linhasGuiaAtivas.Where(l => l.isVertical).Select(l => l.posicaoPx).ToList();
            var horizontais = linhasGuiaAtivas.Where(l => !l.isVertical).Select(l => l.posicaoPx).ToList();

            if (verticais.Count > 0 && horizontais.Count > 0)
            {
                using (SolidBrush brushPonto = new SolidBrush(Color.FromArgb(230, 255, 0, 128)))
                {
                    foreach (float vx in verticais)
                        foreach (float hy in horizontais)
                            g.FillEllipse(brushPonto, vx - 3, hy - 3, 6, 6);
                }
            }
        }

        #endregion

        #region Desenhar Canvas

        private void PbCanvas_Paint(object sender, PaintEventArgs e)
        {
            SalvarEstadoHistorico();
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            RectangleF rectEtiqueta = new RectangleF(
                25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom
            );

            g.FillRectangle(Brushes.White, rectEtiqueta);
            g.DrawRectangle(new Pen(Color.FromArgb(230, 126, 34), 2), Rectangle.Round(rectEtiqueta));

            if (configuracao.NumColunas > 1 || configuracao.NumLinhas > 1)
                DesenharGrid(g, rectEtiqueta);

            DesenharElementos(g, rectEtiqueta);

            // Retângulo de seleção múltipla
            if (selecionandoComRetangulo)
            {
                using (Pen penSelecao = new Pen(Color.DodgerBlue, 1))
                using (SolidBrush brushSelecao = new SolidBrush(Color.FromArgb(30, Color.DodgerBlue)))
                {
                    penSelecao.DashStyle = DashStyle.Dot;
                    g.FillRectangle(brushSelecao, retanguloSelecao);
                    g.DrawRectangle(penSelecao, retanguloSelecao);
                }
            }

            // =========================================================
            // DESENHAR LINHAS GUIA DE ALINHAMENTO (por último, sobre tudo)
            // =========================================================
            if (arrastando)
                DesenharLinhasGuia(g, rectEtiqueta);            
        }

        private void DesenharGrid(Graphics g, RectangleF rect)
        {
            SalvarEstadoHistorico();
            using (Pen penGrid = new Pen(Color.FromArgb(100, 189, 195, 199), 1))
            {
                penGrid.DashStyle = DashStyle.Dash;

                if (configuracao.NumColunas > 1)
                {
                    float larguraColuna = configuracao.LarguraEtiqueta / configuracao.NumColunas;
                    for (int i = 1; i < configuracao.NumColunas; i++)
                    {
                        float x = rect.X + (larguraColuna * i * MM_PARA_PIXEL);
                        g.DrawLine(penGrid, x, rect.Y, x, rect.Bottom);
                    }
                }

                if (configuracao.NumLinhas > 1)
                {
                    float alturaLinha = configuracao.AlturaEtiqueta / configuracao.NumLinhas;
                    for (int i = 1; i < configuracao.NumLinhas; i++)
                    {
                        float y = rect.Y + (alturaLinha * i * MM_PARA_PIXEL);
                        g.DrawLine(penGrid, rect.X, y, rect.Right, y);
                    }
                }
            }
            
        }

        private void DesenharElementos(Graphics g, RectangleF rectEtiqueta)
        {
            if (template.Elementos.Count == 0)
            {
                string texto = "Adicione elementos usando a toolbox ←";
                using (Font fonteComZoom = new Font(this.Font.FontFamily, this.Font.Size * zoom, this.Font.Style))
                {
                    SizeF tamanho = g.MeasureString(texto, fonteComZoom);
                    g.DrawString(texto, fonteComZoom, Brushes.Gray,
                        rectEtiqueta.X + (rectEtiqueta.Width - tamanho.Width) / 2,
                        rectEtiqueta.Y + (rectEtiqueta.Height - tamanho.Height) / 2);
                }
                return;
            }

            foreach (var elem in template.Elementos)
            {
                SalvarEstadoHistorico();
                DesenharElemento(g, elem, rectEtiqueta, null);

                bool estaSelecionado = (elem == elementoSelecionado) || elementosSelecionados.Contains(elem);

                if (estaSelecionado)
                {
                    Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);

                    if (arrastando)
                    {
                        bounds.X += deltaArrasto.X;
                        bounds.Y += deltaArrasto.Y;
                    }

                    GraphicsState selectionState = null;
                    if (elem.Rotacao != 0)
                    {
                        selectionState = g.Save();
                        PointF centro = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                        g.TranslateTransform(centro.X, centro.Y);
                        g.RotateTransform(elem.Rotacao);
                        g.TranslateTransform(-centro.X, -centro.Y);
                    }

                    using (Pen penSelecao = new Pen(Color.Blue, 2))
                    {
                        penSelecao.DashStyle = DashStyle.Dash;
                        g.DrawRectangle(penSelecao, bounds);
                    }

                    if (elementoSelecionado == elem && elementosSelecionados.Count == 0)
                        DesenharHandles(g, bounds);

                    if (selectionState != null)
                        g.Restore(selectionState);
                }
            }            
        }

        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, RectangleF rectEtiqueta, Produto produto)
        {
            SalvarEstadoHistorico();
            Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);
            if (elem == elementoSelecionado && arrastando && deltaArrasto != Point.Empty)
            {
                bounds.X += deltaArrasto.X;
                bounds.Y += deltaArrasto.Y;
            }

            GraphicsState state = null;
            if (elem.Rotacao != 0)
            {
                state = g.Save();
                PointF centro = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                g.TranslateTransform(centro.X, centro.Y);
                g.RotateTransform(elem.Rotacao);
                g.TranslateTransform(-centro.X, -centro.Y);
            }

            if (elem.CorFundo.HasValue && elem.CorFundo.Value != Color.Transparent)
            {
                using (SolidBrush fundoBrush = new SolidBrush(elem.CorFundo.Value))
                    g.FillRectangle(fundoBrush, bounds);
            }

            switch (elem.Tipo)
            {
                case TipoElemento.Texto:
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    using (Font fonteComZoom = new Font(elem.Fonte.FontFamily, elem.Fonte.Size * zoom, elem.Fonte.Style))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = elem.Alinhamento,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.LineLimit
                        };
                        g.DrawString(elem.Conteudo ?? "Texto", fonteComZoom, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.Campo:
                    string valor = ObterValorCampo(elem.Conteudo, produto);
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    using (Font fonteComZoom = new Font(elem.Fonte.FontFamily, elem.Fonte.Size * zoom, elem.Fonte.Style))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = elem.Alinhamento,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.LineLimit
                        };
                        g.DrawString(valor, fonteComZoom, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.CodigoBarras:
                    string codigoBarras = ObterValorCampo(elem.Conteudo, produto);
                    //DesenharCodigoBarras(g, codigoBarras, bounds);
                    DesenharCodigoBarras(g, codigoBarras, bounds, elem.UsarBarrasDeGuarda);
                    break;

                case TipoElemento.Imagem:
                    if (elem.Imagem != null)
                        g.DrawImage(elem.Imagem, bounds);
                    else
                    {
                        g.FillRectangle(Brushes.LightGray, bounds);
                        using (Font fonteComZoom = new Font("Arial", 8 * zoom, FontStyle.Regular))
                            g.DrawString("Imagem", fonteComZoom, Brushes.Black, bounds);
                    }
                    break;
            }

            g.DrawRectangle(Pens.LightGray, bounds);

            if (state != null)
                g.Restore(state);
            
        }

        #endregion

        #region Métodos Auxiliares de Desenho

        private Rectangle ConverterParaPixels(Rectangle boundsEmMM, RectangleF rectEtiqueta)
        {
            SalvarEstadoHistorico();
            return new Rectangle(
                (int)(rectEtiqueta.X + boundsEmMM.X * MM_PARA_PIXEL * zoom),
                (int)(rectEtiqueta.Y + boundsEmMM.Y * MM_PARA_PIXEL * zoom),
                (int)(boundsEmMM.Width * MM_PARA_PIXEL * zoom),
                (int)(boundsEmMM.Height * MM_PARA_PIXEL * zoom)
            );
            
        }

        private Rectangle ConverterParaMM(Rectangle boundsEmPixels, RectangleF rectEtiqueta)
        {
            const float pixelParaMM = 1f / MM_PARA_PIXEL;
            return new Rectangle(
                (int)((boundsEmPixels.X - rectEtiqueta.X) * pixelParaMM / zoom),
                (int)((boundsEmPixels.Y - rectEtiqueta.Y) * pixelParaMM / zoom),
                (int)(boundsEmPixels.Width * pixelParaMM / zoom),
                (int)(boundsEmPixels.Height * pixelParaMM / zoom)
            );
        }

        private string ObterValorCampo(string campo, Produto produto)
        {
            if (produto == null) return $"[{campo}]";

            switch (campo)
            {
                case "Mercadoria":       return produto.Nome ?? "";
                case "CodigoMercadoria": return produto.Codigo ?? "";
                case "CodFabricante":    return produto.CodFabricante ?? "";
                case "CodBarras":        return produto.CodBarras ?? "";
                case "PrecoVenda":       return produto.Preco.ToString("F2");
                case "VendaA":           return produto.Preco.ToString("F2");
                case "VendaB":           return produto.Preco.ToString("F2");
                case "VendaC":           return produto.Preco.ToString("F2");
                case "VendaD":           return produto.Preco.ToString("F2");
                case "VendaE":           return produto.Preco.ToString("F2");
                case "Fornecedor":       return produto.Nome ?? "";
                case "Fabricante":       return produto.Nome ?? "";
                case "Grupo":            return "";
                case "Prateleira":       return "";
                case "Garantia":         return "";
                case "Tam":              return "";
                case "Cores":            return "";
                case "CodBarras_Grade":  return produto.CodBarras_Grade ?? "";
                case "PrecoOriginal":
                    return produto.PrecoOriginal.HasValue
                        ? produto.PrecoOriginal.Value.ToString("F2")
                        : produto.Preco.ToString("F2");
                case "PrecoPromocional":
                    return produto.PrecoPromocional.HasValue
                        ? produto.PrecoPromocional.Value.ToString("F2")
                        : produto.Preco.ToString("F2");
                default: return "";
            }
        }

        //private void DesenharCodigoBarras(Graphics g, string codigo, Rectangle bounds)
        //{
        //    SalvarEstadoHistorico();
        //    string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));
        //    if (string.IsNullOrEmpty(codigoLimpo)) codigoLimpo = "0000000000";
        //    if (codigoLimpo.Length < 8) codigoLimpo = codigoLimpo.PadLeft(8, '0');

        //    float larguraBarra = (float)bounds.Width / (codigoLimpo.Length * 2);
        //    float alturaBarras = bounds.Height;

        //    for (int i = 0; i < codigoLimpo.Length; i++)
        //    {
        //        int digito = int.Parse(codigoLimpo[i].ToString());
        //        float larguraAtual = (digito % 2 == 0) ? larguraBarra : larguraBarra * 1.5f;
        //        float x = bounds.X + (i * larguraBarra * 2);

        //        using (SolidBrush brush = new SolidBrush(Color.Black))
        //            g.FillRectangle(brush, x, bounds.Y, larguraAtual, alturaBarras);
        //    }

        //}

        private void DesenharCodigoBarras(Graphics g, string codigo, Rectangle bounds, bool usarGuarda)
        {
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), char.IsDigit));
            if (string.IsNullOrEmpty(codigoLimpo)) codigoLimpo = "0000000000";

            // --- 1. CONFIGURAÇÃO DA FONTE (Tamanho Técnico) ---
            // Reduzido para 8% da altura para ser discreto e profissional
            float tamanhoFonte = bounds.Height * 0.08f;
            if (tamanhoFonte < 3.0f) tamanhoFonte = 3.0f;

            // Altura das barras centrais (encurtadas para o texto caber)
            float alturaBarrasNormal = usarGuarda ? bounds.Height * 0.82f : bounds.Height * 0.80f;
            float larguraBarraUnidade = (float)bounds.Width / (codigoLimpo.Length * 2);

            // [Desenho das barras - Loop FOR igual ao anterior]
            for (int i = 0; i < codigoLimpo.Length; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                float larguraAtual = (digito % 2 == 0) ? larguraBarraUnidade : larguraBarraUnidade * 1.5f;
                float x = bounds.X + (i * larguraBarraUnidade * 2);

                float alturaFinal = alturaBarrasNormal;
                if (usarGuarda)
                {
                    // Barras de guarda: Início, Meio e Fim (Altura Total)
                    bool ehBarraGuarda = (i <= 1 || i == codigoLimpo.Length / 2 || i >= codigoLimpo.Length - 2);
                    if (ehBarraGuarda) alturaFinal = bounds.Height;
                }

                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(brush, x, bounds.Y, larguraAtual, alturaFinal);
                }
            }

            // --- 2. POSICIONAMENTO DO TEXTO (Padrão EAN-13 Milimetrado) ---
            if (usarGuarda && codigoLimpo.Length >= 13)
            {
                using (Font fonteEan = new Font("Arial Narrow", tamanhoFonte, FontStyle.Regular))
                {
                    float yTexto = bounds.Y + (bounds.Height * 0.85f);

                    string prefixo = codigoLimpo.Substring(0, 1);
                    string grupo1 = codigoLimpo.Substring(1, 6);
                    string grupo2 = codigoLimpo.Substring(7, 6);

                    // Prefixo: Antes da primeira barra de guarda
                    g.DrawString(prefixo, fonteEan, Brushes.Black, bounds.X - (tamanhoFonte * 1.5f), yTexto);

                    // Grupo 1: Entre a guarda inicial e a central (Posição 15% da largura)
                    g.DrawString(grupo1, fonteEan, Brushes.Black, bounds.X + (bounds.Width * 0.12f), yTexto);

                    // Grupo 2: Entre a guarda central e a final (Posição 55% da largura)
                    g.DrawString(grupo2, fonteEan, Brushes.Black, bounds.X + (bounds.Width * 0.58f), yTexto);
                }
            }
            else
            {
                // Caso não seja EAN-13 ou não use guarda, desenha centralizado e minúsculo
                using (Font fonteEan = new Font("Arial", tamanhoFonte, FontStyle.Regular))
                {
                    SizeF sz = g.MeasureString(codigoLimpo, fonteEan);
                    g.DrawString(codigoLimpo, fonteEan, Brushes.Black,
                                 bounds.X + (bounds.Width / 2) - (sz.Width / 2),
                                 bounds.Y + (bounds.Height * 0.85f));
                }
            }
        }

        private bool PontoEmRetanguloRotacionado(Point ponto, Rectangle bounds, float rotacao)
        {
            if (rotacao == 0) return bounds.Contains(ponto);

            PointF centro = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
            double anguloRad = -rotacao * Math.PI / 180.0;
            float dx = ponto.X - centro.X;
            float dy = ponto.Y - centro.Y;

            float pontoRotacionadoX = centro.X + (float)(dx * Math.Cos(anguloRad) - dy * Math.Sin(anguloRad));
            float pontoRotacionadoY = centro.Y + (float)(dx * Math.Sin(anguloRad) + dy * Math.Cos(anguloRad));

            return bounds.Contains((int)pontoRotacionadoX, (int)pontoRotacionadoY);
        }

        private void DesenharSetaRotacao(Graphics g, PointF centro, float raio, bool sentidoHorario, Color cor)
        {
            using (Pen pen = new Pen(cor, 1.5f))
            {
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                float anguloInicio = sentidoHorario ? -45 : 45;
                float anguloFim    = sentidoHorario ? -135 : 135;

                RectangleF rect = new RectangleF(centro.X - raio, centro.Y - raio, raio * 2, raio * 2);
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddArc(rect, anguloInicio, anguloFim - anguloInicio);
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DesenharHandles(Graphics g, Rectangle bounds)
        {
            SalvarEstadoHistorico();
            const int handleSize = 6;
            Color handleColor    = Color.FromArgb(0, 120, 215);
            Color rotateHandleColor = Color.Black;

            Brush handleBrush = new SolidBrush(handleColor);

            Point[] handlePositions = new Point[]
            {
                new Point(bounds.Left,                        bounds.Top),
                new Point(bounds.Right,                       bounds.Top),
                new Point(bounds.Right,                       bounds.Bottom),
                new Point(bounds.Left,                        bounds.Bottom),
                new Point(bounds.Left + bounds.Width / 2,    bounds.Top),
                new Point(bounds.Right,                       bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2,    bounds.Bottom),
                new Point(bounds.Left,                        bounds.Top + bounds.Height / 2)
            };

            foreach (var pos in handlePositions)
            {
                g.FillRectangle(handleBrush, pos.X - handleSize / 2, pos.Y - handleSize / 2, handleSize, handleSize);
                g.DrawRectangle(Pens.White,  pos.X - handleSize / 2, pos.Y - handleSize / 2, handleSize, handleSize);
            }

            if (handleSobMouse >= 8 && handleSobMouse <= 11)
            {
                int rotateOffset = 12;
                PointF[] rotatePositions = new PointF[]
                {
                    new PointF(bounds.Left  - rotateOffset, bounds.Top    - rotateOffset),
                    new PointF(bounds.Right + rotateOffset, bounds.Top    - rotateOffset),
                    new PointF(bounds.Right + rotateOffset, bounds.Bottom + rotateOffset),
                    new PointF(bounds.Left  - rotateOffset, bounds.Bottom + rotateOffset)
                };
                bool[] sentidosHorarios = new bool[] { false, true, false, true };
                int indexSeta = handleSobMouse - 8;
                DesenharSetaRotacao(g, rotatePositions[indexSeta], 8f, sentidosHorarios[indexSeta], rotateHandleColor);
            }

            handleBrush.Dispose();
            
        }

        private int ObterHandleClicado(Point mouse, Rectangle bounds, float rotacao = 0)
        {
            const int handleSize = 6;
            const int tolerance  = 3;

            Point mouseTransformado = mouse;
            if (rotacao != 0)
            {
                PointF centro = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                double anguloRad = -rotacao * Math.PI / 180.0;
                float dx = mouse.X - centro.X;
                float dy = mouse.Y - centro.Y;
                mouseTransformado = new Point(
                    (int)(centro.X + dx * Math.Cos(anguloRad) - dy * Math.Sin(anguloRad)),
                    (int)(centro.Y + dx * Math.Sin(anguloRad) + dy * Math.Cos(anguloRad))
                );
            }

            int rotateOffset      = 12;
            int rotateClickRadius = 10;

            PointF[] rotatePositions = new PointF[]
            {
                new PointF(bounds.Left  - rotateOffset, bounds.Top    - rotateOffset),
                new PointF(bounds.Right + rotateOffset, bounds.Top    - rotateOffset),
                new PointF(bounds.Right + rotateOffset, bounds.Bottom + rotateOffset),
                new PointF(bounds.Left  - rotateOffset, bounds.Bottom + rotateOffset)
            };

            for (int i = 0; i < rotatePositions.Length; i++)
            {
                float dx = mouseTransformado.X - rotatePositions[i].X;
                float dy = mouseTransformado.Y - rotatePositions[i].Y;
                if ((float)Math.Sqrt(dx * dx + dy * dy) <= rotateClickRadius)
                    return 8 + i;
            }

            Point[] handlePositions = new Point[]
            {
                new Point(bounds.Left,                     bounds.Top),
                new Point(bounds.Right,                    bounds.Top),
                new Point(bounds.Right,                    bounds.Bottom),
                new Point(bounds.Left,                     bounds.Bottom),
                new Point(bounds.Left + bounds.Width / 2,  bounds.Top),
                new Point(bounds.Right,                    bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2,  bounds.Bottom),
                new Point(bounds.Left,                     bounds.Top + bounds.Height / 2)
            };

            for (int i = 0; i < handlePositions.Length; i++)
            {
                Rectangle handleRect = new Rectangle(
                    handlePositions[i].X - handleSize / 2 - tolerance,
                    handlePositions[i].Y - handleSize / 2 - tolerance,
                    handleSize + tolerance * 2,
                    handleSize + tolerance * 2
                );
                if (handleRect.Contains(mouse)) return i;
            }

            return -1;

        }

        #endregion

        #region Eventos do Mouse

        private void PbCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button != MouseButtons.Left) return;
            SalvarEstadoHistorico();
            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            if (elementoSelecionado != null)
            {
                Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds, rectEtiqueta);
                handleSelecionado = ObterHandleClicado(e.Location, bounds, elementoSelecionado.Rotacao);

                if (handleSelecionado >= 0)
                {
                    if (handleSelecionado >= 8 && handleSelecionado <= 11)
                    {
                        rotacionando = true;
                        pontoInicialMouse = e.Location;
                        anguloInicial = elementoSelecionado.Rotacao;
                        centroRotacao = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
                    }
                    else
                    {
                        redimensionando = true;
                        pontoInicialMouse = e.Location;
                        boundsIniciais = bounds;
                        boundsIniciaisEmMM = elementoSelecionado.Bounds;
                    }
                    return;
                }

                if (PontoEmRetanguloRotacionado(e.Location, bounds, elementoSelecionado.Rotacao))
                {
                    arrastando = true;
                    pontoInicialMouse = e.Location;
                    boundsIniciais = bounds;
                    return;
                }
            }

            foreach (var elem in elementosSelecionados)
            {
                Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);
                if (PontoEmRetanguloRotacionado(e.Location, bounds, elem.Rotacao))
                {
                    arrastando = true;
                    pontoInicialMouse = e.Location;
                    return;
                }
            }

            ElementoEtiqueta elementoClicado = null;
            for (int i = template.Elementos.Count - 1; i >= 0; i--)
            {
                Rectangle bounds = ConverterParaPixels(template.Elementos[i].Bounds, rectEtiqueta);
                if (PontoEmRetanguloRotacionado(e.Location, bounds, template.Elementos[i].Rotacao))
                {
                    elementoClicado = template.Elementos[i];
                    break;
                }
            }

            if (elementoClicado != null)
            {
                if (ModifierKeys == Keys.Control)
                {
                    if (elementosSelecionados.Contains(elementoClicado))
                        elementosSelecionados.Remove(elementoClicado);
                    else
                    {
                        elementosSelecionados.Add(elementoClicado);
                        elementoSelecionado = null;
                    }
                }
                else
                {
                    elementoSelecionado = elementoClicado;
                    elementosSelecionados.Clear();
                    pontoInicialMouse = e.Location;

                    Rectangle bounds = ConverterParaPixels(elementoClicado.Bounds, rectEtiqueta);
                    boundsIniciais = bounds;
                    handleSelecionado = ObterHandleClicado(e.Location, bounds, elementoSelecionado.Rotacao);

                    if (handleSelecionado >= 0) redimensionando = true;
                    else arrastando = true;
                }

                AtualizarPainelPropriedades();
                pbCanvas.Invalidate();
                return;
            }

            elementoSelecionado = null;
            elementosSelecionados.Clear();
            selecionandoComRetangulo = true;
            pontoInicialSelecao = e.Location;
            retanguloSelecao = new Rectangle(e.Location, Size.Empty);

            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
        }

        private void PbCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            if (selecionandoComRetangulo)
            {
                int x = Math.Min(pontoInicialSelecao.X, e.X);
                int y = Math.Min(pontoInicialSelecao.Y, e.Y);
                retanguloSelecao = new Rectangle(x, y,
                    Math.Abs(e.X - pontoInicialSelecao.X),
                    Math.Abs(e.Y - pontoInicialSelecao.Y));
                pbCanvas.Invalidate();
                return;
            }

            if (redimensionando && elementoSelecionado != null)
            {
                float deltaXPixels = e.X - pontoInicialMouse.X;
                float deltaYPixels = e.Y - pontoInicialMouse.Y;

                if (elementoSelecionado.Rotacao != 0)
                {
                    double anguloRad = -elementoSelecionado.Rotacao * Math.PI / 180.0;
                    float dxR = (float)(deltaXPixels * Math.Cos(anguloRad) - deltaYPixels * Math.Sin(anguloRad));
                    float dyR = (float)(deltaXPixels * Math.Sin(anguloRad) + deltaYPixels * Math.Cos(anguloRad));
                    deltaXPixels = dxR;
                    deltaYPixels = dyR;
                }

                float deltaXMM = deltaXPixels / (MM_PARA_PIXEL * zoom);
                float deltaYMM = deltaYPixels / (MM_PARA_PIXEL * zoom);
                RectangleF newBoundsMM = boundsIniciaisEmMM;

                switch (handleSelecionado)
                {
                    case 0: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X + deltaXMM, boundsIniciaisEmMM.Y + deltaYMM, boundsIniciaisEmMM.Width - deltaXMM, boundsIniciaisEmMM.Height - deltaYMM); break;
                    case 1: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X, boundsIniciaisEmMM.Y + deltaYMM, boundsIniciaisEmMM.Width + deltaXMM, boundsIniciaisEmMM.Height - deltaYMM); break;
                    case 2: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X, boundsIniciaisEmMM.Y, boundsIniciaisEmMM.Width + deltaXMM, boundsIniciaisEmMM.Height + deltaYMM); break;
                    case 3: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X + deltaXMM, boundsIniciaisEmMM.Y, boundsIniciaisEmMM.Width - deltaXMM, boundsIniciaisEmMM.Height + deltaYMM); break;
                    case 4: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X, boundsIniciaisEmMM.Y + deltaYMM, boundsIniciaisEmMM.Width, boundsIniciaisEmMM.Height - deltaYMM); break;
                    case 5: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X, boundsIniciaisEmMM.Y, boundsIniciaisEmMM.Width + deltaXMM, boundsIniciaisEmMM.Height); break;
                    case 6: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X, boundsIniciaisEmMM.Y, boundsIniciaisEmMM.Width, boundsIniciaisEmMM.Height + deltaYMM); break;
                    case 7: newBoundsMM = new RectangleF(boundsIniciaisEmMM.X + deltaXMM, boundsIniciaisEmMM.Y, boundsIniciaisEmMM.Width - deltaXMM, boundsIniciaisEmMM.Height); break;
                    case 8: case 9: case 10: case 11: break;
                }

                float minWidthMM  = 10 / (MM_PARA_PIXEL * zoom);
                float minHeightMM = 5  / (MM_PARA_PIXEL * zoom);

                if (newBoundsMM.Width >= minWidthMM && newBoundsMM.Height >= minHeightMM)
                {
                    elementoSelecionado.Bounds = Rectangle.Round(newBoundsMM);
                    pbCanvas.Invalidate();
                }
            }
            else if (arrastando)
            {
                deltaArrasto = new Point(
                    e.X - pontoInicialMouse.X,
                    e.Y - pontoInicialMouse.Y
                );

                // =========================================================
                // ATUALIZAR LINHAS GUIA A CADA MOVIMENTO
                // =========================================================
                AtualizarLinhasGuia(rectEtiqueta);
                // =========================================================

                pbCanvas.Invalidate();
            }
            else if (rotacionando && elementoSelecionado != null)
            {
                float dx = e.X - centroRotacao.X;
                float dy = e.Y - centroRotacao.Y;
                float anguloAtual = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

                float dxInicial       = pontoInicialMouse.X - centroRotacao.X;
                float dyInicial       = pontoInicialMouse.Y - centroRotacao.Y;
                float anguloMouseInicial = (float)(Math.Atan2(dyInicial, dxInicial) * 180 / Math.PI);

                float deltaAngulo = anguloAtual - anguloMouseInicial;
                float novaRotacao = anguloInicial + deltaAngulo;

                while (novaRotacao < 0)   novaRotacao += 360;
                while (novaRotacao >= 360) novaRotacao -= 360;

                elementoSelecionado.Rotacao = novaRotacao;
                pbCanvas.Invalidate();
            }
            else
            {
                if (elementoSelecionado != null)
                {
                    Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds, rectEtiqueta);
                    int handle = ObterHandleClicado(e.Location, bounds, elementoSelecionado.Rotacao);

                    if (handleSobMouse != handle)
                    {
                        handleSobMouse = handle;
                        pbCanvas.Invalidate();
                    }

                    if (handle >= 8 && handle <= 11)
                        pbCanvas.Cursor = Cursors.Hand;
                    else if (handle >= 0)
                    {
                        switch (handle)
                        {
                            case 0: case 2: pbCanvas.Cursor = Cursors.SizeNWSE; break;
                            case 1: case 3: pbCanvas.Cursor = Cursors.SizeNESW; break;
                            case 4: case 6: pbCanvas.Cursor = Cursors.SizeNS;   break;
                            case 5: case 7: pbCanvas.Cursor = Cursors.SizeWE;   break;
                        }
                    }
                    else if (PontoEmRetanguloRotacionado(e.Location, bounds, elementoSelecionado.Rotacao))
                        pbCanvas.Cursor = Cursors.SizeAll;
                    else
                        pbCanvas.Cursor = Cursors.Default;
                }
                else
                {
                    if (handleSobMouse != -1) { handleSobMouse = -1; pbCanvas.Invalidate(); }
                    pbCanvas.Cursor = Cursors.Cross;
                }
            }
        }

        //private void PbCanvas_MouseUp(object sender, MouseEventArgs e)
        //{

        //    RectangleF rectEtiqueta = new RectangleF(25, 25,
        //        configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
        //        configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

        //    if (selecionandoComRetangulo)
        //    {
        //        selecionandoComRetangulo = false;
        //        elementosSelecionados.Clear();
        //        foreach (var elemento in template.Elementos)
        //        {
        //            Rectangle bounds = ConverterParaPixels(elemento.Bounds, rectEtiqueta);
        //            if (retanguloSelecao.IntersectsWith(bounds))
        //                elementosSelecionados.Add(elemento);
        //        }
        //        pbCanvas.Invalidate();
        //        return;
        //    }

        //    if (arrastando && deltaArrasto != Point.Empty)
        //    {
        //        SalvarEstadoHistorico();
        //        float scale    = MM_PARA_PIXEL * zoom;
        //        float thresholdMM = SNAP_THRESHOLD_PX / scale;

        //        float deltaXMM = deltaArrasto.X / scale;
        //        float deltaYMM = deltaArrasto.Y / scale;

        //        // =========================================================
        //        // SNAP MAGNÉTICO: ajusta a posição final se houver linha guia ativa
        //        // =========================================================
        //        if (snapAtivo && linhasGuiaAtivas.Count > 0 && elementoSelecionado != null)
        //        {
        //            float novoX = elementoSelecionado.Bounds.X + deltaXMM;
        //            float novoY = elementoSelecionado.Bounds.Y + deltaYMM;
        //            float w     = elementoSelecionado.Bounds.Width;
        //            float h     = elementoSelecionado.Bounds.Height;

        //            float[] bX = { novoX, novoX + w / 2f, novoX + w };
        //            float[] bY = { novoY, novoY + h / 2f, novoY + h };

        //            var refX = new List<float> { 0f, configuracao.LarguraEtiqueta / 2f, configuracao.LarguraEtiqueta };
        //            var refY = new List<float> { 0f, configuracao.AlturaEtiqueta  / 2f, configuracao.AlturaEtiqueta  };
        //            foreach (var outro in template.Elementos)
        //            {
        //                if (outro == elementoSelecionado) continue;
        //                refX.AddRange(new[] { (float)outro.Bounds.X, outro.Bounds.X + outro.Bounds.Width / 2f, (float)outro.Bounds.Right });
        //                refY.AddRange(new[] { (float)outro.Bounds.Y, outro.Bounds.Y + outro.Bounds.Height / 2f, (float)outro.Bounds.Bottom });
        //            }

        //            // Encontrar o snap mais próximo em X
        //            float melhorDX = float.MaxValue;
        //            float ajusteX  = 0;
        //            for (int i = 0; i < bX.Length; i++)
        //            {
        //                foreach (float rx in refX)
        //                {
        //                    float diff = bX[i] - rx;
        //                    if (Math.Abs(diff) < Math.Abs(melhorDX) && Math.Abs(diff) <= thresholdMM)
        //                    {
        //                        melhorDX = diff;
        //                        // offset para mover novoX de modo que bX[i] == rx
        //                        ajusteX = -(diff);
        //                        if (i == 1) ajusteX += 0; // centro já alinha
        //                    }
        //                }
        //            }

        //            // Encontrar o snap mais próximo em Y
        //            float melhorDY = float.MaxValue;
        //            float ajusteY  = 0;
        //            for (int i = 0; i < bY.Length; i++)
        //            {
        //                foreach (float ry in refY)
        //                {
        //                    float diff = bY[i] - ry;
        //                    if (Math.Abs(diff) < Math.Abs(melhorDY) && Math.Abs(diff) <= thresholdMM)
        //                    {
        //                        melhorDY = diff;
        //                        ajusteY  = -(diff);
        //                    }
        //                }
        //            }

        //            if (Math.Abs(melhorDX) <= thresholdMM) deltaXMM += ajusteX;
        //            if (Math.Abs(melhorDY) <= thresholdMM) deltaYMM += ajusteY;
        //        }
        //        // =========================================================

        //        if (elementoSelecionado != null)
        //        {
        //            elementoSelecionado.Bounds = new Rectangle(
        //                (int)(elementoSelecionado.Bounds.X + deltaXMM),
        //                (int)(elementoSelecionado.Bounds.Y + deltaYMM),
        //                elementoSelecionado.Bounds.Width,
        //                elementoSelecionado.Bounds.Height
        //            );
        //        }

        //        foreach (var elemento in elementosSelecionados)
        //        {
        //            elemento.Bounds = new Rectangle(
        //                (int)(elemento.Bounds.X + deltaXMM),
        //                (int)(elemento.Bounds.Y + deltaYMM),
        //                elemento.Bounds.Width,
        //                elemento.Bounds.Height
        //            );
        //        }

        //        deltaArrasto = Point.Empty;
        //        pbCanvas.Invalidate();
        //    }

        //    // =========================================================
        //    // LIMPAR LINHAS GUIA AO SOLTAR O MOUSE
        //    // =========================================================
        //    linhasGuiaAtivas.Clear();
        //    // =========================================================

        //    arrastando     = false;
        //    redimensionando = false;
        //    rotacionando   = false;
        //    handleSelecionado = -1;
        //    pbCanvas.Cursor = Cursors.Default;
        //    pbCanvas.Invalidate();
        //}
        private void PbCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            if (selecionandoComRetangulo)
            {
                selecionandoComRetangulo = false;
                elementosSelecionados.Clear();
                foreach (var elemento in template.Elementos)
                {
                    Rectangle bounds = ConverterParaPixels(elemento.Bounds, rectEtiqueta);
                    if (retanguloSelecao.IntersectsWith(bounds))
                        elementosSelecionados.Add(elemento);
                }
                pbCanvas.Invalidate();
                return;
            }

            if (arrastando && deltaArrasto != Point.Empty)
            {
                float scale = MM_PARA_PIXEL * zoom;

                // 1. Calculamos quanto o mouse moveu em MM
                float moveXMM = deltaArrasto.X / scale;
                float moveYMM = deltaArrasto.Y / scale;

                // 2. Aplicamos aos elementos usando Round (evita o pulo de 1px)
                if (elementoSelecionado != null)
                {
                    elementoSelecionado.Bounds = new Rectangle(
                        (int)Math.Round(elementoSelecionado.Bounds.X + moveXMM),
                        (int)Math.Round(elementoSelecionado.Bounds.Y + moveYMM),
                        elementoSelecionado.Bounds.Width,
                        elementoSelecionado.Bounds.Height
                    );
                }

                foreach (var el in elementosSelecionados)
                {
                    el.Bounds = new Rectangle(
                        (int)Math.Round(el.Bounds.X + moveXMM),
                        (int)Math.Round(el.Bounds.Y + moveYMM),
                        el.Bounds.Width,
                        el.Bounds.Height
                    );
                }

                // 3. SALVAR HISTÓRICO (Aqui o Ctrl+Z registra a posição final correta)
                SalvarEstadoHistorico();

                // 4. ZERAR O DELTA (Essencial para não somar de novo no próximo Paint)
                deltaArrasto = Point.Empty;
                arrastando = false;
                linhasGuiaAtivas.Clear();

                pbCanvas.Invalidate();
            }

            // Limpeza final
            linhasGuiaAtivas.Clear();
            arrastando = false;
            redimensionando = false;
            rotacionando = false;
            handleSelecionado = -1;
            pbCanvas.Cursor = Cursors.Default;
            pbCanvas.Invalidate();
        }

        private void PbCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            
            if (ModifierKeys == Keys.Control)
            {
                zoom += e.Delta > 0 ? 0.1f : -0.1f;
                zoom = Math.Max(0.3f, Math.Min(3.0f, zoom));
                AtualizarTamanhoCanvas();
                pbCanvas.Invalidate();
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
        private void FormEtiqueta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Desfazer();
                e.SuppressKeyPress = true; // Impede o "beep" do Windows
            }
        }

        #endregion

        #region Eventos de Configuração

        private void CarregarDadosNaInterface()
        {
            cmbImpressora.Items.Clear();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                cmbImpressora.Items.Add(printer);

            if (!string.IsNullOrEmpty(configuracao.ImpressoraPadrao) &&
                cmbImpressora.Items.Contains(configuracao.ImpressoraPadrao))
                cmbImpressora.SelectedItem = configuracao.ImpressoraPadrao;
            else if (cmbImpressora.Items.Count > 0)
                cmbImpressora.SelectedIndex = 0;

            CarregarPapeisDaImpressora();
        }

        private void CmbImpressora_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarPapeisDaImpressora();
            AtualizarConfiguracao();
        }

        private void CmbPapel_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarConfiguracao();
        }

        private void CarregarPapeisDaImpressora()
        {
            cmbPapel.Items.Clear();
            if (cmbImpressora.SelectedItem == null) return;

            try
            {
                var printerSettings = new System.Drawing.Printing.PrinterSettings
                {
                    PrinterName = cmbImpressora.SelectedItem.ToString()
                };

                foreach (System.Drawing.Printing.PaperSize papel in printerSettings.PaperSizes)
                    cmbPapel.Items.Add(papel.PaperName);

                if (!string.IsNullOrEmpty(configuracao.PapelPadrao) &&
                    cmbPapel.Items.Cast<object>().Any(x => x.ToString() == configuracao.PapelPadrao))
                    cmbPapel.SelectedItem = configuracao.PapelPadrao;
                else if (cmbPapel.Items.Count > 0)
                    cmbPapel.SelectedIndex = 0;
            }
            catch
            {
                cmbPapel.Items.Add("(Erro ao carregar papéis)");
                cmbPapel.SelectedIndex = 0;
            }
        }

        private void ChkPadraoDesativar_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarEstadoMargens();
        }

        private void chkBarrasGuarda_CheckedChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado != null && elementoSelecionado.Tipo == TipoElemento.CodigoBarras)
            {
                // Salva a preferência no objeto que será serializado/impresso
                elementoSelecionado.UsarBarrasDeGuarda = chkBarrasGuarda.Checked;

                // Força o redesenho imediato no canvas do designer para visualização
                pbCanvas.Invalidate();
            }
        }

        private void AtualizarEstadoMargens()
        {
            bool desabilitado = chkPadraoDesativar.Checked;
            numMargemSuperior.Enabled = !desabilitado;
            numMargemInferior.Enabled = !desabilitado;
            numMargemEsquerda.Enabled = !desabilitado;
            numMargemDireita.Enabled  = !desabilitado;

            if (desabilitado)
            {
                numMargemSuperior.Value = 0;
                numMargemInferior.Value = 0;
                numMargemEsquerda.Value = 0;
                numMargemDireita.Value  = 0;
            }
        }

        private void AtualizarConfiguracao()
        {
            configuracao.LarguraEtiqueta       = (float)numLargura.Value;
            configuracao.AlturaEtiqueta        = (float)numAltura.Value;
            configuracao.ImpressoraPadrao      = cmbImpressora.SelectedItem?.ToString() ?? "";
            configuracao.PapelPadrao           = cmbPapel.SelectedItem?.ToString() ?? "";
            configuracao.NumColunas            = (int)numColunas.Value;
            configuracao.NumLinhas             = (int)numLinhas.Value;
            configuracao.EspacamentoColunas    = (float)numEspacamentoColunas.Value;
            configuracao.EspacamentoLinhas     = (float)numEspacamentoLinhas.Value;
            configuracao.MargemSuperior        = (float)numMargemSuperior.Value;
            configuracao.MargemInferior        = (float)numMargemInferior.Value;
            configuracao.MargemEsquerda        = (float)numMargemEsquerda.Value;
            configuracao.MargemDireita         = (float)numMargemDireita.Value;

            template.Largura = configuracao.LarguraEtiqueta;
            template.Altura  = configuracao.AlturaEtiqueta;

            AtualizarTamanhoCanvas();
            pbCanvas?.Invalidate();
        }

        private void AtualizarTamanhoCanvas()
        {
            if (pbCanvas == null || panelCanvas == null) return;

            int larguraPixels = (int)(configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom);
            int alturaPixels  = (int)(configuracao.AlturaEtiqueta  * MM_PARA_PIXEL * zoom);

            pbCanvas.Size = new Size(larguraPixels + 50, alturaPixels + 50);

            int posX = pbCanvas.Width  < panelCanvas.Width  ? (panelCanvas.Width  - pbCanvas.Width)  / 2 : 0;
            int posY = pbCanvas.Height < panelCanvas.Height ? (panelCanvas.Height - pbCanvas.Height) / 2 : 0;

            pbCanvas.Location = new Point(posX, posY);
        }

        #endregion

        #region Eventos de Teclado

        private void FormDesignNovo_KeyDown(object sender, KeyEventArgs e)
        {
            
            List<ElementoEtiqueta> elementosParaMover = new List<ElementoEtiqueta>();

            if (elementosSelecionados.Count > 0) elementosParaMover.AddRange(elementosSelecionados);
            else if (elementoSelecionado != null) elementosParaMover.Add(elementoSelecionado);

            if (elementosParaMover.Count == 0) return;

            if (e.Control && e.KeyCode == Keys.Z)
            {
                Desfazer();
                e.SuppressKeyPress = true; // Evita o som de "erro" do Windows
            }

            bool houveAlteracao = false;
            int passo = 1;

            switch (e.KeyCode)
            {
                case Keys.Left:  foreach (var el in elementosParaMover) { var p = el.Bounds; p.X -= passo; el.Bounds = p; } houveAlteracao = true; break;
                case Keys.Right: foreach (var el in elementosParaMover) { var p = el.Bounds; p.X += passo; el.Bounds = p; } houveAlteracao = true; break;
                case Keys.Up:    foreach (var el in elementosParaMover) { var p = el.Bounds; p.Y -= passo; el.Bounds = p; } houveAlteracao = true; break;
                case Keys.Down:  foreach (var el in elementosParaMover) { var p = el.Bounds; p.Y += passo; el.Bounds = p; } houveAlteracao = true; break;
                case Keys.Delete:
                    foreach (var el in elementosParaMover) template.Elementos.Remove(el);
                    elementoSelecionado = null;
                    elementosSelecionados.Clear();
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
            }

            if (houveAlteracao)
            {
                pbCanvas.Invalidate();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            
        }

        #endregion

        #region Botões de Ação

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nomeTemplateAtual))
            {
                using (var formNome = new FormNomeTemplate())
                {
                    if (formNome.ShowDialog() == DialogResult.OK)
                    {
                        nomeTemplateAtual = formNome.NomeTemplate;
                        configuracao.NomeEtiqueta = nomeTemplateAtual;
                    }
                    else return;
                }
            }

            if (TemplateManager.SalvarTemplate(template, nomeTemplateAtual))
            {
                ConfiguracaoManager.SalvarConfiguracao(nomeTemplateAtual, configuracao);
                MessageBox.Show(
                    $"Template '{nomeTemplateAtual}' salvo com sucesso!\n\n" +
                    $"✅ Template: {configuracao.LarguraEtiqueta:F1} x {configuracao.AlturaEtiqueta:F1} mm\n" +
                    $"✅ Elementos: {template.Elementos.Count}\n" +
                    $"✅ Layout: {configuracao.NumColunas} col x {configuracao.NumLinhas} lin\n" +
                    $"✅ Configuração vinculada",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Erro ao salvar template!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "Deseja criar um novo template?\n\nAs alterações não salvas serão perdidas.",
                "Novo Template", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                template = new TemplateEtiqueta { Largura = 100, Altura = 30 };
                configuracao = new ConfiguracaoEtiqueta { LarguraEtiqueta = 100, AlturaEtiqueta = 30, NumColunas = 1, NumLinhas = 1 };
                nomeTemplateAtual = null;
                elementoSelecionado = null;
                CarregarDadosNaInterface();
                AtualizarConfiguracao();
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            float larguraPapel = 210;
            float alturaPapel  = 297;
            string nomePapel   = cmbPapel.SelectedItem?.ToString() ?? "A4";

            if (cmbImpressora.SelectedItem != null)
            {
                try
                {
                    PrinterSettings printerSettings = new PrinterSettings
                    {
                        PrinterName = cmbImpressora.SelectedItem.ToString()
                    };
                    foreach (PaperSize paperSize in printerSettings.PaperSizes)
                    {
                        if (paperSize.PaperName == nomePapel)
                        {
                            larguraPapel = (paperSize.Width  / 100f) * 25.4f;
                            alturaPapel  = (paperSize.Height / 100f) * 25.4f;
                            break;
                        }
                    }
                }
                catch { }
            }

            FormPreview formPreview = new FormPreview(template, configuracao, nomePapel, larguraPapel, alturaPapel);
            formPreview.ShowDialog();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void SalvarEstadoHistorico()
        {
            try
            {
                // Agora o template.SalvarParaXml() existe e retorna uma string JSON
                string snapshot = template.SalvarParaXml();

                if (historicoUndo.Count > 0 && historicoUndo.Peek() == snapshot)
                    return;

                historicoUndo.Push(snapshot);

                if (historicoUndo.Count > Max_Undo_Steps)
                {
                    // Lógica para remover o item mais antigo se necessário
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar histórico: " + ex.Message);
            }
        }

        private void Desfazer()
        {
            // 1. Verificamos se há estados suficientes para voltar
            // Se só tiver 1, é o estado inicial, não há o que desfazer.
            if (historicoUndo.Count <= 1) return;

            // 2. Removemos o estado ATUAL da pilha (o que está na tela agora)
            historicoUndo.Pop();

            // 3. Pegamos o estado ANTERIOR sem removê-lo (usando Peek)
            // Assim, se apertar Ctrl+Z de novo, o processo se repete
            string snapshotAnterior = historicoUndo.Peek();

            try
            {
                // 4. Restauramos o template com o estado recuperado
                this.template = TemplateEtiqueta.CarregarDeSnapshot(snapshotAnterior);

                // 5. IMPORTANTE: Limpar seleções
                // Como o objeto 'template' mudou, as referências antigas de 'elementoSelecionado' 
                // agora são de um objeto que não existe mais na memória ativa.
                elementoSelecionado = null;
                elementosSelecionados.Clear();

                // 6. Atualizar a interface
                pbCanvas.Invalidate();
                CarregarDadosNaInterface();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao restaurar snapshot: " + ex.Message);
            }
        }

        #endregion

        #region Propriedades Públicas

        public TemplateEtiqueta ObterTemplate() => template;
        public ConfiguracaoEtiqueta ObterConfiguracao() => configuracao;

        #endregion
    }
}