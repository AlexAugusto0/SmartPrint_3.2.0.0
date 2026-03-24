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
using System.Drawing.Printing;

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
        //private void btnCalibrarAgora_Click(object sender, EventArgs e)
        //{
        //    if (comboBox1.SelectedItem is CalibracaoInfo selecionado)
        //    {
        //        if (string.IsNullOrEmpty(selecionado.ComandoCalibracao))
        //        {
        //            MessageBox.Show("Comando de calibração não configurado para este modelo.");
        //            return;
        //        }

        //        // 1. Você precisa capturar qual impressora está instalada no Windows
        //        // Pode ser via PrintDialog ou uma configuração salva no seu sistema
        //        //string nomeImpressora = "Elgin L42";
        //        string nomeImpressora = new PrinterSettings().PrinterName;

        //        try
        //        {
        //            // 2. Envia o comando RAW (Bruto)
        //            // Se você não tiver a RawPrinterHelper, precisará adicioná-la ao projeto
        //            bool sucesso = RawPrinterHelper.SendStringToPrinter(nomeImpressora, selecionado.ComandoCalibracao);

        //            if (sucesso)
        //                MessageBox.Show($"Comando {selecionado.ComandoCalibracao} enviado para {selecionado.Nome}!");
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Erro ao enviar comando: " + ex.Message);
        //        }
        //    }
        //}
        private void btnCalibrarAgora_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is CalibracaoInfo selecionado)
            {
                if (string.IsNullOrEmpty(selecionado.ComandoCalibracao))
                {
                    MessageBox.Show("Comando de calibração não configurado para este modelo.");
                    return;
                }

                // Criamos o diálogo de impressão
                using (PrintDialog pd = new PrintDialog())
                {
                    // Opcional: Você pode configurar para abrir já com a impressora padrão selecionada
                    pd.AllowSelection = false;
                    pd.AllowSomePages = false;

                    // Abre a janela de seleção para o usuário
                    if (pd.ShowDialog() == DialogResult.OK)
                    {
                        // Pegamos o nome da impressora que o usuário clicou e deu "OK"
                        string nomeImpressora = pd.PrinterSettings.PrinterName;

                        try
                        {
                            // Envia o comando RAW para a impressora escolhida
                            bool sucesso = RawPrinterHelper.SendStringToPrinter(nomeImpressora, selecionado.ComandoCalibracao);

                            if (sucesso)
                            {
                                MessageBox.Show($"Comando enviado com sucesso para: {nomeImpressora}\n" +
                                                $"Modelo: {selecionado.Nome}", "Sucesso",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Não foi possível entregar o comando à fila de impressão.",
                                                "Erro de Comunicação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro técnico ao enviar comando: " + ex.Message, "Erro",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
