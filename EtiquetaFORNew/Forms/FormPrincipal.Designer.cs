namespace EtiquetaFORNew
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupProduto;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblPreco;
        private System.Windows.Forms.TextBox txtPreco;
        private System.Windows.Forms.Label lblQtd;
        private System.Windows.Forms.NumericUpDown numQtd;
        private System.Windows.Forms.DataGridView dgvProdutos;

        // ⭐ NOVOS CONTROLES PARA GERENCIAMENTO DE PRODUTOS
        private System.Windows.Forms.CheckBox chkSelecionarTodos;
        private System.Windows.Forms.Button btnLimparTodos;
        private System.Windows.Forms.Button btnCarregar;
  

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            this.groupProduto = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbCor = new System.Windows.Forms.ComboBox();
            this.lblCor = new System.Windows.Forms.Label();
            this.cmbTamanho = new System.Windows.Forms.ComboBox();
            this.lblTamanho = new System.Windows.Forms.Label();
            this.BtnAdicionar2 = new System.Windows.Forms.Button();
            this.cmbBuscaReferencia = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBuscaNome = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBuscaCodigo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numQtd = new System.Windows.Forms.NumericUpDown();
            this.lblQtd = new System.Windows.Forms.Label();
            this.txtPreco = new System.Windows.Forms.TextBox();
            this.lblPreco = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.colSelecionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPreco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemover = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnDesigner = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnCalibracao = new System.Windows.Forms.Button();
            this.btnSincronizar = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnSincronizar2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkSelecionarTodos = new System.Windows.Forms.CheckBox();
            this.btnLimparTodos = new System.Windows.Forms.Button();
            this.btnCarregar = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupProduto.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSincronizar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupProduto
            // 
            this.groupProduto.BackColor = System.Drawing.Color.White;
            this.groupProduto.Controls.Add(this.panel1);
            this.groupProduto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupProduto.Location = new System.Drawing.Point(-1, 3);
            this.groupProduto.Name = "groupProduto";
            this.groupProduto.Size = new System.Drawing.Size(1004, 85);
            this.groupProduto.TabIndex = 1;
            this.groupProduto.TabStop = false;
            this.groupProduto.Text = "Adicionar Produto";
            this.groupProduto.Enter += new System.EventHandler(this.groupProduto_Enter);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbCor);
            this.panel1.Controls.Add(this.lblCor);
            this.panel1.Controls.Add(this.cmbTamanho);
            this.panel1.Controls.Add(this.lblTamanho);
            this.panel1.Controls.Add(this.BtnAdicionar2);
            this.panel1.Controls.Add(this.cmbBuscaReferencia);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cmbBuscaNome);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbBuscaCodigo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numQtd);
            this.panel1.Controls.Add(this.lblQtd);
            this.panel1.Controls.Add(this.txtPreco);
            this.panel1.Controls.Add(this.lblPreco);
            this.panel1.Controls.Add(this.txtCodigo);
            this.panel1.Controls.Add(this.lblCodigo);
            this.panel1.Controls.Add(this.txtNome);
            this.panel1.Controls.Add(this.lblNome);
            this.panel1.Location = new System.Drawing.Point(6, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 51);
            this.panel1.TabIndex = 0;
            // 
            // cmbCor
            // 
            this.cmbCor.FormattingEnabled = true;
            this.cmbCor.Location = new System.Drawing.Point(756, 22);
            this.cmbCor.Name = "cmbCor";
            this.cmbCor.Size = new System.Drawing.Size(74, 23);
            this.cmbCor.TabIndex = 18;
            // 
            // lblCor
            // 
            this.lblCor.AutoSize = true;
            this.lblCor.Location = new System.Drawing.Point(753, 5);
            this.lblCor.Name = "lblCor";
            this.lblCor.Size = new System.Drawing.Size(29, 15);
            this.lblCor.TabIndex = 17;
            this.lblCor.Text = "Cor:";
            // 
            // cmbTamanho
            // 
            this.cmbTamanho.FormattingEnabled = true;
            this.cmbTamanho.Location = new System.Drawing.Point(676, 23);
            this.cmbTamanho.Name = "cmbTamanho";
            this.cmbTamanho.Size = new System.Drawing.Size(74, 23);
            this.cmbTamanho.TabIndex = 16;
            this.cmbTamanho.SelectedIndexChanged += new System.EventHandler(this.cmbTamanho_SelectedIndexChanged);
            // 
            // lblTamanho
            // 
            this.lblTamanho.AutoSize = true;
            this.lblTamanho.Location = new System.Drawing.Point(673, 6);
            this.lblTamanho.Name = "lblTamanho";
            this.lblTamanho.Size = new System.Drawing.Size(33, 15);
            this.lblTamanho.TabIndex = 15;
            this.lblTamanho.Text = "Tam:";
            // 
            // BtnAdicionar2
            // 
            this.BtnAdicionar2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.BtnAdicionar2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnAdicionar2.FlatAppearance.BorderSize = 0;
            this.BtnAdicionar2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAdicionar2.ForeColor = System.Drawing.Color.Black;
            this.BtnAdicionar2.Location = new System.Drawing.Point(891, 21);
            this.BtnAdicionar2.Name = "BtnAdicionar2";
            this.BtnAdicionar2.Size = new System.Drawing.Size(90, 25);
            this.BtnAdicionar2.TabIndex = 14;
            this.BtnAdicionar2.Text = "Adicionar";
            this.BtnAdicionar2.UseVisualStyleBackColor = false;
            this.BtnAdicionar2.Click += new System.EventHandler(this.BtnAdicionar2_Click);
            // 
            // cmbBuscaReferencia
            // 
            this.cmbBuscaReferencia.FormattingEnabled = true;
            this.cmbBuscaReferencia.Location = new System.Drawing.Point(72, 24);
            this.cmbBuscaReferencia.Name = "cmbBuscaReferencia";
            this.cmbBuscaReferencia.Size = new System.Drawing.Size(120, 23);
            this.cmbBuscaReferencia.TabIndex = 11;
            this.cmbBuscaReferencia.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaReferencia_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(69, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "Referência:";
            // 
            // cmbBuscaNome
            // 
            this.cmbBuscaNome.FormattingEnabled = true;
            this.cmbBuscaNome.Location = new System.Drawing.Point(198, 24);
            this.cmbBuscaNome.Name = "cmbBuscaNome";
            this.cmbBuscaNome.Size = new System.Drawing.Size(640, 23);
            this.cmbBuscaNome.TabIndex = 12;
            this.cmbBuscaNome.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaNome_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(195, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Mercadoria:";
            // 
            // cmbBuscaCodigo
            // 
            this.cmbBuscaCodigo.DropDownWidth = 150;
            this.cmbBuscaCodigo.FormattingEnabled = true;
            this.cmbBuscaCodigo.Location = new System.Drawing.Point(3, 24);
            this.cmbBuscaCodigo.Name = "cmbBuscaCodigo";
            this.cmbBuscaCodigo.Size = new System.Drawing.Size(63, 23);
            this.cmbBuscaCodigo.TabIndex = 10;
            this.cmbBuscaCodigo.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaCodigo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Código:";
            // 
            // numQtd
            // 
            this.numQtd.Location = new System.Drawing.Point(845, 23);
            this.numQtd.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numQtd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQtd.Name = "numQtd";
            this.numQtd.Size = new System.Drawing.Size(40, 23);
            this.numQtd.TabIndex = 13;
            this.numQtd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblQtd
            // 
            this.lblQtd.AutoSize = true;
            this.lblQtd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQtd.Location = new System.Drawing.Point(842, 6);
            this.lblQtd.Name = "lblQtd";
            this.lblQtd.Size = new System.Drawing.Size(30, 15);
            this.lblQtd.TabIndex = 6;
            this.lblQtd.Text = "Qtd:";
            this.lblQtd.Click += new System.EventHandler(this.lblQtd_Click);
            // 
            // txtPreco
            // 
            this.txtPreco.Location = new System.Drawing.Point(530, 48);
            this.txtPreco.Name = "txtPreco";
            this.txtPreco.Size = new System.Drawing.Size(80, 23);
            this.txtPreco.TabIndex = 5;
            this.txtPreco.Visible = false;
            // 
            // lblPreco
            // 
            this.lblPreco.AutoSize = true;
            this.lblPreco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreco.Location = new System.Drawing.Point(479, 51);
            this.lblPreco.Name = "lblPreco";
            this.lblPreco.Size = new System.Drawing.Size(40, 15);
            this.lblPreco.TabIndex = 4;
            this.lblPreco.Text = "Preço:";
            this.lblPreco.Visible = false;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(525, 42);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(100, 23);
            this.txtCodigo.TabIndex = 3;
            this.txtCodigo.Visible = false;
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCodigo.Location = new System.Drawing.Point(550, 52);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(49, 15);
            this.lblCodigo.TabIndex = 2;
            this.lblCodigo.Text = "Código:";
            this.lblCodigo.Visible = false;
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(448, 44);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(220, 23);
            this.txtNome.TabIndex = 1;
            this.txtNome.Visible = false;
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNome.Location = new System.Drawing.Point(537, 49);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(43, 15);
            this.lblNome.TabIndex = 0;
            this.lblNome.Text = "Nome:";
            this.lblNome.Visible = false;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.AllowUserToAddRows = false;
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.White;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelecionar,
            this.colNome,
            this.colCodigo,
            this.colPreco,
            this.colQuantidade,
            this.colTam,
            this.colCor,
            this.colRemover});
            this.dgvProdutos.Location = new System.Drawing.Point(12, 164);
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.RowHeadersVisible = false;
            this.dgvProdutos.Size = new System.Drawing.Size(1004, 370);
            this.dgvProdutos.TabIndex = 2;
            this.dgvProdutos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProdutos_CellContentClick);
            // 
            // colSelecionar
            // 
            this.colSelecionar.HeaderText = "Sel.";
            this.colSelecionar.Name = "colSelecionar";
            this.colSelecionar.Width = 35;
            // 
            // colNome
            // 
            this.colNome.HeaderText = "Nome";
            this.colNome.Name = "colNome";
            this.colNome.Width = 500;
            // 
            // colCodigo
            // 
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            // 
            // colPreco
            // 
            this.colPreco.HeaderText = "Preço";
            this.colPreco.Name = "colPreco";
            this.colPreco.Width = 260;
            // 
            // colQuantidade
            // 
            this.colQuantidade.HeaderText = "Qtd";
            this.colQuantidade.Name = "colQuantidade";
            this.colQuantidade.Width = 50;
            // 
            // colTam
            // 
            this.colTam.HeaderText = "Tam";
            this.colTam.Name = "colTam";
            this.colTam.Visible = false;
            this.colTam.Width = 60;
            // 
            // colCor
            // 
            this.colCor.HeaderText = "Cor";
            this.colCor.Name = "colCor";
            this.colCor.Visible = false;
            this.colCor.Width = 80;
            // 
            // colRemover
            // 
            this.colRemover.HeaderText = "Excluir";
            this.colRemover.Name = "colRemover";
            this.colRemover.Text = "X";
            this.colRemover.UseColumnTextForButtonValue = true;
            this.colRemover.Width = 55;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblTitulo.Location = new System.Drawing.Point(80, 6);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(260, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "SISTEMA DE ETIQUETAS";
            // 
            // btnDesigner
            // 
            this.btnDesigner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnDesigner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDesigner.FlatAppearance.BorderSize = 0;
            this.btnDesigner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesigner.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDesigner.ForeColor = System.Drawing.Color.Black;
            this.btnDesigner.Location = new System.Drawing.Point(665, 7);
            this.btnDesigner.Name = "btnDesigner";
            this.btnDesigner.Size = new System.Drawing.Size(180, 30);
            this.btnDesigner.TabIndex = 1;
            this.btnDesigner.Text = "Designer de Etiqueta";
            this.btnDesigner.UseVisualStyleBackColor = false;
            this.btnDesigner.Click += new System.EventHandler(this.btnDesigner_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.FlatAppearance.BorderSize = 0;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.Black;
            this.btnImprimir.Location = new System.Drawing.Point(861, 540);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(150, 30);
            this.btnImprimir.TabIndex = 2;
            this.btnImprimir.Text = "Imprimir Etiquetas";
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.btnCalibracao);
            this.panelTop.Controls.Add(this.btnSincronizar);
            this.panelTop.Controls.Add(this.btnConfig);
            this.panelTop.Controls.Add(this.pictureBox4);
            this.panelTop.Controls.Add(this.btnDesigner);
            this.panelTop.Controls.Add(this.btnSincronizar2);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Location = new System.Drawing.Point(12, 12);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1004, 47);
            this.panelTop.TabIndex = 0;
            // 
            // btnCalibracao
            // 
            this.btnCalibracao.BackColor = System.Drawing.Color.Transparent;
            this.btnCalibracao.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCalibracao.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCalibracao.FlatAppearance.BorderSize = 0;
            this.btnCalibracao.Image = global::EtiquetaFORNew.Properties.Resources.calibracao3;
            this.btnCalibracao.Location = new System.Drawing.Point(851, 6);
            this.btnCalibracao.Name = "btnCalibracao";
            this.btnCalibracao.Size = new System.Drawing.Size(45, 34);
            this.btnCalibracao.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnCalibracao, "Calibração de Etiquetadoras");
            this.btnCalibracao.UseVisualStyleBackColor = false;
            this.btnCalibracao.Click += new System.EventHandler(this.btnCalibracao_Click);
            // 
            // btnSincronizar
            // 
            this.btnSincronizar.BackColor = System.Drawing.Color.Transparent;
            this.btnSincronizar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSincronizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSincronizar.FlatAppearance.BorderSize = 0;
            this.btnSincronizar.Image = global::EtiquetaFORNew.Properties.Resources.Sincronizando3;
            this.btnSincronizar.Location = new System.Drawing.Point(902, 6);
            this.btnSincronizar.Name = "btnSincronizar";
            this.btnSincronizar.Size = new System.Drawing.Size(45, 34);
            this.btnSincronizar.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btnSincronizar, "Sincronização de Dados");
            this.btnSincronizar.UseVisualStyleBackColor = false;
            this.btnSincronizar.Click += new System.EventHandler(this.btnSincronizar_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.Transparent;
            this.btnConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.Image = global::EtiquetaFORNew.Properties.Resources.Engrenagem20x20;
            this.btnConfig.Location = new System.Drawing.Point(953, 6);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(45, 34);
            this.btnConfig.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnConfig, "Configurações");
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = global::EtiquetaFORNew.Properties.Resources.Engrenagem;
            this.pictureBox4.Location = new System.Drawing.Point(571, 7);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(44, 27);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // btnSincronizar2
            // 
            this.btnSincronizar2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSincronizar2.Image = global::EtiquetaFORNew.Properties.Resources.Sincronizando;
            this.btnSincronizar2.Location = new System.Drawing.Point(618, 9);
            this.btnSincronizar2.Name = "btnSincronizar2";
            this.btnSincronizar2.Size = new System.Drawing.Size(31, 24);
            this.btnSincronizar2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSincronizar2.TabIndex = 3;
            this.btnSincronizar2.TabStop = false;
            this.btnSincronizar2.Visible = false;
            this.btnSincronizar2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EtiquetaFORNew.Properties.Resources.icone_novo_2025_PNG1;
            this.pictureBox1.Location = new System.Drawing.Point(13, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(63, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.groupProduto);
            this.panel2.Location = new System.Drawing.Point(12, 57);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1004, 77);
            this.panel2.TabIndex = 3;
            // 
            // chkSelecionarTodos
            // 
            this.chkSelecionarTodos.AutoSize = true;
            this.chkSelecionarTodos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkSelecionarTodos.Location = new System.Drawing.Point(26, 139);
            this.chkSelecionarTodos.Name = "chkSelecionarTodos";
            this.chkSelecionarTodos.Size = new System.Drawing.Size(119, 19);
            this.chkSelecionarTodos.TabIndex = 5;
            this.chkSelecionarTodos.Text = "Selecionar Todos";
            this.chkSelecionarTodos.UseVisualStyleBackColor = true;
            this.chkSelecionarTodos.CheckedChanged += new System.EventHandler(this.chkSelecionarTodos_CheckedChanged);
            // 
            // btnLimparTodos
            // 
            this.btnLimparTodos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnLimparTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimparTodos.FlatAppearance.BorderSize = 0;
            this.btnLimparTodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimparTodos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLimparTodos.ForeColor = System.Drawing.Color.White;
            this.btnLimparTodos.Location = new System.Drawing.Point(912, 136);
            this.btnLimparTodos.Name = "btnLimparTodos";
            this.btnLimparTodos.Size = new System.Drawing.Size(104, 25);
            this.btnLimparTodos.TabIndex = 6;
            this.btnLimparTodos.Text = "🗑️ Excluir Todos";
            this.btnLimparTodos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLimparTodos.UseVisualStyleBackColor = false;
            this.btnLimparTodos.Click += new System.EventHandler(this.btnLimparTodos_Click);
            // 
            // btnCarregar
            // 
            this.btnCarregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnCarregar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCarregar.FlatAppearance.BorderSize = 0;
            this.btnCarregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarregar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCarregar.ForeColor = System.Drawing.Color.Black;
            this.btnCarregar.Location = new System.Drawing.Point(790, 136);
            this.btnCarregar.Name = "btnCarregar";
            this.btnCarregar.Size = new System.Drawing.Size(116, 25);
            this.btnCarregar.TabIndex = 7;
            this.btnCarregar.Text = "📥 Carregar";
            this.btnCarregar.UseVisualStyleBackColor = false;
            this.btnCarregar.Click += new System.EventHandler(this.btnCarregar_Click);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1023, 575);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.dgvProdutos);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.chkSelecionarTodos);
            this.Controls.Add(this.btnLimparTodos);
            this.Controls.Add(this.btnCarregar);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SmartPrint v1.0 - Menu Principal";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.groupProduto.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSincronizar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnDesigner;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbBuscaReferencia;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBuscaNome;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBuscaCodigo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnAdicionar2;
        private System.Windows.Forms.PictureBox btnSincronizar2;
        private System.Windows.Forms.ComboBox cmbCor;
        private System.Windows.Forms.Label lblCor;
        private System.Windows.Forms.ComboBox cmbTamanho;
        private System.Windows.Forms.Label lblTamanho;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelecionar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPreco;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantidade;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCor;
        private System.Windows.Forms.DataGridViewButtonColumn colRemover;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnSincronizar;
        private System.Windows.Forms.Button btnCalibracao;
    }
}