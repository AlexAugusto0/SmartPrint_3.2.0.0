using EtiquetaFORNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class FormConfiguracao : Form
    {
        private ConfiguracaoSistema _config;
        private SoftcomShopService _service;
        private ConfigForm _configFormSql;

        public FormConfiguracao()
        {
            InitializeComponent();
            CarregarConfiguracoes();

            txtUrlDispositivo.KeyDown += txtUrlDispositivo_KeyDown;

            this.FormClosing += FormConfiguracao_FormClosing;
        }

        //private void FormConfiguracao_Load(object sender, EventArgs e)
        //{
        //    cboTipoConexao.Items.Clear();
        //    cboTipoConexao.Items.Add("SQL Server");
        //    cboTipoConexao.Items.Add("SoftcomShop");

        //    if (_config.TipoConexaoAtiva == TipoConexao.SqlServer)
        //        cboTipoConexao.SelectedIndex = 0;
        //    else
        //        cboTipoConexao.SelectedIndex = 1;

        //    AtualizarPaineis();

        //}
        private void FormConfiguracao_Load(object sender, EventArgs e)
        {
            // 1. Configuração do ComboBox de Conexão (SQL vs SoftcomShop)
            cboTipoConexao.Items.Clear();
            cboTipoConexao.Items.Add("SQL Server");
            cboTipoConexao.Items.Add("SoftcomShop");

            if (_config.TipoConexaoAtiva == TipoConexao.SqlServer)
                cboTipoConexao.SelectedIndex = 0;
            else
                cboTipoConexao.SelectedIndex = 1;

            // 2. Configuração do ComboBox de Módulo (Novo código que você adicionou)
            if (comboModuloApp.Items.Count == 0)
            {
                comboModuloApp.Items.Add("Padrao");
                comboModuloApp.Items.Add("Confeccao");
            }

            // Carrega a configuração do DatabaseConfig para o módulo
            var configDb = DatabaseConfig.LoadConfiguration();
            if (!string.IsNullOrEmpty(configDb.ModuloAppWeb))
            {
                int index = comboModuloApp.FindStringExact(configDb.ModuloAppWeb);
                comboModuloApp.SelectedIndex = index >= 0 ? index : 0;
            }
            else
            {
                comboModuloApp.SelectedIndex = 0; // Default
            }

            // 3. Atualiza a visibilidade dos painéis baseada na conexão
            AtualizarPaineis();
        }

        #region Carregar/Salvar Configurações

        private void CarregarConfiguracoes()
        {
            try
            {
                _config = ConfiguracaoSistema.Carregar();

                if (_config.SoftcomShop != null)
                {
                    txtBaseURL.Text = _config.SoftcomShop.BaseURL;
                    txtClientId.Text = _config.SoftcomShop.ClientId;
                    txtClientSecret.Text = _config.SoftcomShop.ClientSecret;
                    txtEmpresaName.Text = _config.SoftcomShop.CompanyName;
                    txtEmpresaCNPJ.Text = _config.SoftcomShop.CompanyCNPJ;
                    txtDeviceName.Text = _config.SoftcomShop.DeviceName;
                    txtDeviceId.Text = _config.SoftcomShop.DeviceId;

                    // Atualizar contador de caracteres
                    AtualizarContadorCaracteres();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SalvarConfiguracoes()
        {
            try
            {
                _config.TipoConexaoAtiva = cboTipoConexao.SelectedIndex == 0
                    ? TipoConexao.SqlServer
                    : TipoConexao.SoftcomShop;

                if (cboTipoConexao.SelectedIndex == 1)
                {
                    if (_config.SoftcomShop == null)
                        _config.SoftcomShop = new SoftcomShopConfig();

                    _config.SoftcomShop.BaseURL = txtBaseURL.Text.Trim();
                    _config.SoftcomShop.ClientId = txtClientId.Text.Trim();
                    _config.SoftcomShop.ClientSecret = txtClientSecret.Text.Trim();
                    _config.SoftcomShop.CompanyName = txtEmpresaName.Text.Trim();
                    _config.SoftcomShop.CompanyCNPJ = txtEmpresaCNPJ.Text.Trim();
                    _config.SoftcomShop.DeviceName = txtDeviceName.Text.Trim();
                    _config.SoftcomShop.DeviceId = txtDeviceId.Text.Trim();
                }
                else
                {
                    var formProd = Application.OpenForms.OfType<ConfigForm>().FirstOrDefault();

                    if (formProd != null)
                    {
                        formProd.btnSalvar_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("O formulário de configuração não foi encontrado aberto.");
                    }
                }

                    _config.Salvar();

                var configDb = DatabaseConfig.LoadConfiguration();
                configDb.ModuloAppWeb = comboModuloApp.SelectedItem?.ToString() ?? "Padrao";
                DatabaseConfig.SaveConfiguration(configDb);

                this.Close();
                MessageBox.Show("Configurações salvas com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Parser de URL do Dispositivo

        /// <summary>
        /// Processa URL quando usuário sai do campo
        /// </summary>
        private void txtUrlDispositivo_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrlDispositivo.Text))
                return;

            try
            {
                ParsearUrlDispositivo(txtUrlDispositivo.Text);
                txtUrlDispositivo.Clear();

                MessageBox.Show("URL processada com sucesso!\n\nOs campos foram preenchidos automaticamente.",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar URL:\n\n{ex.Message}\n\nVerifique se a URL está correta.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ParsearUrlDispositivo(string urlCompleta)
        {
            if (string.IsNullOrWhiteSpace(urlCompleta))
                return;

            string[] partes = urlCompleta.Split('?');

            if (partes.Length < 2)
            {
                MessageBox.Show("URL inválida! Não contém parâmetros (?).",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string urlBase = partes[0];
            if (urlBase.Contains("com.br"))
            {
                int indiceComBr = urlBase.IndexOf("com.br") + 6;
                urlBase = urlBase.Substring(0, indiceComBr);
            }

            string parametros = partes[1];
            Dictionary<string, string> dadosUrl = ParsearParametrosUrl(parametros);
            PreencherCamposDeUrl(urlBase, dadosUrl);
        }

        private Dictionary<string, string> ParsearParametrosUrl(string parametros)
        {
            var resultado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] pares = parametros.Split('&');

            foreach (string par in pares)
            {
                if (string.IsNullOrWhiteSpace(par))
                    continue;

                string[] partesPar = par.Split('=');

                if (partesPar.Length == 2)
                {
                    string chave = partesPar[0].Trim();
                    string valor = Uri.UnescapeDataString(partesPar[1].Trim());
                    resultado[chave] = valor;
                }
            }

            return resultado;
        }

        private void PreencherCamposDeUrl(string urlBase, Dictionary<string, string> dados)
        {
            if (!string.IsNullOrEmpty(urlBase))
                txtBaseURL.Text = urlBase;

            PreencherCampoSeExistir(dados, "BaseURL", txtBaseURL);
            PreencherCampoSeExistir(dados, "Client_Id", txtClientId);
            PreencherCampoSeExistir(dados, "Empresa_Name", txtEmpresaName);
            PreencherCampoSeExistir(dados, "Empresa_CNPJ", txtEmpresaCNPJ);

            // ⭐ Device_Name = Nome do dispositivo no SoftcomShop (vem da URL)
            PreencherCampoSeExistir(dados, "Device_Name", txtDeviceName);

            // ⭐ Device_Id NÃO vem da URL - é preenchido pelo técnico manualmente
            // Não preencher automaticamente

            PreencherCampoSeExistir(dados, "ClientSecret", txtClientSecret);
        }

        private void PreencherCampoSeExistir(Dictionary<string, string> dados, string chave, TextBox campo)
        {
            if (dados.ContainsKey(chave) && !string.IsNullOrWhiteSpace(dados[chave]))
            {
                campo.Text = dados[chave];
            }
        }

        private void PreencherCampoSeExistir(Dictionary<string, string> dados, string chave, MaskedTextBox campo)
        {
            if (dados.ContainsKey(chave) && !string.IsNullOrWhiteSpace(dados[chave]))
            {
                campo.Text = dados[chave];
            }
        }

        #endregion

        #region Eventos dos Controles

        private void cboTipoConexao_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarPaineis();
        }

        private void AtualizarPaineis()
        {
            if (cboTipoConexao.SelectedIndex == 0)
            {
                panelSqlServer.Visible = true;
                panelSoftcomShop.Visible = false;
                CarregarConfigFormSql();
            }
            else
            {
                panelSqlServer.Visible = false;
                panelSoftcomShop.Visible = true;
                RemoverConfigFormSql();
            }
        }



        private void CarregarConfigFormSql()
        {
            if (_configFormSql != null)
                return;

            _configFormSql = new ConfigForm();
            _configFormSql.TopLevel = false;
            _configFormSql.FormBorderStyle = FormBorderStyle.None;
            _configFormSql.Dock = DockStyle.Fill;

            panelSqlServer.Controls.Clear();
            panelSqlServer.Controls.Add(_configFormSql);
            _configFormSql.Show();
        }

        private void RemoverConfigFormSql()
        {
            if (_configFormSql != null)
            {
                panelSqlServer.Controls.Remove(_configFormSql);
                _configFormSql.Dispose();
                _configFormSql = null;
            }
        }

        /// <summary>
        /// ⭐ NOVO: Contador de caracteres do Device_Id
        /// </summary>
        private void txtDeviceId_TextChanged(object sender, EventArgs e)
        {
            AtualizarContadorCaracteres();
        }

        private void AtualizarContadorCaracteres()
        {
            int count = txtDeviceId.Text.Length;
            lblCaracteres.Text = $"{count} caracteres";

            // Mudar cor baseado na quantidade
            if (count < 16)
                lblCaracteres.ForeColor = System.Drawing.Color.Red;
            else
                lblCaracteres.ForeColor = System.Drawing.Color.Green;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (cboTipoConexao.SelectedIndex == 1)
            {
                if (!ValidarCamposSoftcomShop())
                    return;
            }

            // Detectar se mudou o tipo de conexão
            var configAnterior = ConfiguracaoSistema.Carregar();
            bool mudouTipoConexao = configAnterior.TipoConexaoAtiva !=
                (cboTipoConexao.SelectedIndex == 0 ? TipoConexao.SqlServer : TipoConexao.SoftcomShop);

            SalvarConfiguracoes();

            // Se mudou tipo de conexão, perguntar se quer reiniciar
            if (mudouTipoConexao)
            {
                var result = MessageBox.Show(
                    "Tipo de conexão alterado!\n\n" +
                    "Para aplicar as mudanças, o sistema precisa ser reiniciado.\n\n" +
                    "Deseja reiniciar agora?",
                    "Reiniciar Sistema",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Reiniciar aplicação
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private async void btnTestarConexao_Click(object sender, EventArgs e)
        {
            // ⭐ Validar Device_Id antes de conectar
            if (!ValidarDeviceId())
                return;

            if (!ValidarCamposSoftcomShop())
                return;

            var result = MessageBox.Show(
                $"Deseja cadastrar este dispositivo no SoftcomShop?\n\n" +
                $"Dispositivo: {txtDeviceName.Text}\n" +
                $"Device ID: {txtDeviceId.Text}",
                "Confirmar Cadastro",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Salvar antes de cadastrar
            SalvarConfiguracoes();

            try
            {
                btnTestarConexao.Enabled = false;
                Cursor = Cursors.WaitCursor;

                // ⭐ IGUAL AO ACCESS: Botão "Conectar" cadastra o dispositivo
                _service = new SoftcomShopService(_config.SoftcomShop);
                string clientSecret = await _service.CadastrarDispositivoAsync();

                if (!string.IsNullOrEmpty(clientSecret))
                {
                    // Atualizar Client Secret
                    txtClientSecret.Text = clientSecret;
                    _config.SoftcomShop.ClientSecret = clientSecret;
                    _config.Salvar();

                    MessageBox.Show(
                        "Dispositivo cadastrado com sucesso!\n\n" +
                        "Client Secret foi gerado e salvo automaticamente.\n\n" +
                        "Você já pode sincronizar produtos.",
                        "Conexão Estabelecida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Falha ao conectar. Verifique as credenciais.\n\n" +
                        "Certifique-se de que:\n" +
                        "- A URL está correta\n" +
                        "- O Client ID está correto\n" +
                        "- O dispositivo não foi cadastrado anteriormente",
                        "Erro na Conexão",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao conectar:\n\n{ex.Message}\n\n" +
                    "Verifique sua conexão com a internet e as credenciais.",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnTestarConexao.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void txtUrlDispositivo_KeyDown(object sender, KeyEventArgs e)
        {
            // Verifica se a tecla pressionada foi o ENTER
            if (e.KeyCode == Keys.Enter)
            {
                // Impede o "BIP" do Windows ao apertar Enter em um campo de linha única
                e.SuppressKeyPress = true;

                if (!string.IsNullOrWhiteSpace(txtUrlDispositivo.Text))
                {
                    try
                    {
                        ParsearUrlDispositivo(txtUrlDispositivo.Text);
                        txtUrlDispositivo.Clear();

                        MessageBox.Show("URL processada com sucesso!\n\nOs campos foram preenchidos automaticamente.",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Opcional: Move o foco para o próximo campo após o processamento
                        this.SelectNextControl((Control)sender, true, true, true, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao processar URL:\n\n{ex.Message}",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }        

        #endregion

        #region Validações

        /// <summary>
        /// ⭐ NOVO: Valida Device_Id (mínimo 16 caracteres)
        /// </summary>
        private bool ValidarDeviceId()
        {
            if (string.IsNullOrWhiteSpace(txtDeviceId.Text))
            {
                MessageBox.Show("Informe o Device ID!",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDeviceId.Focus();
                return false;
            }

            if (txtDeviceId.Text.Trim().Length < 16)
            {
                MessageBox.Show("O Device ID deve ter no mínimo 16 caracteres!\n\n" +
                                $"Atual: {txtDeviceId.Text.Length} caracteres\n" +
                                "Necessário: 16 caracteres",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDeviceId.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarCamposSoftcomShop()
        {
            if (string.IsNullOrWhiteSpace(txtBaseURL.Text))
            {
                MessageBox.Show("Informe a URL Base da API!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBaseURL.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtClientId.Text))
            {
                MessageBox.Show("Informe o Client ID!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClientId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmpresaName.Text))
            {
                MessageBox.Show("Informe o Nome da Empresa!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpresaName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmpresaCNPJ.Text))
            {
                MessageBox.Show("Informe o CNPJ da Empresa!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpresaCNPJ.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDeviceName.Text))
            {
                MessageBox.Show("Informe o Nome do Dispositivo!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDeviceName.Focus();
                return false;
            }

            // ⭐ Validar Device_Id
            if (!ValidarDeviceId())
                return false;

            // ⭐ NÃO validar ClientSecret - ele é GERADO ao clicar em Conectar

            return true;
        }

        #endregion

        #region Cleanup

        private void FormConfiguracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoverConfigFormSql();
        }

        #endregion
    }
}