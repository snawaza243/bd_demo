using BD_First.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FirstScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlsenController : ControllerBase
    {
        private readonly ILogger<AlsenController> _logger;
        public AlsenController(ILogger<AlsenController> logger)
        {
            _logger = logger;
        }

        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url= "https://www.alsen.pl/canon-pixma-g3410-2315c009aa")
        {
            var product = new ProductDetails
            {
                Name = string.Empty,

                mrp = string.Empty,

                Delivery = string.Empty,

                ImageUrl = [],

                description = [],
                TechnicalData = new Dictionary<string, string>(),
                productspecification = new Dictionary<string, string>(),


            };

            try
            {

                using (HttpClient client = new HttpClient())
                {
                    // Set a timeout
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Set User-Agent header to mimic a browser request
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

                    // Attempt to fetch the response
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Failed to fetch product page. Status code: {response.StatusCode}");
                        return StatusCode((int)response.StatusCode, $"Failed to fetch product details. Status code: {response.StatusCode}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(responseBody);

                    _logger.LogInformation(document.DocumentNode.OuterHtml);

                    var productCategory = document.DocumentNode.SelectNodes("//ul[contains(@class,'breadcrumb')]//li//span");
                    if (productCategory != null)
                    {
                        product.productcategory = productCategory
                            .Select(row => row.InnerText.Trim())
                            .ToArray();
                    }

                    var nameNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,'product-name')]//span");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var skuNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"codes-producer\")]//div/strong[1]");
                    if (skuNode != null)
                    {
                        product.sku = skuNode.InnerText.Trim();
                    }

                    var modelNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"codes-producer\")]//div[contains(@class,\"product-code\")]/strong");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var availableNode = document.DocumentNode.SelectSingleNode("/html/body/div[6]/div/div[1]/section/div[2]/div[1]/div[1]/div[2]/div[6]/div/strong/font/font");
                    if (availableNode != null)
                    {
                        product.Availibility = availableNode.InnerText.Trim();
                    }

                    var deliveryNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"product-additions product-shipping\")]//div/strong/font");
                    if (deliveryNode != null)
                    {
                        product.Delivery = deliveryNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//div[@class=\"product-price\"]//div/span[1]");
                    if (mrp != null)
                    {
                        product.mrp = mrp.InnerText.Trim();
                    }

                    var sellPrice = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"sales on-sale clearfix\")]/strong/span");
                    if (sellPrice != null)
                    {
                        product.sellingprice = sellPrice.InnerText.Trim();
                    }

                    var pricediscount = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"savings-amount\")]//strong//span[2]");
                    if (pricediscount != null)
                    {
                        product.pricediscount = pricediscount.InnerText.Trim();
                    }

                    var imageNodes = document.DocumentNode.SelectNodes("//ul[contains(@class,'carousel-indicators')]//li//span[contains(@class,\"preload-image\")]//img");
                    if (imageNodes != null)
                    {
                        product.ImageUrl = imageNodes
                            .Select(node => node.GetAttributeValue("data-src", string.Empty))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .ToArray();
                    }

                    var technicaldataNodes = document.DocumentNode.SelectNodes("//div[contains(@class,'product-tab-content cms-content-tab dane-techniczne')]//table//tr");
                    if (technicaldataNodes != null)
                    {
                        foreach (var detailNode in technicaldataNodes)
                        {
                            var keyNode = detailNode.SelectSingleNode("./td[1]");
                            var valueNode = detailNode.SelectSingleNode("./td[2]");

                            if (keyNode != null && valueNode != null)
                            {
                                //string techDataName = keyNode.InnerText.Trim();
                                //string techDataValue = valueNode.InnerText.Trim();

                                string key = Regex.Replace(keyNode.InnerText, @"\s+", " ").Trim();
                                string value = Regex.Replace(valueNode.InnerText, @"\s+", " ").Trim();

                                //            return $"{key}: {value}";

                                product.productspecification[key] = value;
                            }
                        }
                    }

                    //var specificationRows = document.DocumentNode.SelectNodes("//div[contains(@class,'product-tab-content cms-content-tab dane-techniczne')]//table//tr");
                    //if (specificationRows != null)
                    //{
                    //    product.Specification = specificationRows
                    //    .Select(row =>
                    //    {
                    //         var keyNode = row.SelectSingleNode("./td[1]");
                    //         var valueNode = row.SelectSingleNode("./td[2]");

                    //        if (keyNode != null && valueNode != null)
                    //        {
                    //            // Clean up the text by removing tabs, newlines, and extra spaces
                    //            string key = Regex.Replace(keyNode.InnerText, @"\s+", " ").Trim();
                    //            string value = Regex.Replace(valueNode.InnerText, @"\s+", " ").Trim();

                    //            return $"{key}: {value}";
                    //        }
                    //        return null;
                    //    })
                    //   .Where(item => item != null)
                    //   .ToArray();
                    //}

                    var productDetailsRows = document.DocumentNode.SelectNodes("//table[contains(@class,\'product-attributes\')]//tr");
                    if (productDetailsRows != null)
                    {
                        foreach (var row in productDetailsRows)
                        {
                            var keyNode = row.SelectSingleNode("./td[1]");
                            var valueNode = row.SelectSingleNode("./td[2]");

                            if (keyNode != null && valueNode != null)
                            {
                                // Clean up the text by removing tabs, newlines, and extra spaces
                                string key = Regex.Replace(keyNode.InnerText, @"\s+", " ").Trim();
                                string value = Regex.Replace(valueNode.InnerText, @"\s+", " ").Trim();

                                // Store the key-value pair in the dictionary
                                product.TechnicalData[key] = value;
                            }
                        }
                    }


                    var descriptionRow = document.DocumentNode.SelectNodes("//div[contains(@class,\"product-description\")]");
                    if (descriptionRow != null)
                    {
                        product.description = descriptionRow
                            .Select(row => row.InnerText.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                            .ToArray();
                    }

                    var reviews = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"vg yc hs\")]"); // Selects the first <font> with non-empty text
                    if (reviews != null)
                    {
                        product.reviews = reviews.InnerText.Trim();
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
