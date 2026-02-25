using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtiquetaFORNew.Forms
{
    public partial class calibracao : Form
    {
        public calibracao()
        {
            InitializeComponent();
            groupBoxManual.Visible = true;
            // Garante a conexão do evento se não estiver no Designer
            this.Load += calibracao_Load;
        }

        private void calibracao_Load(object sender, EventArgs e)
        {
            // Busca os dados do Manager (que usa CalibracaoInfo)
            var listaCalibracoes = CalibracaoManager.CarregarCalibracoes();
            //MessageBox.Show("Total encontrado: " + listaCalibracoes.Count);
            if (listaCalibracoes != null && listaCalibracoes.Count > 0)
            {
                this.comboBox1.DataSource = null;
                this.comboBox1.DataSource = listaCalibracoes;

                // IMPORTANTE: Deve bater com o nome na classe CalibracaoInfo
                this.comboBox1.DisplayMember = "Nome";
                this.comboBox1.ValueMember = "YoutubeUrl";

                this.comboBox1.SelectedIndex = -1;
            }
        }

        private void btnAssistir_Click(object sender, EventArgs e)
        {
            // Cast corrigido para CalibracaoInfo (a classe que o Manager usa)
            if (comboBox1.SelectedItem is CalibracaoInfo selecionado)
            {
                if (!string.IsNullOrEmpty(selecionado.YoutubeUrl))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selecionado.YoutubeUrl,
                        UseShellExecute = true
                    });
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica se o item selecionado é do tipo CalibracaoInfo
            if (comboBox1.SelectedItem is CalibracaoInfo selecionado)
            {
                // Carrega a imagem usando o método que criamos
                pictureBox1.Image = selecionado.ObterImagem();

                // Dica: Configure o SizeMode no Designer para Zoom para não distorcer
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnAssistirCalibracao_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is CalibracaoInfo selecionado)
            {
                if (!string.IsNullOrEmpty(selecionado.YoutubeUrl))
                {
                    try
                    {
                        // Abre o link do YouTube no navegador padrão
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = selecionado.YoutubeUrl,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao abrir o navegador: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Vídeo não disponível para este modelo.");
                }
            }
            else
            {
                MessageBox.Show("Selecione um modelo na lista primeiro.");
            }

        }
    }
}
