namespace EtiquetaFORNew
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtModuloApp = new System.Windows.Forms.TextBox();
            this.lblModuloApp = new System.Windows.Forms.Label();
            this.chkMostrarSenha = new System.Windows.Forms.CheckBox();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.txtPorta = new System.Windows.Forms.TextBox();
            this.lblPorta = new System.Windows.Forms.Label();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.lblSenha = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.btnListarBancos = new System.Windows.Forms.Button();
            this.cmbBancoDados = new System.Windows.Forms.ComboBox();
            this.lblBancoDados = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.lblServidor = new System.Windows.Forms.Label();
            this.btnTestar = new System.Windows.Forms.Button();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbLoja = new System.Windows.Forms.ComboBox();
            this.txtCaminhoFront = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelecionarFront = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(279, 20);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Configuração do Banco de Dados";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtModuloApp);
            this.groupBox1.Controls.Add(this.lblModuloApp);
            this.groupBox1.Controls.Add(this.chkMostrarSenha);
            this.groupBox1.Controls.Add(this.txtTimeout);
            this.groupBox1.Controls.Add(this.lblTimeout);
            this.groupBox1.Controls.Add(this.txtPorta);
            this.groupBox1.Controls.Add(this.lblPorta);
            this.groupBox1.Controls.Add(this.txtSenha);
            this.groupBox1.Controls.Add(this.lblSenha);
            this.groupBox1.Controls.Add(this.txtUsuario);
            this.groupBox1.Controls.Add(this.lblUsuario);
            this.groupBox1.Controls.Add(this.btnListarBancos);
            this.groupBox1.Controls.Add(this.cmbBancoDados);
            this.groupBox1.Controls.Add(this.lblBancoDados);
            this.groupBox1.Controls.Add(this.txtServidor);
            this.groupBox1.Controls.Add(this.lblServidor);
            this.groupBox1.Controls.Add(this.btnTestar);
            this.groupBox1.Location = new System.Drawing.Point(12, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(630, 246);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1 - Configurações de Conexão";
            // 
            // txtModuloApp
            // 
            this.txtModuloApp.Enabled = false;
            this.txtModuloApp.Location = new System.Drawing.Point(490, 212);
            this.txtModuloApp.Name = "txtModuloApp";
            this.txtModuloApp.ReadOnly = true;
            this.txtModuloApp.Size = new System.Drawing.Size(128, 20);
            this.txtModuloApp.TabIndex = 19;
            // 
            // lblModuloApp
            // 
            this.lblModuloApp.AutoSize = true;
            this.lblModuloApp.Location = new System.Drawing.Point(490, 196);
            this.lblModuloApp.Name = "lblModuloApp";
            this.lblModuloApp.Size = new System.Drawing.Size(67, 13);
            this.lblModuloApp.TabIndex = 20;
            this.lblModuloApp.Text = "Módulo App:";
            // 
            // chkMostrarSenha
            // 
            this.chkMostrarSenha.AutoSize = true;
            this.chkMostrarSenha.Location = new System.Drawing.Point(493, 166);
            this.chkMostrarSenha.Name = "chkMostrarSenha";
            this.chkMostrarSenha.Size = new System.Drawing.Size(95, 17);
            this.chkMostrarSenha.TabIndex = 13;
            this.chkMostrarSenha.Text = "Mostrar Senha";
            this.chkMostrarSenha.UseVisualStyleBackColor = true;
            this.chkMostrarSenha.CheckedChanged += new System.EventHandler(this.chkMostrarSenha_CheckedChanged);
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(490, 42);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(128, 20);
            this.txtTimeout.TabIndex = 12;
            this.txtTimeout.Text = "120";
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(487, 26);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(52, 13);
            this.lblTimeout.TabIndex = 11;
            this.lblTimeout.Text = "*Timeout:";
            // 
            // txtPorta
            // 
            this.txtPorta.Location = new System.Drawing.Point(268, 42);
            this.txtPorta.Name = "txtPorta";
            this.txtPorta.Size = new System.Drawing.Size(195, 20);
            this.txtPorta.TabIndex = 10;
            this.txtPorta.Text = "5433";
            // 
            // lblPorta
            // 
            this.lblPorta.AutoSize = true;
            this.lblPorta.Location = new System.Drawing.Point(265, 26);
            this.lblPorta.Name = "lblPorta";
            this.lblPorta.Size = new System.Drawing.Size(39, 13);
            this.lblPorta.TabIndex = 9;
            this.lblPorta.Text = "*Porta:";
            // 
            // txtSenha
            // 
            this.txtSenha.Location = new System.Drawing.Point(16, 123);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.Size = new System.Drawing.Size(447, 20);
            this.txtSenha.TabIndex = 8;
            this.txtSenha.UseSystemPasswordChar = true;
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.Location = new System.Drawing.Point(13, 107);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(45, 13);
            this.lblSenha.TabIndex = 7;
            this.lblSenha.Text = "*Senha:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(16, 84);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(447, 20);
            this.txtUsuario.TabIndex = 6;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(13, 68);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(50, 13);
            this.lblUsuario.TabIndex = 5;
            this.lblUsuario.Text = "*Usuário:";
            // 
            // btnListarBancos
            // 
            this.btnListarBancos.Location = new System.Drawing.Point(16, 189);
            this.btnListarBancos.Name = "btnListarBancos";
            this.btnListarBancos.Size = new System.Drawing.Size(98, 21);
            this.btnListarBancos.TabIndex = 14;
            this.btnListarBancos.Text = "Listar Bancos";
            this.btnListarBancos.UseVisualStyleBackColor = true;
            this.btnListarBancos.Click += new System.EventHandler(this.btnListarBancos_Click);
            // 
            // cmbBancoDados
            // 
            this.cmbBancoDados.FormattingEnabled = true;
            this.cmbBancoDados.Location = new System.Drawing.Point(16, 162);
            this.cmbBancoDados.Name = "cmbBancoDados";
            this.cmbBancoDados.Size = new System.Drawing.Size(447, 21);
            this.cmbBancoDados.TabIndex = 4;
            // 
            // lblBancoDados
            // 
            this.lblBancoDados.AutoSize = true;
            this.lblBancoDados.Location = new System.Drawing.Point(13, 146);
            this.lblBancoDados.Name = "lblBancoDados";
            this.lblBancoDados.Size = new System.Drawing.Size(94, 13);
            this.lblBancoDados.TabIndex = 3;
            this.lblBancoDados.Text = "*Banco de Dados:";
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(16, 42);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(246, 20);
            this.txtServidor.TabIndex = 2;
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(13, 26);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(101, 13);
            this.lblServidor.TabIndex = 1;
            this.lblServidor.Text = "*Servidor\\Instância:";
            // 
            // btnTestar
            // 
            this.btnTestar.Location = new System.Drawing.Point(291, 202);
            this.btnTestar.Name = "btnTestar";
            this.btnTestar.Size = new System.Drawing.Size(172, 30);
            this.btnTestar.TabIndex = 3;
            this.btnTestar.Text = "Testar Conexão";
            this.btnTestar.UseVisualStyleBackColor = true;
            this.btnTestar.Click += new System.EventHandler(this.btnTestar_Click);
            // 
            // btnSalvar
            // 
            this.btnSalvar.Location = new System.Drawing.Point(375, 405);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(100, 30);
            this.btnSalvar.TabIndex = 4;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Visible = false;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(482, 405);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Visible = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EtiquetaFORNew.Properties.Resources.Banco_de_dados;
            this.pictureBox1.Location = new System.Drawing.Point(297, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 28);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbLoja);
            this.groupBox2.Location = new System.Drawing.Point(12, 357);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(630, 42);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2 -Seleciona a Empresa.";
            // 
            // cmbLoja
            // 
            this.cmbLoja.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLoja.Enabled = false;
            this.cmbLoja.FormattingEnabled = true;
            this.cmbLoja.Location = new System.Drawing.Point(6, 19);
            this.cmbLoja.Name = "cmbLoja";
            this.cmbLoja.Size = new System.Drawing.Size(246, 21);
            this.cmbLoja.TabIndex = 19;
            this.cmbLoja.SelectedIndexChanged += new System.EventHandler(this.cmbLoja_SelectedIndexChanged);
            // 
            // txtCaminhoFront
            // 
            this.txtCaminhoFront.BackColor = System.Drawing.Color.White;
            this.txtCaminhoFront.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCaminhoFront.Location = new System.Drawing.Point(150, 58);
            this.txtCaminhoFront.Name = "txtCaminhoFront";
            this.txtCaminhoFront.Size = new System.Drawing.Size(416, 20);
            this.txtCaminhoFront.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(20, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 21;
            this.label1.Text = "Selecione o Front:";
            // 
            // btnSelecionarFront
            // 
            this.btnSelecionarFront.Location = new System.Drawing.Point(575, 58);
            this.btnSelecionarFront.Name = "btnSelecionarFront";
            this.btnSelecionarFront.Size = new System.Drawing.Size(25, 20);
            this.btnSelecionarFront.TabIndex = 23;
            this.btnSelecionarFront.Text = "...";
            this.btnSelecionarFront.UseVisualStyleBackColor = true;
            this.btnSelecionarFront.Click += new System.EventHandler(this.btnSelecionarFront_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 447);
            this.Controls.Add(this.btnSelecionarFront);
            this.Controls.Add(this.txtCaminhoFront);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuração do Banco de Dados";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.Button btnTestar;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ComboBox cmbBancoDados;
        private System.Windows.Forms.Label lblBancoDados;
        private System.Windows.Forms.Button btnListarBancos;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.TextBox txtPorta;
        private System.Windows.Forms.Label lblPorta;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.CheckBox chkMostrarSenha;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtModuloApp;
        private System.Windows.Forms.Label lblModuloApp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbLoja;
        private System.Windows.Forms.TextBox txtCaminhoFront;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelecionarFront;
    }
}