using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace EtiquetaFORNew.Data
{
    /// <summary>
    /// Gerencia o banco de dados SQLite local para cache de mercadorias
    /// ATUALIZADO: Suporta SQL Server E SoftcomShop
    /// </summary>
    public class LocalDatabaseManager
    {
        public static bool isConfeccao = false;
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");

        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        /// <summary>
        /// Obtém a connection string (para uso externo)
        /// </summary>
        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        /// <summary>
        /// ⭐ ATUALIZADO: Inicializa o banco local com campos adicionais para SoftcomShop
        /// </summary>
        //public static void InicializarBanco()
        //{
        //    try
        //    {
        //        // Criar arquivo do banco se não existir
        //        if (!File.Exists(DbPath))
        //        {
        //            SQLiteConnection.CreateFile(DbPath);
        //        }

        //        using (var conn = new SQLiteConnection(ConnectionString))
        //        {
        //            conn.Open();

        //            // ⭐ TABELA PRINCIPAL: Mercadorias
        //            // Campos originais + campos do SoftcomShop
        //            string createTable = @"
        //                CREATE TABLE IF NOT EXISTS Mercadorias (
        //                    -- Campos originais
        //                    CodigoMercadoria INTEGER,
        //                    CodFabricante TEXT,
        //                    CodBarras TEXT,
        //                    Mercadoria TEXT NOT NULL,
        //                    PrecoVenda REAL,
        //                    VendaA REAL,
        //                    VendaB REAL,
        //                    VendaC REAL,
        //                    VendaD REAL,
        //                    VendaE REAL,
        //                    Fornecedor TEXT,
        //                    Fabricante TEXT,
        //                    Grupo TEXT,
        //                    Prateleira TEXT,
        //                    Garantia TEXT,
        //                    Tam TEXT,
        //                    Cores TEXT,
        //                    CodBarras_Grade TEXT,
        //                    Registro INTEGER,
        //                    UltimaAtualizacao DATETIME DEFAULT CURRENT_TIMESTAMP,

        //                    -- ⭐ NOVOS: Campos específicos do SoftcomShop
        //                    ID_SoftcomShop INTEGER DEFAULT 0,
        //                    Origem TEXT DEFAULT 'SQL',
        //                    Referencia TEXT,
        //                    Marca TEXT,
        //                    Ativo INTEGER DEFAULT 1,
        //                    GerarEtiqueta INTEGER DEFAULT 0,
        //                    QuantidadeEtiqueta INTEGER DEFAULT 1
        //                );

        //                CREATE INDEX IF NOT EXISTS idx_codfabricante ON Mercadorias(CodFabricante);
        //                CREATE INDEX IF NOT EXISTS idx_mercadoria ON Mercadorias(Mercadoria);
        //                CREATE INDEX IF NOT EXISTS idx_codbarras ON Mercadorias(CodBarras);
        //                CREATE INDEX IF NOT EXISTS idx_fornecedor ON Mercadorias(Fornecedor);
        //                CREATE INDEX IF NOT EXISTS idx_fabricante ON Mercadorias(Fabricante);
        //                CREATE INDEX IF NOT EXISTS idx_grupo ON Mercadorias(Grupo);
        //                CREATE INDEX IF NOT EXISTS idx_id_softcomshop ON Mercadorias(ID_SoftcomShop);
        //                CREATE INDEX IF NOT EXISTS idx_origem ON Mercadorias(Origem);

        //                CREATE TABLE IF NOT EXISTS ProdutosSelecionados (
        //                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
        //                    CodigoMercadoria INTEGER,
        //                    Nome TEXT NOT NULL,
        //                    Codigo TEXT NOT NULL,
        //                    Preco REAL NOT NULL,
        //                    Quantidade INTEGER NOT NULL,
        //                    DataSelecao DATETIME DEFAULT CURRENT_TIMESTAMP,
        //                    FOREIGN KEY (CodigoMercadoria) REFERENCES Mercadorias(CodigoMercadoria)
        //                );

        //                CREATE TABLE IF NOT EXISTS ConfiguracaoSync (
        //                    Id INTEGER PRIMARY KEY,
        //                    UltimaSincronizacao DATETIME,
        //                    TotalRegistros INTEGER,
        //                    TipoOrigem TEXT DEFAULT 'SQL'
        //                );
        //            ";

        //            using (var cmd = new SQLiteCommand(createTable, conn))
        //            {
        //                cmd.ExecuteNonQuery();
        //            }

        //            // ⭐ VERIFICAR E ADICIONAR CAMPOS NOVOS SE NÃO EXISTIREM
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "ID_SoftcomShop", "INTEGER DEFAULT 0");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Origem", "TEXT DEFAULT 'SQL'");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Referencia", "TEXT");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Marca", "TEXT");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Ativo", "INTEGER DEFAULT 1");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "GerarEtiqueta", "INTEGER DEFAULT 0");
        //            AdicionarCampoSeNaoExistir(conn, "Mercadorias", "QuantidadeEtiqueta", "INTEGER DEFAULT 1");
        //            AdicionarCampoSeNaoExistir(conn, "ConfiguracaoSync", "TipoOrigem", "TEXT DEFAULT 'SQL'");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Erro ao inicializar banco local: {ex.Message}", ex);
        //    }
        //}

        public static void InicializarBanco()
        {
            try
            {
                // Criar arquivo do banco se não existir
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // 1. Criação das Tabelas Base
                    string createTable = @"
                CREATE TABLE IF NOT EXISTS Mercadorias (
                    CodigoMercadoria INTEGER,
                    CodFabricante TEXT,
                    CodBarras TEXT,
                    Mercadoria TEXT NOT NULL,
                    PrecoVenda REAL,
                    VendaA REAL,
                    VendaB REAL,
                    VendaC REAL,
                    VendaD REAL,
                    VendaE REAL,
                    Fornecedor TEXT,
                    Fabricante TEXT,
                    Grupo TEXT,
                    Prateleira TEXT,
                    Garantia TEXT,
                    Tam TEXT,
                    Cores TEXT,
                    CodBarras_Grade TEXT,
                    Registro INTEGER,
                    UltimaAtualizacao DATETIME DEFAULT CURRENT_TIMESTAMP,
                    
                    -- Campos SoftcomShop
                    ID_SoftcomShop INTEGER DEFAULT 0,
                    Origem TEXT DEFAULT 'SQL',
                    Referencia TEXT,
                    Marca TEXT,
                    Ativo INTEGER DEFAULT 1,
                    GerarEtiqueta INTEGER DEFAULT 0,
                    QuantidadeEtiqueta INTEGER DEFAULT 1,
                    
                    -- CAMPOS DE PROMOÇÃO (Garantia de criação na estrutura inicial)
                    EmPromocao INTEGER DEFAULT 0,
                    PrecoPromocional REAL DEFAULT 0
                );

                CREATE INDEX IF NOT EXISTS idx_codbarras ON Mercadorias(CodBarras);
                CREATE INDEX IF NOT EXISTS idx_mercadoria ON Mercadorias(Mercadoria);

                CREATE TABLE IF NOT EXISTS ProdutosSelecionados (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CodigoMercadoria INTEGER,
                    Nome TEXT NOT NULL,
                    Codigo TEXT NOT NULL,
                    Preco REAL NOT NULL,
                    Quantidade INTEGER NOT NULL,
                    DataSelecao DATETIME DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS ConfiguracaoSync (
                    Id INTEGER PRIMARY KEY,
                    UltimaSincronizacao DATETIME,
                    TotalRegistros INTEGER,
                    TipoOrigem TEXT DEFAULT 'SQL'
                );
            ";

                    using (var cmd = new SQLiteCommand(createTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 2. ⭐ GARANTIA DE CAMPOS (Para bancos já existentes)
                    // Se o usuário já tem o LocalData.db, o 'CREATE TABLE IF NOT EXISTS' não adiciona colunas novas.
                    // Por isso, forçamos a verificação individual de cada campo essencial para o Web.

                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "ID_SoftcomShop", "INTEGER DEFAULT 0");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Origem", "TEXT DEFAULT 'SQL'");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Referencia", "TEXT");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Marca", "TEXT");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "Ativo", "INTEGER DEFAULT 1");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "GerarEtiqueta", "INTEGER DEFAULT 0");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "QuantidadeEtiqueta", "INTEGER DEFAULT 1");

                    // ⭐ AJUSTE SOLICITADO: Colunas para as promoções Web aparecerem
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "EmPromocao", "INTEGER DEFAULT 0");
                    AdicionarCampoSeNaoExistir(conn, "Mercadorias", "PrecoPromocional", "REAL DEFAULT 0");

                    AdicionarCampoSeNaoExistir(conn, "ConfiguracaoSync", "TipoOrigem", "TEXT DEFAULT 'SQL'");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inicializar banco local: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ⭐ NOVO: Adiciona campo na tabela se não existir
        /// </summary>
        private static void AdicionarCampoSeNaoExistir(SQLiteConnection conn, string tabela, string campo, string tipo)
        {
            try
            {
                // Verificar se campo existe
                string checkQuery = $"PRAGMA table_info({tabela})";
                bool existe = false;

                using (var cmd = new SQLiteCommand(checkQuery, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == campo)
                        {
                            existe = true;
                            break;
                        }
                    }
                }

                // Adicionar se não existir
                if (!existe)
                {
                    string alterQuery = $"ALTER TABLE {tabela} ADD COLUMN {campo} {tipo}";
                    using (var cmd = new SQLiteCommand(alterQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    System.Diagnostics.Debug.WriteLine($"✓ Campo {campo} adicionado à tabela {tabela}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar campo {campo}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sincroniza mercadorias do SQL Server para o SQLite local
        /// MANTIDO: Funcionalidade original preservada 100%
        /// </summary>
        public static int SincronizarMercadorias(string filtro = "", int limite = 0)
        {
            try
            {
                string sqlServerConnStr = DatabaseConfig.GetConnectionString();
                DatabaseConfig.ConfigData config = DatabaseConfig.LoadConfiguration();

                if (string.IsNullOrEmpty(sqlServerConnStr))
                {
                    throw new Exception("Configuração do SQL Server não encontrada!");
                }

                int registrosImportados = 0;

                // ⭐ PRIMEIRA TENTATIVA: Com todos os campos (incluindo VendaD e VendaE)
                try
                {
                    registrosImportados = ExecutarSincronizacao(sqlServerConnStr, config.Loja, filtro, limite, true);
                }
                catch (SqlException)
                {
                    // ⭐ SEGUNDA TENTATIVA: Sem VendaD e VendaE
                    System.Diagnostics.Debug.WriteLine("⚠️ Erro ao sincronizar com VendaD/VendaE. Tentando sem estes campos...");
                    registrosImportados = ExecutarSincronizacao(sqlServerConnStr, config.Loja, filtro, limite, false);
                }

                return registrosImportados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao sincronizar mercadorias: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ⭐ ATUALIZADO: Executa a sincronização marcando origem como SQL
        /// </summary>
        private static int ExecutarSincronizacao(string connectionString, string loja, string filtro, int limite, bool incluirVendaDE)
        {
            int registrosImportados = 0;

            using (var sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();

                // Montar query com ou sem VendaD/VendaE
                string camposVendaDE = incluirVendaDE ? "[VendaD] as VendaD,\n                            [VendaE] as VendaE," : "";

                string query = @"
            SELECT TOP " + (limite > 0 ? limite.ToString() : "999999") + @"
                [Código da Mercadoria] as CodigoMercadoria,
                [Cód Fabricante] as CodFabricante,
                [Cód Barra] as CodBarras,
                [Mercadoria],
                [Preço de Venda] as PrecoVenda,
                [VendaA] as VendaA,
                [VendaB] as VendaB,
                [VendaC] as VendaC,
                " + camposVendaDE + @"
                [Fornecedor] as Fornecedor,
                [Fabricante] as Fabricante,
                [Grupo] as Grupo,
                [Prateleira] as Prateleira,
                [Garantia] as Garantia,    
                [Tam] as Tam,
                [Cores] as Cores,
                [CodBarras] as CodBarras_Grade
            FROM [memoria_MercadoriasLojas]
            WHERE [Loja] = '" + loja + @"'
            " + (string.IsNullOrEmpty(filtro) ? "" : "AND " + filtro) + @"
                and [Desativado] = 0
            ORDER BY [Código da Mercadoria]
        ";

                using (var sqlCmd = new SqlCommand(query, sqlConn))
                using (var reader = sqlCmd.ExecuteReader())
                {
                    // Inserir no SQLite
                    using (var localConn = new SQLiteConnection(ConnectionString))
                    {
                        localConn.Open();

                        // ⭐ CORREÇÃO: Limpar TODOS os produtos
                        // Quando sincroniza SQL Server, NÃO deve ter produtos SoftcomShop
                        using (var deleteCmd = new SQLiteCommand("DELETE FROM Mercadorias", localConn))
                        {
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Também limpar produtos selecionados
                        using (var deleteCmd = new SQLiteCommand("DELETE FROM ProdutosSelecionados", localConn))
                        {
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Iniciar transação para performance
                        using (var transaction = localConn.BeginTransaction())
                        {
                            // ⭐ ATUALIZADO: Incluir campo Origem = 'SQL'
                            string insertQuery = @"
                        INSERT INTO Mercadorias 
                        (CodigoMercadoria, CodFabricante, CodBarras, Mercadoria, PrecoVenda, 
                         VendaA, VendaB, VendaC, VendaD, VendaE, 
                         Fornecedor, Fabricante, Grupo, Prateleira, Garantia, 
                         Tam, Cores, CodBarras_Grade, Origem)
                        VALUES 
                        (@cod, @fabr, @barras, @merc, @preco, 
                         @vendaA, @vendaB, @vendaC, @vendaD, @vendaE, 
                         @fornecedor, @fabricante, @grupo, @prateleira, @garantia, 
                         @tam, @cores, @codbarras_grade, 'SQL')
                    ";

                            using (var insertCmd = new SQLiteCommand(insertQuery, localConn))
                            {
                                while (reader.Read())
                                {
                                    insertCmd.Parameters.Clear();
                                    insertCmd.Parameters.AddWithValue("@cod", reader["CodigoMercadoria"]);
                                    insertCmd.Parameters.AddWithValue("@fabr", reader["CodFabricante"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@barras", reader["CodBarras"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@merc", reader["Mercadoria"]);
                                    insertCmd.Parameters.AddWithValue("@preco", reader["PrecoVenda"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@vendaA", reader["VendaA"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@vendaB", reader["VendaB"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@vendaC", reader["VendaC"] ?? DBNull.Value);

                                    // ⭐ VendaD e VendaE: Se não incluir, usa DBNull
                                    if (incluirVendaDE)
                                    {
                                        insertCmd.Parameters.AddWithValue("@vendaD", reader["VendaD"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@vendaE", reader["VendaE"] ?? DBNull.Value);
                                    }
                                    else
                                    {
                                        insertCmd.Parameters.AddWithValue("@vendaD", DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@vendaE", DBNull.Value);
                                    }

                                    insertCmd.Parameters.AddWithValue("@fornecedor", reader["Fornecedor"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@fabricante", reader["Fabricante"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@grupo", reader["Grupo"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@prateleira", reader["Prateleira"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@garantia", reader["Garantia"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@tam", reader["Tam"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@cores", reader["Cores"] ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@codbarras_grade", reader["CodBarras_Grade"] ?? DBNull.Value);

                                    insertCmd.ExecuteNonQuery();
                                    registrosImportados++;
                                }
                            }

                            transaction.Commit();
                        }

                        // ⭐ ATUALIZADO: Atualizar info de sincronização com tipo
                        string updateSync = @"
                    INSERT OR REPLACE INTO ConfiguracaoSync (Id, UltimaSincronizacao, TotalRegistros, TipoOrigem)
                    VALUES (1, datetime('now'), @total, 'SQL')
                ";
                        using (var syncCmd = new SQLiteCommand(updateSync, localConn))
                        {
                            syncCmd.Parameters.AddWithValue("@total", registrosImportados);
                            syncCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return registrosImportados;
        }

        /// <summary>
        /// Busca mercadorias no banco local
        /// MANTIDO: Funcionalidade original preservada
        /// </summary>
        public static DataTable BuscarMercadorias(string termoBusca = "", int limite = 100)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            CodigoMercadoria,
                            CodFabricante,
                            CodBarras,
                            Mercadoria,
                            PrecoVenda,
                            VendaA,
                            VendaB,
                            VendaC,
                            VendaD,
                            VendaE,
                            Fornecedor,
                            Fabricante,
                            Grupo,
                            Prateleira,
                            Garantia,
                            Tam,
                            Cores,
                            CodBarras_Grade,
                            Registro
                        FROM Mercadorias
                    ";

                    if (!string.IsNullOrEmpty(termoBusca))
                    {
                        query += @" 
                            WHERE Mercadoria LIKE @termo 
                            OR CodFabricante LIKE @termo 
                            OR CodBarras LIKE @termo
                            OR CodigoMercadoria = @codigo
                        ";
                    }

                    query += " ORDER BY Mercadoria LIMIT " + limite;

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(termoBusca))
                        {
                            cmd.Parameters.AddWithValue("@termo", "%" + termoBusca + "%");

                            // Tentar converter para número
                            if (int.TryParse(termoBusca, out int codigo))
                            {
                                cmd.Parameters.AddWithValue("@codigo", codigo);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@codigo", -1);
                            }
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadorias: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtém valores distintos de um campo específico
        /// MANTIDO: Funcionalidade original preservada
        /// </summary>
        public static DataTable ObterValoresDistintos(string campo)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"
                        SELECT DISTINCT {campo}
                        FROM Mercadorias
                        WHERE {campo} IS NOT NULL 
                        AND {campo} != ''
                        ORDER BY {campo}
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        DataTable dt = new DataTable();
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter valores distintos de {campo}: {ex.Message}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Busca mercadorias por múltiplos filtros
        /// MANTIDO: Funcionalidade original preservada
        /// </summary>
        //public static DataTable BuscarMercadoriasPorFiltros(string grupo = null, string fabricante = null, string fornecedor = null, bool isConfeccao = false)
        //{
        //    try
        //    {
        //        using (var conn = new SQLiteConnection(ConnectionString))
        //        {
        //            conn.Open();

        //            string query = @"
        //        SELECT 
        //            CodigoMercadoria,
        //            CodFabricante,
        //            CodBarras,
        //            Mercadoria,
        //            PrecoVenda,
        //            VendaA,
        //            VendaB,
        //            VendaC,
        //            VendaD,         
        //            VendaE,
        //            Fornecedor,
        //            Fabricante,
        //            Grupo,
        //            Prateleira,
        //            Garantia,
        //            Tam,
        //            Cores,
        //            CodBarras_Grade,
        //            Registro
        //        FROM Mercadorias
        //        WHERE 1=1
        //    ";

        //            List<string> condicoes = new List<string>();

        //            if (!string.IsNullOrEmpty(grupo))
        //                condicoes.Add("Grupo = @grupo");

        //            if (!string.IsNullOrEmpty(fabricante))
        //                condicoes.Add("Fabricante = @fabricante");

        //            if (!string.IsNullOrEmpty(fornecedor))
        //                condicoes.Add("Fornecedor = @fornecedor");

        //            if (condicoes.Count > 0)
        //                query += " AND " + string.Join(" AND ", condicoes);

        //            if (isConfeccao)
        //            {
        //                query += " ORDER BY Mercadoria, Tam, Cores";
        //            }
        //            else
        //            {
        //                query += " ORDER BY Mercadoria";
        //            }

        //            using (var cmd = new SQLiteCommand(query, conn))
        //            {
        //                if (!string.IsNullOrEmpty(grupo))
        //                    cmd.Parameters.AddWithValue("@grupo", grupo);

        //                if (!string.IsNullOrEmpty(fabricante))
        //                    cmd.Parameters.AddWithValue("@fabricante", fabricante);

        //                if (!string.IsNullOrEmpty(fornecedor))
        //                    cmd.Parameters.AddWithValue("@fornecedor", fornecedor);

        //                DataTable dt = new DataTable();
        //                using (var adapter = new SQLiteDataAdapter(cmd))
        //                {
        //                    adapter.Fill(dt);
        //                }

        //                System.Diagnostics.Debug.WriteLine($"BuscarMercadoriasPorFiltros (isConfeccao={isConfeccao}): {dt.Rows.Count} linhas");

        //                return dt;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Erro ao buscar mercadorias por filtros: {ex.Message}");
        //        return new DataTable();
        //    }
        //}


        public static DataTable BuscarMercadoriasPorFiltros(
    string grupo = null,
    string fabricante = null,
    string fornecedor = null,
    bool isConfeccao = false,
    int? idPromocao = null) // ⭐ Novo parâmetro opcional
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // Adicionamos PrecoVenda as PrecoOriginal e PrecoPromocional para o Grid reconhecer
                    string query = @"
                SELECT 
                    CodigoMercadoria,
                    CodFabricante,
                    CodBarras,
                    Mercadoria,
                    PrecoVenda,
                    PrecoVenda as PrecoOriginal, 
                    PrecoPromocional,
                    VendaA, VendaB, VendaC, VendaD, VendaE,
                    Fornecedor,
                    Fabricante,
                    Grupo,
                    Prateleira,
                    Garantia,
                    Tam,
                    Cores,
                    CodBarras_Grade,
                    Registro,
                    ID_Promocao
                FROM Mercadorias
                WHERE 1=1
            ";

                    List<string> condicoes = new List<string>();

                    if (!string.IsNullOrEmpty(grupo))
                        condicoes.Add("Grupo = @grupo");

                    if (!string.IsNullOrEmpty(fabricante))
                        condicoes.Add("Fabricante = @fabricante");

                    if (!string.IsNullOrEmpty(fornecedor))
                        condicoes.Add("Fornecedor = @fornecedor");

                    // ⭐ FILTRO DE PROMOÇÃO
                    if (idPromocao.HasValue)
                        condicoes.Add("EmPromocao = 1 AND ID_Promocao = @idPromocao");

                    if (condicoes.Count > 0)
                        query += " AND " + string.Join(" AND ", condicoes);

                    query += isConfeccao ? " ORDER BY Mercadoria, Tam, Cores" : " ORDER BY Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(grupo)) cmd.Parameters.AddWithValue("@grupo", grupo);
                        if (!string.IsNullOrEmpty(fabricante)) cmd.Parameters.AddWithValue("@fabricante", fabricante);
                        if (!string.IsNullOrEmpty(fornecedor)) cmd.Parameters.AddWithValue("@fornecedor", fornecedor);

                        // ⭐ SETA O ID DA PROMOÇÃO
                        if (idPromocao.HasValue) cmd.Parameters.AddWithValue("@idPromocao", idPromocao.Value);

                        DataTable dt = new DataTable();
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar mercadorias: {ex.Message}");
                return new DataTable();
            }
        }

        // ========================================
        // TODOS OS OUTROS MÉTODOS MANTIDOS IGUAIS
        // ========================================

        public static DataTable BuscarMercadorias(string termoBusca, string nomeCampo, int limite = 500)
        {
            if (string.IsNullOrWhiteSpace(nomeCampo))
                return BuscarMercadorias(termoBusca, limite);

            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"
                        SELECT DISTINCT 
                            CodigoMercadoria,
                            CodFabricante,
                            CodBarras,
                            Mercadoria,
                            PrecoVenda,
                            VendaA,
                            VendaB,
                            VendaC,
                            VendaD,
                            VendaE,
                            Fornecedor,
                            Fabricante,
                            Grupo,
                            Prateleira,
                            Garantia,
                            Tam,
                            Cores,
                            CodBarras_Grade,
                            Registro
                        FROM Mercadorias
                        WHERE {nomeCampo} LIKE @termo
                        ORDER BY {nomeCampo}
                        LIMIT {limite}";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@termo", "%" + termoBusca + "%");

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadorias por {nomeCampo}: {ex.Message}", ex);
            }
        }

        public static DataRow BuscarMercadoriaPorCodigo(int codigo)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT * FROM Mercadorias 
                        WHERE CodigoMercadoria = @codigo
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadoria: {ex.Message}", ex);
            }
        }

        public static void AdicionarProdutoSelecionado(int codigoMercadoria, string nome,
            string codigo, decimal preco, int quantidade)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO ProdutosSelecionados 
                        (CodigoMercadoria, Nome, Codigo, Preco, Quantidade)
                        VALUES (@codMerc, @nome, @cod, @preco, @qtd)
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codMerc", codigoMercadoria);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@cod", codigo);
                        cmd.Parameters.AddWithValue("@preco", preco);
                        cmd.Parameters.AddWithValue("@qtd", quantidade);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar produto: {ex.Message}", ex);
            }
        }

        public static DataTable ObterProdutosSelecionados()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT * FROM ProdutosSelecionados 
                        ORDER BY DataSelecao DESC
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter produtos: {ex.Message}", ex);
            }
        }

        public static void LimparProdutosSelecionados()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM ProdutosSelecionados", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao limpar produtos: {ex.Message}", ex);
            }
        }

        public static void RemoverProdutoSelecionado(int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM ProdutosSelecionados WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover produto: {ex.Message}", ex);
            }
        }

        public static (DateTime? ultima, int total) ObterInfoSincronizacao()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT UltimaSincronizacao, TotalRegistros FROM ConfiguracaoSync WHERE Id = 1";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime? ultima = reader["UltimaSincronizacao"] != DBNull.Value
                                ? Convert.ToDateTime(reader["UltimaSincronizacao"])
                                : (DateTime?)null;

                            int total = reader["TotalRegistros"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalRegistros"])
                                : 0;

                            return (ultima, total);
                        }
                    }
                }

                return (null, 0);
            }
            catch
            {
                return (null, 0);
            }
        }

        public static bool PrecisaSincronizar()
        {
            try
            {
                // ✅ VERIFICAR SE ESTÁ CONFIGURADO PARA SQL SERVER
                var configSistema = ConfiguracaoSistema.Carregar();

                // Se for SoftcomShop, NÃO pedir sincronização automática do SQL
                if (configSistema != null && configSistema.TipoConexaoAtiva == TipoConexao.SoftcomShop)
                {
                    return false; // SoftcomShop tem sua própria sincronização
                }

                // Se não tiver SQL Server configurado, não pedir sincronização
                string sqlConnectionString = DatabaseConfig.GetConnectionString();
                if (string.IsNullOrEmpty(sqlConnectionString))
                {
                    return false; // Não tem SQL Server configurado
                }

                // ✅ VERIFICAR SE JÁ TEM PRODUTOS NO BANCO LOCAL
                int totalProdutos = ObterTotalMercadorias();
                if (totalProdutos > 0)
                {
                    // Tem produtos, verificar data da última sincronização
                    var (ultima, _) = ObterInfoSincronizacao();

                    if (ultima == null)
                    {
                        // Tem produtos mas não tem registro de sincronização
                        // Provavelmente importado do SoftcomShop
                        return false;
                    }

                    // Verificar se passou mais de 24h
                    return (DateTime.Now - ultima.Value).TotalHours > 24;
                }
                else
                {
                    // ✅ NÃO TEM PRODUTOS - Perguntar se quer sincronizar
                    return true;
                }
            }
            catch
            {
                return false; // Em caso de erro, não forçar sincronização
            }
        }

        public static int ObterTotalMercadorias()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Mercadorias", conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static (List<string> tamanhos, List<string> cores) BuscarTamanhosECoresPorCodigo(int codigo)
        {
            var tamanhos = new List<string>();
            var cores = new List<string>();

            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT Tam, Cores
                        FROM Mercadorias
                        WHERE CodigoMercadoria = @codigo
                        AND (Tam IS NOT NULL OR Cores IS NOT NULL)
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tam = reader["Tam"]?.ToString();
                                if (!string.IsNullOrEmpty(tam))
                                {
                                    if (!tamanhos.Contains(tam))
                                        tamanhos.Add(tam);
                                }

                                string cor = reader["Cores"]?.ToString();
                                if (!string.IsNullOrEmpty(cor))
                                {
                                    if (!cores.Contains(cor))
                                        cores.Add(cor);
                                }
                            }
                        }
                    }
                }

                tamanhos.Sort();
                cores.Sort();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar tamanhos e cores: {ex.Message}", ex);
            }

            return (tamanhos, cores);
        }

        public static string BuscarCodigoBarrasPorCodTamCor(string codigo, string tamanho, string cor)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT CodBarras_Grade
                        FROM Mercadorias
                        WHERE CodigoMercadoria = @codigo
                        AND Tam = @tamanho
                        AND Cores = @cor
                        LIMIT 1
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);
                        cmd.Parameters.AddWithValue("@tamanho", tamanho);
                        cmd.Parameters.AddWithValue("@cor", cor);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar código de barras: {ex.Message}");
                return string.Empty;
            }
        }

        public static DataTable BuscarMercadoriaPorCodTamCor(string codigo, string tamanho, string cor)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT *
                        FROM Mercadorias
                        WHERE CodigoMercadoria = @codigo
                        AND Tam = @tamanho
                        AND Cores = @cor
                        LIMIT 1
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);
                        cmd.Parameters.AddWithValue("@tamanho", tamanho);
                        cmd.Parameters.AddWithValue("@cor", cor);

                        DataTable dt = new DataTable();
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar mercadoria: {ex.Message}");
                return null;
            }
        }

        public static DataTable ObterSubGruposPorGrupo(string grupo)
        {
            return LocalDatabaseManagerExtensions.ObterSubGruposPorGrupo(grupo);
        }

        public static DataTable ObterGruposDoSQLServer()
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("SQL Server não configurado, usando dados locais");
                    return ObterValoresDistintos("Grupo");
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT g.GRP_DESCRI AS Grupo
                        FROM grp g
                        INNER JOIN memoria_MercadoriasLojas m ON m.GRUPO = g.GRP_DESCRI
                        WHERE g.GRP_DESCRI IS NOT NULL 
                        AND g.GRP_DESCRI != ''
                        ORDER BY g.GRP_DESCRI
                    ";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        DataTable dt = new DataTable();
                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter grupos do SQL Server: {ex.Message}");
                return ObterValoresDistintos("Grupo");
            }
        }

        public static DataTable ObterFabricantesDoSQLServer()
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("SQL Server não configurado, usando dados locais");
                    return ObterValoresDistintos("Fabricante");
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT f.GRP_DESCRI AS Fabricante
                        FROM Fabricante f
                        INNER JOIN memoria_MercadoriasLojas m ON m.FABRICANTE = f.GRP_DESCRI
                        WHERE f.GRP_DESCRI IS NOT NULL 
                        AND f.GRP_DESCRI != ''
                        ORDER BY f.GRP_DESCRI
                    ";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        DataTable dt = new DataTable();
                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter fabricantes do SQL Server: {ex.Message}");
                return ObterValoresDistintos("Fabricante");
            }
        }
        public static async Task SincronizarPromocoesDeAcordoComOrigem()
        {
            var config = ConfiguracaoSistema.Carregar();
            if (config.TipoConexaoAtiva == TipoConexao.SoftcomShop)
            {
                // USA O GETCONNECTIONSTRING DO PRÓPRIO LOCALDATABASEMANAGER
                var dsManager = new SoftcomShopDataManager(config.SoftcomShop, GetConnectionString());
                await dsManager.SincronizarPromocoesAtivasAsync();
            }
        }

    }
}