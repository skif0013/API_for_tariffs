//using HtmlAgilityPack;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//public class TariffsParser
//{
//    private readonly HttpClient _httpClient;

//    public TariffsParser(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    public async Task<List<string>> GetTariffsData(int dataCount)
//    {
//        var url = "https://yasno.com.ua/b2c-tariffs";
//        var response = await _httpClient.GetStringAsync(url);

//        var htmlDoc = new HtmlDocument();
//        htmlDoc.LoadHtml(response);

//        // Находим нужную категорию по data-count
//        var categoryNode = htmlDoc.DocumentNode
//            .SelectSingleNode($"//article[contains(@class, 'partial-tariff') and @data-count='{dataCount}']");

//        if (categoryNode == null) return new List<string>();

//        var priceNodes = categoryNode.SelectNodes(".//div[contains(@class, 'partial-tariff-price')]");
//        var tariffs = new List<string>();

//        if (priceNodes != null)
//        {
//            foreach (var priceNode in priceNodes)
//            {
//                var title = priceNode.SelectSingleNode(".//h3[contains(@class, 'partial-tariff-price__title')]").InnerText.Trim();
//                var value = priceNode.SelectSingleNode(".//strong[contains(@class, 'partial-tariff-price__value')]").InnerText.Trim();
//                var unit = priceNode.SelectSingleNode(".//span[contains(@class, 'partial-tariff-price__unit')]").InnerText.Trim();

//                tariffs.Add($"{title}: {value} {unit}");
//            }
//        }

//        return tariffs;
//    }
//}



using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_for_tariffs
{
    public class TariffService
    {
        private readonly HttpClient _httpClient;

        public TariffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<HtmlNode> GetCategoryNode(int categoryId)
        {
            var url = "https://yasno.com.ua/b2c-tariffs";
            var response = await _httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Ищем категорию по data-count
            return htmlDoc.DocumentNode.SelectSingleNode($"//article[@data-count='{categoryId}']");
        }

        public async Task<string> GetPriceWithoutVat(int categoryId)
        {
            var categoryNode = await GetCategoryNode(categoryId);
            var priceNode = categoryNode?.SelectSingleNode(".//div[contains(@class, 'partial-tariff-price')]//h3[text()='без ПДВ']/following-sibling::strong");

            return priceNode != null ? priceNode.InnerText.Trim() + " грн/кВт·год" : null;
        }

        public async Task<string> GetVatPrice(int categoryId)
        {
            var categoryNode = await GetCategoryNode(categoryId);
            var vatNode = categoryNode?.SelectSingleNode(".//div[contains(@class, 'partial-tariff-price')]//h3[text()='ПДВ']/following-sibling::strong");

            return vatNode != null ? vatNode.InnerText.Trim() + " грн/кВт·год" : null;
        }

        public async Task<string> GetPriceWithVat(int categoryId)
        {
            var categoryNode = await GetCategoryNode(categoryId);
            var priceWithVatNode = categoryNode?.SelectSingleNode(".//div[contains(@class, 'partial-tariff-price')]//h3[text()='з ПДВ']/following-sibling::strong");

            return priceWithVatNode != null ? priceWithVatNode.InnerText.Trim() + " грн/кВт·год" : null;
        }
    }
}
