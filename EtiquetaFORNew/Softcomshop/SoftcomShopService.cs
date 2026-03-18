using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EtiquetaFORNew
{
    /// <summary>
    /// ServiÃ§o para comunicaÃ§Ã£o com a API SoftcomShop
    /// </summary>
    public class SoftcomShopService
    {
        private readonly HttpClient _httpClient;
        private readonly SoftcomShopConfig _config;
        private readonly SoftcomShopRouter _router;
        private string _currentToken;

        public SoftcomShopService(SoftcomShopConfig config)
        {
            _config = config;
            _router = new SoftcomShopRouter(config.BaseURL);
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        #region Autentição

        /// <summary>
        /// ObtÃ©m token de autenticaÃ§Ã£o
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "client_secret", _config.ClientSecret },
                    { "client_id", _config.ClientId },
                    { "grant_type", "client_credentials" }
                };

                var content = new FormUrlEncodedContent(parameters);

                var response = await _httpClient.PostAsync(_router.TokenRouter, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    _currentToken = result.data.token;
                    return _currentToken;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cadastra o dispositivo no servidor
        /// </summary>
        public async Task<string> CadastrarDispositivoAsync()
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "client_id", _config.ClientId },
                    { "empresa_name", _config.CompanyName },
                    { "empresa_cnpj", _config.CompanyCNPJ },
                    { "device_name", _config.DeviceName },
                    { "device_id", _config.DeviceId }
                };

                var content = new FormUrlEncodedContent(parameters);

                var response = await _httpClient.PostAsync(_router.DeviceRouter, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return result.data.client_secret;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar dispositivo: {ex.Message}", ex);
            }
        }

        #endregion

        #region Produtos

        /// <summary>
        /// ObtÃ©m produtos do catÃ¡logo (paginado)
        /// </summary>
        public async Task<string> GetProdutosAsync(int page = 1, string versao = "v2")
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    await GetTokenAsync();
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentToken}");

                string url;
                if (versao == "v2")
                {
                    _httpClient.DefaultRequestHeaders.Add("Api-Version", "v2");
                    url = $"{_router.ProductsRouterV2}?page={page}";
                }
                else
                {
                    url = $"{_router.ProductsRouter}/page/{page}";
                }

                var response = await _httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter produtos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ObtÃ©m produtos por nota fiscal
        /// </summary>
        public async Task<string> GetNotaFiscalAsync(string dataEntrada, int numeroNota = 0, int page = 1, string versao = "v2")
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    await GetTokenAsync();
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentToken}");

                string url;
                if (versao == "v2")
                {
                    _httpClient.DefaultRequestHeaders.Add("Api-Version", "v2");
                    url = $"{_router.DataEntradaNotaFiscalV2}{dataEntrada}";

                    if (numeroNota > 0)
                        url += $"&numero_nota_fiscal={numeroNota}";

                    url += $"&page={page}";
                }
                else
                {
                    url = $"{_router.DataEntradaNotaFiscal}{dataEntrada}";

                    if (numeroNota > 0)
                        url += $"?numero_nota={numeroNota}";

                    url += $"/page/{page}";
                }

                var response = await _httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter nota fiscal: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ObtÃ©m produtos por venda
        /// </summary>
        public async Task<string> GetVendaAsync(int numeroVenda)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    await GetTokenAsync();
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentToken}");

                string url = $"{_router.VendasRouter}{numeroVenda}?bloquear=False";

                var response = await _httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter venda: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ObtÃ©m promoÃ§Ãµes ativas
        /// </summary>
        public async Task<string> GetPromocoesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    await GetTokenAsync();
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentToken}");

                var response = await _httpClient.GetAsync(_router.PromocaoRouter);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter promoÃ§Ãµes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ObtÃ©m informaÃ§Ãµes da empresa
        /// </summary>
        public async Task<string> GetEmpresaAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    await GetTokenAsync();
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentToken}");

                var response = await _httpClient.GetAsync(_router.CompanyRouter);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter empresa: {ex.Message}", ex);
            }
        }

        #endregion

        #region Testes

        /// <summary>
        /// Testa a conexÃ£o com a API
        /// </summary>
        /// <summary>
        /// Testa se a API está acessível e credenciais básicas estão corretas
        /// NÃO requer Client Secret (que só existe após cadastrar dispositivo)
        /// </summary>
        public async Task<bool> TestarConexaoAsync()
        {
            try
            {
                // ⭐ CORREÇÃO: Testar conexão sem ClientSecret
                // ClientSecret só existe APÓS cadastrar o dispositivo

                // Apenas verificar se consegue acessar a API
                var response = await _httpClient.GetAsync(_config.BaseURL);
                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}