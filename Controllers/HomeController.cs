using API_for_tariffs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_for_tariffs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TariffService _tariffsParser;
        public HomeController(ILogger<HomeController> logger, TariffService tariffsParser)
        {
            _logger = logger;
            _tariffsParser = tariffsParser;
        }

        /// <param name="categoryId">
        /// Идентификатор категории тарифа:
        /// - 1: Тариф для населення та колективних побутових споживачів без електроопалення
        /// - 2: Тариф для гуртожитків (та інших, хто має право на ціни для побутових споживачів)
        /// - 3: Тариф для населення та колективних побутових споживачів з електроопаленням (при споживанні до 2000 кВт∙год/міс)
        /// - 4: Тариф для населення та колективних побутових споживачів з електроопаленням (при споживанні понад 2000 кВт∙год/міс)
        /// </param>

        [HttpGet("api/electricity")]
        public async Task<IActionResult> GetTariffs(int categoryId)
        {
            var PriceWithoutVat = await _tariffsParser.GetPriceWithoutVat(categoryId);
            var VatPrice = await _tariffsParser.GetVatPrice(categoryId);
            var PriceWithVat = await _tariffsParser.GetPriceWithVat(categoryId);

            if (VatPrice == null & PriceWithoutVat == null && PriceWithVat == null) 
            {
                return NotFound();
            }

            var result = new
            {
                PriceWithoutVat = PriceWithoutVat,
                VatPrice = VatPrice,
                PriceWithVat = PriceWithVat
            };

            return Ok(result);
        }



        public IActionResult Index()
        {
            return Ok("ok");
        }

       

       
    }
}
