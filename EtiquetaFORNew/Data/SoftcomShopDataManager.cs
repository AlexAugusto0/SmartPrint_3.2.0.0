using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Gerenciador de sincronização entre SoftcomShop API e SQLite local
    /// </summary>
    public class SoftcomShopDataManager
    {
        private readonly SoftcomShopService _service;
        private readonly string _connectionString;
        private readonly SoftcomShopService _softcomShopService;

        public SoftcomShopDataManager(SoftcomShopConfig config, string sqliteConnectionString)
        {
            _service = new SoftcomShopService(config);
            _connectionString = sqliteConnectionString;
            _softcomShopService = new SoftcomShopService(config);
        }

        #region Sincronização de Produtos

        /// <summary>
        /// Sincroniza todos os produtos do catálogo
        /// </summary>
        public async Task<SyncResult> SincronizarProdutosAsync(string versao = "v2", IProgress<string> progress = null)
        {
            var result = new SyncResult();
            int paginaAtual = 1;
            bool temMaisPaginas = true;

            try
            {
                progress?.Report("Iniciando sincronização de produtos...");

                // Limpar tabelas na primeira página
                if (paginaAtual == 1)
                {
                    LimparTabelasProdutos();
                }

                while (temMaisPaginas)
                {
                    progress?.Report($"Sincronizando página {paginaAtual}...");

                    string jsonResponse = await _service.GetProdutosAsync(paginaAtual, versao);
                    var response = JObject.Parse(jsonResponse);

                    // Verificar se há produtos
                    var produtos = response["data"] as JArray;
                    if (produtos == null || produtos.Count == 0)
                    {
                        temMaisPaginas = false;
                        continue;
                    }

                    // Processar produtos
                    result.ProdutosAdicionados += ProcessarProdutos(produtos, versao);

                    // Atualizar timestamp
                    if (response["date_sync"] != null)
                    {
                        AtualizarTimestamp(response["date_sync"].ToString());
                    }

                    // Verificar se tem mais páginas
                    if (versao == "v2")
                    {
                        int totalPaginas = response["meta"]["last_page"].ToObject<int>();
                        temMaisPaginas = paginaAtual < totalPaginas;
                    }
                    else
                    {
                        int totalPaginas = response["meta"]["page"]["count"].ToObject<int>();
                        temMaisPaginas = paginaAtual < totalPaginas;
                    }

                    paginaAtual++;
                }

                progress?.Report("Sincronização concluída!");
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.MensagemErro = ex.Message;
                progress?.Report($"Erro: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Processa e insere produtos no banco local
        /// </summary>
        private int ProcessarProdutos(JArray produtos, string versao)
        {
            int count = 0;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var produto in produtos)
                        {
                            InserirProduto(conn, produto, versao);
                            
                            // Processar tabela de preços (se houver)
                            if (produto["tabela_precos"] != null)
                            {
                                ProcessarTabelaPrecos(conn, produto);
                            }

                            // Processar atributos (TAM/COR) se versão v2
                            if (versao == "v2" && produto["sku_atributo"] != null)
                            {
                                ProcessarAtributos(conn, produto);
                            }

                            count++;
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Insere um produto no banco local
        /// </summary>
        // ================================================================================
        // CORREÇÃO - SoftcomShopDataManager.cs
        // SUBSTITUIR MÉTODO InserirProduto (LINHA ~148)
        // ================================================================================
        // PROBLEMA: Estava preenchendo "Referencia" mas ComboBox usa "CodFabricante"
        // SOLUÇÃO: Preencher AMBOS os campos
        // ================================================================================

        private void InserirProduto(SQLiteConnection conn, JToken produto, string versao)
        {
            var cmd = new SQLiteCommand(@"
        INSERT INTO Mercadorias (
            ID_SoftcomShop, CodigoMercadoria, CodFabricante, CodBarras, CodBarras_Grade, 
            Mercadoria, PrecoVenda, Fabricante, Grupo, 
            UltimaAtualizacao, Ativo, Tam, Cores, Origem,
            GerarEtiqueta, QuantidadeEtiqueta
        ) VALUES (
            @id, @codMerc, @codFabricante, @codBarras, @codBarrasGrade, 
            @mercadoria, @preco, @fabricante, @grupo,
            @dataAtualizacao, @ativo, @tam, @cor, 'SOFTCOMSHOP',
            0, 1
        )", conn);

            long produtoId = produto["produto_id"]?.ToObject<long>() ?? 0;

            cmd.Parameters.AddWithValue("@id", produtoId);
            cmd.Parameters.AddWithValue("@codMerc", produtoId);

            // Referência
            string referencia = produto["referencia"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(referencia))
            {
                referencia = produtoId.ToString();
            }
            cmd.Parameters.AddWithValue("@codFabricante", referencia);

            cmd.Parameters.AddWithValue("@codBarras", produto["codigo_barras"]?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@codBarrasGrade", produto["codigo_barras_grade"]?.ToString() ?? "");

            // ⭐ CORREÇÃO: Tentar múltiplos campos para o nome do produto
            string nomeProduto = null;

            // Tentar campo "produto_nome"
            if (!string.IsNullOrWhiteSpace(produto["produto_nome"]?.ToString()))
            {
                nomeProduto = produto["produto_nome"].ToString();
            }
            // Se vazio, tentar "descricao"
            else if (!string.IsNullOrWhiteSpace(produto["descricao"]?.ToString()))
            {
                nomeProduto = produto["descricao"].ToString();
            }
            // Se vazio, tentar "nome"
            else if (!string.IsNullOrWhiteSpace(produto["nome"]?.ToString()))
            {
                nomeProduto = produto["nome"].ToString();
            }
            // Se vazio, tentar "produto_descricao"
            else if (!string.IsNullOrWhiteSpace(produto["produto_descricao"]?.ToString()))
            {
                nomeProduto = produto["produto_descricao"].ToString();
            }
            // Fallback: usar referência ou ID
            else
            {
                nomeProduto = !string.IsNullOrWhiteSpace(referencia)
                    ? referencia
                    : $"Produto {produtoId}";
            }

            cmd.Parameters.AddWithValue("@mercadoria", nomeProduto);

            // Preço
            decimal preco = decimal.TryParse(
                produto["preco_venda"]?.ToString().Replace(".", ","),
                out decimal p) ? p : 0;
            cmd.Parameters.AddWithValue("@preco", preco);

            cmd.Parameters.AddWithValue("@fabricante", produto["marca_nome"]?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@grupo", produto["grupo_nome"]?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@dataAtualizacao", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@ativo", 1);

            cmd.Parameters.AddWithValue("@tam", "");
            cmd.Parameters.AddWithValue("@cor", "");

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Processa tabelas de preço A, B, C, D, E
        /// </summary>
        //private void ProcessarTabelaPrecos(SQLiteConnection conn, JToken produto)
        //{
        //    long produtoId = produto["produto_id"].ToObject<long>();
        //    string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";
        //    var tabelaPrecos = produto["tabela_precos"] as JArray;

        //    if (tabelaPrecos == null) return;

        //    foreach (var preco in tabelaPrecos)
        //    {
        //        string tipo = preco["descricao"]?.ToString() ?? "";
        //        decimal valor = decimal.TryParse(preco["preco"]?.ToString().Replace(".", ","), out decimal v) ? v : 0;

        //        string campo = null;
        //        switch (tipo)
        //        {
        //            case "A":
        //                campo = "VendaA";
        //                break;
        //            case "B":
        //                campo = "VendaB";
        //                break;
        //            case "C":
        //                campo = "VendaC";
        //                break;
        //            case "D":
        //                campo = "VendaD";
        //                break;
        //            case "E":
        //                campo = "VendaE";
        //                break;
        //        }

        //        if (campo != null)
        //        {
        //            var cmd = new SQLiteCommand($@"
        //                UPDATE Mercadorias 
        //                SET {campo} = @valor
        //                WHERE ID_SoftcomShop = @id 
        //                {(string.IsNullOrEmpty(codBarrasGrade) ? "" : "OR CodBarras_Grade = @codBarrasGrade")}
        //            ", conn);

        //            cmd.Parameters.AddWithValue("@valor", valor);
        //            cmd.Parameters.AddWithValue("@id", produtoId);
        //            if (!string.IsNullOrEmpty(codBarrasGrade))
        //                cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
        private void ProcessarTabelaPrecos(SQLiteConnection conn, JToken produto)
        {
            long produtoId = produto["produto_id"].ToObject<long>();
            // IMPORTANTE: Se o produto não tem grade, não devemos tentar filtrar por ela no OR
            string codBarrasGrade = produto["codigo_barras_grade"]?.ToString();
            var tabelaPrecos = produto["tabela_precos"] as JArray;

            if (tabelaPrecos == null) return;

            foreach (var preco in tabelaPrecos)
            {
                string tipo = preco["descricao"]?.ToString() ?? "";
                decimal valor = decimal.TryParse(preco["preco"]?.ToString().Replace(".", ","), out decimal v) ? v : 0;

                string campo = null;
                switch (tipo)
                {
                    case "A": campo = "VendaA"; break;
                    case "B": campo = "VendaB"; break;
                    case "C": campo = "VendaC"; break;
                    case "D": campo = "VendaD"; break;
                    case "E": campo = "VendaE"; break;
                }

                if (campo != null)
                {
                    // Melhorei a lógica do WHERE para ser mais restritiva
                    string sql = $@"UPDATE Mercadorias SET {campo} = @valor WHERE ID_SoftcomShop = @id";

                    // Só adiciona o filtro de grade se ele realmente existir e não for vazio
                    if (!string.IsNullOrWhiteSpace(codBarrasGrade))
                    {
                        sql += " AND CodBarras_Grade = @codBarrasGrade";
                    }

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@valor", valor);
                        cmd.Parameters.AddWithValue("@id", produtoId);

                        if (!string.IsNullOrWhiteSpace(codBarrasGrade))
                            cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Processa atributos de grade (TAM/COR)
        /// </summary>
        //private void ProcessarAtributos(SQLiteConnection conn, JToken produto)
        //{
        //    long produtoId = produto["produto_id"].ToObject<long>();
        //    string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";
        //    var atributos = produto["sku_atributo"] as JArray;

        //    if (atributos == null) return;

        //    string tam = "";
        //    string cor = "";

        //    foreach (var atributo in atributos)
        //    {
        //        string nome = atributo["nome"]?.ToString() ?? "";
        //        string itemNome = atributo["item_nome"]?.ToString() ?? "";

        //        if (nome.StartsWith("TAM"))
        //            tam = itemNome;
        //        else if (nome.StartsWith("COR"))
        //            cor = itemNome;
        //    }

        //    if (!string.IsNullOrEmpty(tam) || !string.IsNullOrEmpty(cor))
        //    {
        //        var cmd = new SQLiteCommand(@"
        //            UPDATE Mercadorias 
        //            SET Tam = @tam, Cores = @cor
        //            WHERE ID_SoftcomShop = @id 
        //            " + (string.IsNullOrEmpty(codBarrasGrade) ? "" : "OR CodBarras_Grade = @codBarrasGrade"), conn);

        //        cmd.Parameters.AddWithValue("@tam", tam);
        //        cmd.Parameters.AddWithValue("@cor", cor);
        //        cmd.Parameters.AddWithValue("@id", produtoId);
        //        if (!string.IsNullOrEmpty(codBarrasGrade))
        //            cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

        //        cmd.ExecuteNonQuery();
        //    }
        //}

        private void ProcessarAtributos(SQLiteConnection conn, JToken produto)
        {
            // IMPORTANTE: Para grade, o 'id' do JSON (produto_empresa_grade_id) 
            // ou o 'codigo_barras_grade' são as únicas chaves seguras.
            string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";
            var atributos = produto["sku_atributo"] as JArray;

            if (atributos == null || string.IsNullOrEmpty(codBarrasGrade)) return;

            string valorTam = "";
            string valorCor = "";

            foreach (var atributo in atributos)
            {
                string nomeAttr = atributo["nome"]?.ToString().ToUpper() ?? "";
                string itemNome = atributo["item_nome"]?.ToString() ?? "";

                // Mesma lógica de limpeza que funcionou anteriormente
                if (nomeAttr.StartsWith("TAM"))
                    valorTam = itemNome.Contains(":") ? itemNome.Split(':')[1].Trim() : itemNome;
                else if (nomeAttr.StartsWith("COR"))
                    valorCor = itemNome.Contains(":") ? itemNome.Split(':')[1].Trim() : itemNome;
            }

            if (!string.IsNullOrEmpty(valorTam) || !string.IsNullOrEmpty(valorCor))
            {
                // MUDANÇA CRUCIAL: O WHERE agora é estritamente pelo CodBarras_Grade.
                // Isso impede que o Tamanho 'P' sobrescreva o 'M' no banco local.
                var cmd = new SQLiteCommand(@"
            UPDATE Mercadorias 
            SET Tam = @tam, 
                Cores = @cor 
            WHERE CodBarras_Grade = @codBarrasGrade", conn);

                cmd.Parameters.AddWithValue("@tam", valorTam);
                cmd.Parameters.AddWithValue("@cor", valorCor);
                cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

                cmd.ExecuteNonQuery();
            }
        }


        #endregion

        #region Busca por Nota Fiscal

        /// <summary>
        /// Busca produtos por nota fiscal
        /// </summary>
        public async Task<SyncResult> BuscarPorNotaFiscalAsync(DateTime dataEntrada, int numeroNota = 0, string versao = "v2", IProgress<string> progress = null)
        {
            var result = new SyncResult();

            try
            {
                progress?.Report("Buscando nota fiscal...");

                string dataFormatada = dataEntrada.ToString("yyyy-MM-dd");
                string jsonResponse = await _service.GetNotaFiscalAsync(dataFormatada, numeroNota, 1, versao);
                
                var response = JObject.Parse(jsonResponse);
                var produtos = response["data"] as JArray;

                if (produtos == null || produtos.Count == 0)
                {
                    result.Sucesso = false;
                    result.MensagemErro = "Nenhum produto encontrado para esta nota fiscal.";
                    return result;
                }

                // Limpar etiquetas anteriores
                LimparEtiquetas();

                // Processar produtos marcando para impressão
                result.ProdutosAdicionados = ProcessarProdutosNotaFiscal(produtos, versao);

                progress?.Report($"{result.ProdutosAdicionados} produtos carregados!");
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.MensagemErro = ex.Message;
            }

            return result;
        }

        private int ProcessarProdutosNotaFiscal(JArray produtos, string versao)
        {
            int count = 0;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                foreach (var produto in produtos)
                {
                    // Verificar se produto já existe
                    long produtoId = produto["produto_id"].ToObject<long>();
                    string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";

                    if (ProdutoExiste(conn, produtoId, codBarrasGrade))
                    {
                        // Atualizar e marcar para impressão
                        AtualizarProdutoNF(conn, produto, versao);
                    }
                    else
                    {
                        // Inserir novo produto
                        InserirProduto(conn, produto, versao);
                        MarcarParaImpressao(conn, produtoId, codBarrasGrade, 
                            produto["compra_item_quantidade"]?.ToObject<int>() ?? 1);
                    }

                    count++;
                }
            }

            return count;
        }

        #endregion

        #region Busca por Venda

        /// <summary>
        /// Busca produtos por venda
        /// </summary>
        public async Task<SyncResult> BuscarPorVendaAsync(int numeroVenda, IProgress<string> progress = null)
        {
            var result = new SyncResult();

            try
            {
                progress?.Report($"Buscando venda {numeroVenda}...");

                string jsonResponse = await _service.GetVendaAsync(numeroVenda);
                var response = JObject.Parse(jsonResponse);

                if (!jsonResponse.Contains("sucesso"))
                {
                    result.Sucesso = false;
                    result.MensagemErro = "Venda não encontrada.";
                    return result;
                }

                var produtos = response["data"]["itens"] as JArray;

                if (produtos == null || produtos.Count == 0)
                {
                    result.Sucesso = false;
                    result.MensagemErro = "Nenhum produto encontrado nesta venda.";
                    return result;
                }

                // Limpar etiquetas anteriores
                LimparEtiquetas();

                // Processar produtos
                result.ProdutosAdicionados = ProcessarProdutosVenda(produtos);

                progress?.Report($"{result.ProdutosAdicionados} produtos carregados!");
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.MensagemErro = ex.Message;
            }

            return result;
        }

        private int ProcessarProdutosVenda(JArray produtos)
        {
            int count = 0;

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                foreach (var produto in produtos)
                {
                    long produtoId = produto["produto_id"].ToObject<long>();
                    string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";
                    int quantidade = produto["quantidade"]?.ToObject<int>() ?? 1;

                    if (ProdutoExiste(conn, produtoId, codBarrasGrade))
                    {
                        MarcarParaImpressao(conn, produtoId, codBarrasGrade, quantidade);
                    }

                    count++;
                }
            }

            return count;
        }

        #endregion

        #region Métodos Auxiliares

        private void LimparTabelasProdutos()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // ⭐ CORREÇÃO: Limpar TODOS os produtos
                    // Quando está em modo SoftcomShop, só deve ter produtos do SoftcomShop
                    // Não faz sentido misturar SQL Server + SoftcomShop
                    cmd.CommandText = "DELETE FROM Mercadorias";
                    cmd.ExecuteNonQuery();

                    // Também limpar ProdutosSelecionados para evitar referências quebradas
                    cmd.CommandText = "DELETE FROM ProdutosSelecionados";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LimparEtiquetas()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Mercadorias SET GerarEtiqueta = 0, QuantidadeEtiqueta = 1";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool ProdutoExiste(SQLiteConnection conn, long produtoId, string codBarrasGrade)
        {
            var cmd = new SQLiteCommand(@"
                SELECT COUNT(*) FROM Mercadorias 
                WHERE ID_SoftcomShop = @id 
                " + (string.IsNullOrEmpty(codBarrasGrade) ? "" : "OR CodBarras_Grade = @codBarrasGrade"), conn);

            cmd.Parameters.AddWithValue("@id", produtoId);
            if (!string.IsNullOrEmpty(codBarrasGrade))
                cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private void MarcarParaImpressao(SQLiteConnection conn, long produtoId, string codBarrasGrade, int quantidade)
        {
            var cmd = new SQLiteCommand(@"
                UPDATE Mercadorias 
                SET GerarEtiqueta = 1, QuantidadeEtiqueta = @qtd
                WHERE ID_SoftcomShop = @id 
                " + (string.IsNullOrEmpty(codBarrasGrade) ? "" : "OR CodBarras_Grade = @codBarrasGrade"), conn);

            cmd.Parameters.AddWithValue("@qtd", quantidade);
            cmd.Parameters.AddWithValue("@id", produtoId);
            if (!string.IsNullOrEmpty(codBarrasGrade))
                cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

            cmd.ExecuteNonQuery();
        }

        private void AtualizarProdutoNF(SQLiteConnection conn, JToken produto, string versao)
        {
            long produtoId = produto["produto_id"].ToObject<long>();
            string codBarrasGrade = produto["codigo_barras_grade"]?.ToString() ?? "";
            int quantidade = produto["compra_item_quantidade"]?.ToObject<int>() ?? 1;

            decimal preco = decimal.TryParse(produto["preco_venda"]?.ToString().Replace(".", ","), out decimal p) ? p : 0;

            var cmd = new SQLiteCommand(@"
                UPDATE Mercadorias 
                SET PrecoVenda = @preco, 
                    UltimaAtualizacao = @data,
                    GerarEtiqueta = 1,
                    QuantidadeEtiqueta = @qtd
                WHERE ID_SoftcomShop = @id 
                " + (string.IsNullOrEmpty(codBarrasGrade) ? "" : "OR CodBarras_Grade = @codBarrasGrade"), conn);

            cmd.Parameters.AddWithValue("@preco", preco);
            cmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@qtd", quantidade);
            cmd.Parameters.AddWithValue("@id", produtoId);
            if (!string.IsNullOrEmpty(codBarrasGrade))
                cmd.Parameters.AddWithValue("@codBarrasGrade", codBarrasGrade);

            cmd.ExecuteNonQuery();
        }

        private void AtualizarTimestamp(string timestamp)
        {
            // Salvar timestamp da última sincronização
            var config = ConfiguracaoSistema.Carregar();
            config.SoftcomShop.DataSync = timestamp;
            config.Salvar();
        }
        //public async Task SincronizarPromocoesAtivasAsync()
        //{
        //    try
        //    {
        //        // 1. Chama o método que você postou acima para obter o JSON               
        //        string jsonResponse = await _softcomShopService.GetPromocoesAsync();
        //        var response = JObject.Parse(jsonResponse);
        //        var promocoes = response["data"] as JArray;

        //        if (promocoes == null) return;

        //        using (var conn = new SQLiteConnection(_connectionString))
        //        {
        //            await conn.OpenAsync();

        //            using (var transaction = conn.BeginTransaction())
        //            {
        //                // 2. RESET: Antes de aplicar as novas, zeramos as promoções antigas no SQLite
        //                // Isso garante que se uma promoção acabou no Web, ela saia da etiqueta local.
        //                using (var cmdReset = new SQLiteCommand("UPDATE Mercadorias SET PrecoPromocional = 0, EmPromocao = 0", conn))
        //                {
        //                    await cmdReset.ExecuteNonQueryAsync();
        //                }

        //                // 3. PROCESSAMENTO: Percorre as promoções vindas do Web
        //                foreach (var promo in promocoes)
        //                {
        //                    var itens = promo["itens"] as JArray;
        //                    if (itens == null) continue;

        //                    foreach (var item in itens)
        //                    {
        //                        // No JSON do SoftcomShop, o vínculo costuma ser pelo CodigoMercadoria (SKU/EAN)
        //                        string sku = item["CodigoMercadoria"]?.ToString() ?? "";
        //                        decimal precoPromocional = item["Preco"]?.ToObject<decimal>() ?? 0;

        //                        if (string.IsNullOrEmpty(sku)) continue;

        //                        // 4. ATUALIZAÇÃO: Grava o preço de oferta na mercadoria correspondente
        //                        // Usamos o CodBarras ou CodBarras_Grade para garantir que atinja o item certo
        //                        string sqlUpdate = @"
        //                    UPDATE Mercadorias 
        //                    SET PrecoPromocional = @preco,
        //                        EmPromocao = 1
        //                    WHERE CodBarras = @sku OR CodBarras_Grade = @sku";

        //                        using (var cmdUpd = new SQLiteCommand(sqlUpdate, conn))
        //                        {
        //                            cmdUpd.Parameters.AddWithValue("@preco", precoPromocional);
        //                            cmdUpd.Parameters.AddWithValue("@sku", sku);
        //                            await cmdUpd.ExecuteNonQueryAsync();
        //                        }
        //                    }
        //                }
        //                transaction.Commit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao integrar promoções: {ex.Message}");
        //    }
        //}

        public async Task SincronizarPromocoesAtivasAsync()
        {
            try
            {
                // 1. Obtém os dados da API
                string jsonResponse = await _service.GetPromocoesAsync();
                if (string.IsNullOrEmpty(jsonResponse)) return;

                var response = JToken.Parse(jsonResponse);
                // Garante a leitura independente da versão da API (v1 ou v2)
                var listaPromocoes = response["data"] ?? response["produtos"] ?? response;

                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        // 2. Limpa estados promocionais anteriores (Garante integridade)
                        using (var cmdReset = new System.Data.SQLite.SQLiteCommand(
                            "UPDATE Mercadorias SET EmPromocao = 0, PrecoPromocional = 0, Origem = NULL", conn))
                        {
                            await cmdReset.ExecuteNonQueryAsync();
                        }

                        foreach (var item in listaPromocoes)
                        {
                            var subItens = item["itens"] as JArray ?? (item.HasValues ? new JArray(item) : null);
                            if (subItens == null) continue;

                            foreach (var promo in subItens)
                            {
                                // 3. Identificação do Produto (Vínculo com a Nuvem)
                                // 'produto_id' na API é o ID_SoftcomShop no seu banco
                                string idApi = (promo["produto_id"] ?? promo["id"])?.ToString()?.Trim() ?? "";

                                // 4. Tratamento do Preço
                                string precoBruto = (promo["preco_promocional"] ?? promo["valor"])?.ToString() ?? "0";
                                decimal preco = 0;
                                decimal.TryParse(precoBruto.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out preco);

                                if (string.IsNullOrEmpty(idApi) || preco <= 0) continue;

                                // 5. Datas de Vigência (Essencial para a consulta Promocoes_Ativas_filtroData)
                                string dIni = promo["data_inicio"]?.ToString() ?? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                                string dFim = promo["data_fim"]?.ToString() ?? DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

                                // 6. SQL CORRIGIDO (Equivalência ID_SoftcomShop)
                                // O segredo está em comparar o idApi com ID_SoftcomShop, não com CódigoMercadoria
                                string sql = @"UPDATE Mercadorias 
                                     SET EmPromocao = 1, 
                                         PrecoPromocional = @p,
                                         Origem = 'PROMOCAO',
                                         PromocaoDataInicio = @dIni,
                                         PromocaoDataFim = @dFim
                                     WHERE TRIM(CAST(ID_SoftcomShop AS TEXT)) = @id 
                                        OR TRIM(CAST(CodBarras AS TEXT)) = @id";

                                using (var cmd = new System.Data.SQLite.SQLiteCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@p", preco);
                                    cmd.Parameters.AddWithValue("@dIni", dIni);
                                    cmd.Parameters.AddWithValue("@dFim", dFim);
                                    cmd.Parameters.AddWithValue("@id", idApi);

                                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                                    // LOG DE DIAGNÓSTICO
                                    if (rowsAffected > 0)
                                        System.Diagnostics.Debug.WriteLine($"✅ ID {idApi} atualizado com sucesso.");
                                    else
                                        System.Diagnostics.Debug.WriteLine($"⚠️ ID {idApi} da API não encontrado no campo ID_SoftcomShop do SQLite.");
                                }
                            }
                        }
                        // 7. Confirmação final da gravação no arquivo físico
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha na sincronização de promoções: " + ex.Message);
            }
        }


        #endregion
    }

    /// <summary>
    /// Resultado de uma operação de sincronização
    /// </summary>
    public class SyncResult
    {
        public bool Sucesso { get; set; }
        public int ProdutosAdicionados { get; set; }
        public int ProdutosAtualizados { get; set; }
        public string MensagemErro { get; set; }
    }
}
