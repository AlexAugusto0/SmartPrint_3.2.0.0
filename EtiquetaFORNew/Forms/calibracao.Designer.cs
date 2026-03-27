namespace EtiquetaFORNew.Forms
{
    partial class calibracao
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(calibracao));
            this.panelPrincipal = new System.Windows.Forms.Panel();
            this.panelConteudo = new System.Windows.Forms.Panel();
            this.groupBoxManual = new System.Windows.Forms.GroupBox();
            this.txtDescricao = new System.Windows.Forms.TextBox();
            this.Calibrar = new System.Windows.Forms.Button();
            this.lblSelecione = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnAssistirCalibracao = new System.Windows.Forms.Button();
            this.panelTitulo = new System.Windows.Forms.Panel();
            this.btndriver = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblPassoPasso = new System.Windows.Forms.Label();
            this.panelPrincipal.SuspendLayout();
            this.panelConteudo.SuspendLayout();
            this.groupBoxManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.panelPrincipal.Controls.Add(this.panelConteudo);
            this.panelPrincipal.Controls.Add(this.panelTitulo);
            this.panelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPrincipal.Location = new System.Drawing.Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Padding = new System.Windows.Forms.Padding(20);
            this.panelPrincipal.Size = new System.Drawing.Size(884, 561);
            this.panelPrincipal.TabIndex = 0;
            // 
            // panelConteudo
            // 
            this.panelConteudo.BackColor = System.Drawing.Color.White;
            this.panelConteudo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelConteudo.Controls.Add(this.groupBoxManual);
            this.panelConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConteudo.Location = new System.Drawing.Point(20, 83);
            this.panelConteudo.Name = "panelConteudo";
            this.panelConteudo.Padding = new System.Windows.Forms.Padding(20);
            this.panelConteudo.Size = new System.Drawing.Size(844, 458);
            this.panelConteudo.TabIndex = 2;
            // 
            // groupBoxManual
            // 
            this.groupBoxManual.Controls.Add(this.lblPassoPasso);
            this.groupBoxManual.Controls.Add(this.txtDescricao);
            this.groupBoxManual.Controls.Add(this.Calibrar);
            this.groupBoxManual.Controls.Add(this.lblSelecione);
            this.groupBoxManual.Controls.Add(this.comboBox1);
            this.groupBoxManual.Controls.Add(this.pictureBox1);
            this.groupBoxManual.Controls.Add(this.btnAssistirCalibracao);
            this.groupBoxManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxManual.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxManual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.groupBoxManual.Location = new System.Drawing.Point(20, 20);
            this.groupBoxManual.Name = "groupBoxManual";
            this.groupBoxManual.Padding = new System.Windows.Forms.Padding(15);
            this.groupBoxManual.Size = new System.Drawing.Size(802, 416);
            this.groupBoxManual.TabIndex = 1;
            this.groupBoxManual.TabStop = false;
            this.groupBoxManual.Text = "Selecione o Modelo da sua Etiquetadora:";
            this.groupBoxManual.Visible = false;
            // 
            // txtDescricao
            // 
            this.txtDescricao.BackColor = System.Drawing.Color.White;
            this.txtDescricao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescricao.Location = new System.Drawing.Point(463, 110);
            this.txtDescricao.Multiline = true;
            this.txtDescricao.Name = "txtDescricao";
            this.txtDescricao.ReadOnly = true;
            this.txtDescricao.Size = new System.Drawing.Size(321, 200);
            this.txtDescricao.TabIndex = 5;
            // 
            // Calibrar
            // 
            this.Calibrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.Calibrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Calibrar.FlatAppearance.BorderSize = 0;
            this.Calibrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Calibrar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Calibrar.ForeColor = System.Drawing.Color.Black;
            this.Calibrar.Location = new System.Drawing.Point(234, 325);
            this.Calibrar.Name = "Calibrar";
            this.Calibrar.Size = new System.Drawing.Size(181, 40);
            this.Calibrar.TabIndex = 4;
            this.Calibrar.Text = "Calibrar";
            this.Calibrar.UseVisualStyleBackColor = false;
            this.Calibrar.Click += new System.EventHandler(this.btnCalibrarAgora_Click);
            // 
            // lblSelecione
            // 
            this.lblSelecione.AutoSize = true;
            this.lblSelecione.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelecione.Location = new System.Drawing.Point(15, 40);
            this.lblSelecione.Name = "lblSelecione";
            this.lblSelecione.Size = new System.Drawing.Size(205, 17);
            this.lblSelecione.TabIndex = 0;
            this.lblSelecione.Text = "Escolha o modelo da impressora:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(15, 65);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(400, 25);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(15, 110);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnAssistirCalibracao
            // 
            this.btnAssistirCalibracao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnAssistirCalibracao.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAssistirCalibracao.FlatAppearance.BorderSize = 0;
            this.btnAssistirCalibracao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAssistirCalibracao.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAssistirCalibracao.ForeColor = System.Drawing.Color.Black;
            this.btnAssistirCalibracao.Location = new System.Drawing.Point(15, 325);
            this.btnAssistirCalibracao.Name = "btnAssistirCalibracao";
            this.btnAssistirCalibracao.Size = new System.Drawing.Size(181, 40);
            this.btnAssistirCalibracao.TabIndex = 3;
            this.btnAssistirCalibracao.Text = "Assistir";
            this.btnAssistirCalibracao.UseVisualStyleBackColor = false;
            this.btnAssistirCalibracao.Click += new System.EventHandler(this.btnAssistirCalibracao_Click);
            // 
            // panelTitulo
            // 
            this.panelTitulo.BackColor = System.Drawing.Color.White;
            this.panelTitulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTitulo.Controls.Add(this.btndriver);
            this.panelTitulo.Controls.Add(this.pictureBox2);
            this.panelTitulo.Controls.Add(this.lblTitulo);
            this.panelTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitulo.Location = new System.Drawing.Point(20, 20);
            this.panelTitulo.Name = "panelTitulo";
            this.panelTitulo.Size = new System.Drawing.Size(844, 63);
            this.panelTitulo.TabIndex = 0;
            // 
            // btndriver
            // 
            this.btndriver.BackColor = System.Drawing.Color.Transparent;
            this.btndriver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btndriver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btndriver.FlatAppearance.BorderSize = 0;
            this.btndriver.Image = global::EtiquetaFORNew.Properties.Resources.pngegg30x30;
            this.btndriver.Location = new System.Drawing.Point(777, 15);
            this.btndriver.Name = "btndriver";
            this.btndriver.Size = new System.Drawing.Size(45, 34);
            this.btndriver.TabIndex = 15;
            this.toolTip1.SetToolTip(this.btndriver, "Instalação de Drivers");
            this.btndriver.UseVisualStyleBackColor = false;
            this.btndriver.Click += new System.EventHandler(this.btndriver_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::EtiquetaFORNew.Properties.Resources.icone_novo_2025_PNG;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(67, 54);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblTitulo.Location = new System.Drawing.Point(63, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(334, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Calibração de Etiquetadoras";
            // 
            // lblPassoPasso
            // 
            this.lblPassoPasso.AutoSize = true;
            this.lblPassoPasso.Location = new System.Drawing.Point(459, 70);
            this.lblPassoPasso.Name = "lblPassoPasso";
            this.lblPassoPasso.Size = new System.Drawing.Size(109, 20);
            this.lblPassoPasso.TabIndex = 6;
            this.lblPassoPasso.Text = "Passo a Passo:";
            // 
            // calibracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.panelPrincipal);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "calibracao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calibração das etiquetadoras";
            this.Load += new System.EventHandler(this.calibracao_Load);
            this.panelPrincipal.ResumeLayout(false);
            this.panelConteudo.ResumeLayout(false);
            this.groupBoxManual.ResumeLayout(false);
            this.groupBoxManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelTitulo.ResumeLayout(false);
            this.panelTitulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPrincipal;
        private System.Windows.Forms.Panel panelTitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelConteudo;
        private System.Windows.Forms.GroupBox groupBoxManual;
        private System.Windows.Forms.Label lblSelecione;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnAssistirCalibracao;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button Calibrar;
        private System.Windows.Forms.Button btndriver;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtDescricao;
        private System.Windows.Forms.Label lblPassoPasso;
    }
}