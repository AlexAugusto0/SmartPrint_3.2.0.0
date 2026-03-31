using EtiquetaFORNew.Data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class ConfigForm : Form
    {
        //private NFCeRepository _repository;
        public ConfigForm()
        {
            InitializeComponent();
            VersaoHelper.DefinirTituloComVersao(this, "Configurações");
            CarregarConfiguracao();
        }

        private void CarregarConfiguracao()
        {
            DatabaseConfig.ConfigData config = DatabaseConfig.LoadConfiguration();

            if (!string.IsNullOrEmpty(config.Servidor))
            {
                txtServidor.Text = config.Servidor;
                txtPorta.Text = config.Porta ?? "5433";
                cmbBancoDados.Text = config.Banco;
                txtUsuario.Text = config.Usuario ?? "";
                txtSenha.Text = config.Senha ?? "";
                txtTimeout.Text = config.Timeout ?? "120";

                // Carregar loja se houver configuração salva
                if (!string.IsNullOrEmpty(config.Loja))
                {
                    // Tentar carregar as lojas para popular a combo
                    try
                    {
                        string connStr = ConstruirConnectionString();
                        CarregarLojas(connStr);

                        // Selecionar a loja salva
                        int index = cmbLoja.Items.IndexOf(config.Loja);
                        if (index >= 0)
                        {
                            cmbLoja.SelectedIndex = index;
                        }

                        // Carregar ModuloApp se houver
                        if (!string.IsNullOrEmpty(config.ModuloApp))
                        {
                            txtModuloApp.Text = config.ModuloApp;
                            txtModuloApp.Enabled = true;
                        }
                    }
                    catch
                    {
                        // Se nÃ£o conseguir carregar, apenas ignora
                        // A loja serÃ¡ carregada apÃ³s o teste de conexÃ£o
                    }
                }
            }
            else
            {
                // Valores padrÃ£o
                txtServidor.Text = "localhost\\SQLEXPRESS";
                txtPorta.Text = "5433";
                txtTimeout.Text = "120";
            }

            chkMostrarSenha.Checked = false;
            txtSenha.UseSystemPasswordChar = true;
        }

        private void ParseConnectionString(string connStr)
        {
            try
            {
                string[] parts = connStr.Split(';');
                foreach (string part in parts)
                {
                    string[] keyValue = part.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim().ToLower();
                        string value = keyValue[1].Trim();

                        switch (key)
                        {
                            case "server":
                            case "data source":
                                // Separar servidor e porta se houver vÃƒÂ­rgula
                                if (value.Contains(","))
                                {
                                    string[] serverPort = value.Split(',');
                                    txtServidor.Text = serverPort[0];
                                    txtPorta.Text = serverPort[1];
                                }
                                else
                                {
                                    txtServidor.Text = value;
                                }
                                break;
                            case "database":
                            case "initial catalog":
                                cmbBancoDados.Text = value;
                                break;
                            case "user id":
                            case "uid":
                                txtUsuario.Text = value;
                                break;
                            case "password":
                            case "pwd":
                                txtSenha.Text = value;
                                break;
                            case "connection timeout":
                                txtTimeout.Text = value;
                                break;
                            case "integrated security":
                                // Se usar autenticação Windows, limpar usuÃ¡rio e senha
                                if (value.ToLower() == "true" || value.ToLower() == "sspi")
                                {
                                    txtUsuario.Text = "";
                                    txtSenha.Text = "";
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                // Se não conseguir parsear, deixa os campos vazios
            }
        }

        private void btnListarBancos_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServidor.Text))
            {
                MessageBox.Show("Informe o servidor/instância primeiro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServidor.Focus();
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                btnListarBancos.Enabled = false;
                btnListarBancos.Text = "Carregando...";

                // Construir connection string temporÃ¡ria para listar bancos
                string servidor = txtServidor.Text.Trim();
                string porta = txtPorta.Text.Trim();
                string usuario = txtUsuario.Text.Trim();
                string senha = txtSenha.Text;

                // Adicionar porta ao servidor se informada
                if (!string.IsNullOrEmpty(porta) && porta != "1433")
                {
                    servidor = $"{servidor},{porta}";
                }

                string connStrMaster = $"Server={servidor};Database=master;";

                // Autenticação
                if (!string.IsNullOrEmpty(usuario))
                {
                    connStrMaster += $"User Id={usuario};Password={senha};";
                }
                else
                {
                    connStrMaster += "Integrated Security=true;";
                }

                connStrMaster += "Connection Timeout=10;";

                // Listar bancos de dados
                List<string> bancos = ListarBancosDados(connStrMaster);

                if (bancos.Count > 0)
                {
                    cmbBancoDados.Items.Clear();
                    foreach (string banco in bancos)
                    {
                        cmbBancoDados.Items.Add(banco);
                    }

                    MessageBox.Show($"{bancos.Count} banco(s) de dados encontrado(s)!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (cmbBancoDados.Items.Count > 0 && string.IsNullOrEmpty(cmbBancoDados.Text))
                    {
                        cmbBancoDados.DroppedDown = true;
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum banco de dados encontrado!", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar bancos de dados:\n\n" + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnListarBancos.Enabled = true;
                btnListarBancos.Text = "Listar Bancos";
            }
        }

        private List<string> ListarBancosDados(string connectionString)
        {
            List<string> bancos = new List<string>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT name 
                    FROM sys.databases 
                    WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')
                    AND state_desc = 'ONLINE'
                    ORDER BY name";

                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bancos.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return bancos;
        }

        private string ConstruirConnectionString()
        {
            string servidor = txtServidor.Text.Trim();
            string porta = txtPorta.Text.Trim();
            string banco = cmbBancoDados.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Text;
            string timeout = txtTimeout.Text.Trim();

            // Adicionar porta ao servidor se informada e diferente da padrÃ£o
            if (!string.IsNullOrEmpty(porta) && porta != "1433")
            {
                servidor = $"{servidor},{porta}";
            }

            string connStr = $"Server={servidor};Database={banco};";

            // Se usuÃ¡rio foi informado, usar autenticação SQL
            if (!string.IsNullOrEmpty(usuario))
            {
                connStr += $"User Id={usuario};Password={senha};";
            }
            else
            {
                // Caso contrÃ¡rio, usar autenticaÃ§o Windows
                connStr += "Integrated Security=true;";
            }

            // Adicionar timeout se diferente do padrÃ£o
            if (!string.IsNullOrEmpty(timeout) && timeout != "15")
            {
                connStr += $"Connection Timeout={timeout};";
            }

            return connStr;
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                string connStr = ConstruirConnectionString();
                SmartPrintRepository repo = new SmartPrintRepository(connStr);

                if (repo.TestConnection())
                {
                    MessageBox.Show("Conexão realizada com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //repo.VerificarECriarEstrutura();

                    // Carregar lojas apÃ³s conexÃ£o bem-sucedida
                    CarregarLojas(connStr);
                }
                else
                {
                    MessageBox.Show("Não foi possí­vel conectar ao banco de dados!", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao testar conexão: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtServidor.Text))
            {
                MessageBox.Show("Informe o servidor/instância!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServidor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbBancoDados.Text))
            {
                MessageBox.Show("Informe o banco de dados!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbBancoDados.Focus();
                return false;
            }

            return true;
        }

        public void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                DatabaseConfig.SaveConfiguration(
                    txtServidor.Text.Trim(),
                    txtPorta.Text.Trim(),
                    cmbBancoDados.Text.Trim(),
                    txtUsuario.Text.Trim(),
                    txtSenha.Text,
                    txtTimeout.Text.Trim(),
                    cmbLoja.SelectedItem?.ToString() ?? "",
                    txtModuloApp.Text.Trim()
                );

                MessageBox.Show("Configuração salva com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Atualiza o MainForm sem reiniciar o app
                var mainForm = Application.OpenForms["Main"] as Main;
                if (mainForm != null)
                {
                    mainForm.RecarregarConfiguracao();
                }

                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configuração: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        private void CarregarLojas(string connectionString)
        {
            try
            {
                cmbLoja.Items.Clear();
                cmbLoja.Enabled = false;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Loja
                        FROM Integrar_Lojas 
                        WHERE Desativado = 0
                        ORDER BY Loja";

                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbLoja.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                if (cmbLoja.Items.Count > 0)
                {
                    cmbLoja.Enabled = true;
                    if (cmbLoja.Items.Count == 1)
                    {
                        cmbLoja.SelectedIndex = 0;
                    }
                }

                // Carregar ModuloApp apÃ³s carregar as lojas
                CarregarModuloApp(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar lojas: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarModuloApp(string connectionString)
        {
            try
            {
                txtModuloApp.Clear();
                txtModuloApp.Enabled = false;

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT TOP 1 moduloapp 
                        FROM Empresa";

                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            txtModuloApp.Text = result.ToString();
                            txtModuloApp.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar módulo app: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void cmbLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoja.SelectedItem == null)
                return;

            try
            {
                string lojaSelecionada = cmbLoja.SelectedItem.ToString();
                string connectionString = ConstruirConnectionString();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT Fantasia, CGC, CodigoSuporte
                        FROM Integrar_Lojas 
                        WHERE Loja = @Loja";

                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Loja", lojaSelecionada);

                        using (System.Data.SqlClient.SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string fantasia = reader.IsDBNull(0) ? "" : reader.GetString(0);
                                string cgc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                string codigoSuporte = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                // Chamar a função GetSetRegistroJsonAsync
                                if (!string.IsNullOrEmpty(codigoSuporte) && !string.IsNullOrEmpty(cgc) && !string.IsNullOrEmpty(fantasia))
                                {
                                    string resultado = await DatabaseConfig.GetSetRegistroJsonAsync(codigoSuporte, cgc, fantasia);

                                    // Opcional: vocÃª pode fazer algo com o resultado aqui
                                    // Por exemplo, mostrar uma mensagem ou salvar em algum lugar
                                    System.Diagnostics.Debug.WriteLine($"Resultado do registro: {resultado}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar seleção da loja: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelecionarFront_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Access Database (*.mdb;*.accdb)|*.mdb;*.accdb";
                ofd.Title = "Selecione o Front-end do Sistema";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtCaminhoFront.Text = ofd.FileName; // Um TextBox para mostrar o caminho
                    ImportarConfiguracaoAccess(ofd.FileName);
                }
            }
        }
        private void ImportarConfiguracaoAccess(string caminhoArquivo)
        {
            // String de conexão para arquivos .mdb ou .accdb
            // Se o seu sistema for 32 bits use Microsoft.Jet.OLEDB.4.0; 
            // Para 64 bits ou .accdb use Microsoft.ACE.OLEDB.12.0;
            // Adicionado "Mode=Share Deny None" para evitar conflitos de bloqueio
            string connStringAccess = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={caminhoArquivo};Mode=Share Deny None;Persist Security Info=False;";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connStringAccess))
                {
                    conn.Open();
                    // Pegamos o registro onde BancoSQL está marcado (conforme sua imagem)
                    string query = "SELECT TOP 1 SqlServidor, SqlPorta, SqlBase, SqlLogin, SqlSenha FROM seguranca WHERE BancoSQL = True";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Preenche os campos do seu formulário C#
                                txtServidor.Text = reader["SqlServidor"].ToString();
                                txtPorta.Text = reader["SqlPorta"].ToString();
                                cmbBancoDados.Text = reader["SqlBase"].ToString();
                                txtUsuario.Text = reader["SqlLogin"].ToString();

                                // A senha no Access costuma estar criptografada ou mascarada. 
                                // Se estiver em texto puro, o código abaixo funciona:
                                txtSenha.Text = reader["SqlSenha"].ToString();

                                MessageBox.Show("Configurações importadas do Access com sucesso!", "Importação", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Opcional: Acionar o botão de listar bancos ou testar conexão automaticamente
                                // btnListarBancos.PerformClick();
                            }
                            else
                            {
                                MessageBox.Show("Nenhuma configuração ativa encontrada na tabela 'seguranca'.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ler o arquivo Access: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}