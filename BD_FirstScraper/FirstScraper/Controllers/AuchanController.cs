using FirstScraper.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuchanController : ControllerBase
    {
        private readonly ILogger<AuchanController> _logger;

        public AuchanController(ILogger<AuchanController> logger)
        {
            _logger = logger;
        }
        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url = "https://www.auchan.fr/lego-lego-technic-42171-mercedes-amg-f1-w14-e-performance-replique-decoration-de-bureau/pr-C1784561")
        {
            var product = new ProductDetails
            {
                Name = string.Empty,

                mrp = string.Empty,

                Delivery = string.Empty,

                ImageUrl = [],

                description = [],

                productspecification = new Dictionary<string, string>(),
                display = [],

                processor = [],

                datacarrier = [],

                camera = [],

                network = [],

                portsandinterfaces = [],

                sendingmessages = [],

                design = [],

                Efficiency = [],

                Multimedia = [],

                Software = [],

                Weightanddimensions = [],

                PackageContents = [],

                AdditionalInformation = [],

                Insurance = string.Empty,

                paymentmethod = string.Empty,

                productdelivery = string.Empty,

                productservice = string.Empty,

                ////Discription = string.Empty,

                TechnicalData = new Dictionary<string, string>(),

            };

            try
            {

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(response);

                    var productcategory = document.DocumentNode.SelectNodes("//div[contains(@class,\"site-breadcrumb__container\")]//div//span[contains(@class,\"site-breadcrumb__item\")]/a");
                    if (productcategory != null)
                    {
                        product.productcategory = productcategory
                            .Select(row => $"{row.InnerText.Trim()}")
                            .ToArray();
                    }

                    var brandNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"offer-selector__name--large\")]/a/bold");
                    if (brandNode != null)
                    {
                        product.Brandofproduct = brandNode.InnerText.Trim();
                    }

                    var nameNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"offer-selector__name--large\")]/h1");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var modelNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productModelValue\"]");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//*[@id=\"wrapper\"]//div[contains(@class,\"offer-selector__add2cart-wrapper\")]/section[1]/div/div[2]");
                    if (mrp != null)
                    {
                        product.mrp = mrp.InnerText.Trim();
                    }
                    
                    
                    var imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\"product-gallery__item\")]//nav/button/img");
                    if (imageNodes != null)
                    {
                        product.ImageUrl = imageNodes
                            .Select(node => node.GetAttributeValue("src", string.Empty))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .ToArray();
                    }

                    var rating = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"offer-selector__description-header\")]//div[contains(@class,\"rating-value__wrapper\")]//span[1]");
                    if (rating != null)
                    {
                        product.rating = rating.InnerText.Trim();
                    }

                    var specificationNodes = document.DocumentNode.SelectNodes("//div[@id='product-features']//div[contains(@class, 'product-description__feature-group-wrapper')]");
                    if (specificationNodes != null)
                    {
                        foreach (var detailNode in specificationNodes)
                        {
                            var keyNode = detailNode.SelectSingleNode(".//span[contains(@class, 'product-description__feature-label')]");
                            var valueNode = detailNode.SelectSingleNode(".//span[contains(@class, 'product-description__feature-value')]");

                            if (keyNode != null && valueNode != null)
                            {
                                string specName = keyNode.InnerText.Trim();
                                string specValue = valueNode.InnerText.Trim();
                                product.productspecification[specName] = specValue;
                            }
                        }
                    }


                    var descriptionRow = document.DocumentNode.SelectNodes("//*[@id=\"product-description-only\"]/div/div/div/div[1]");
                    if (descriptionRow != null)
                    {
                        product.description = descriptionRow
                            .Select(row => row.InnerText.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                            .ToArray();
                    }


                }
                
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, $"Error fetching product details: {e.Message}");
            }

            return Ok(product);
        }
    }
}
