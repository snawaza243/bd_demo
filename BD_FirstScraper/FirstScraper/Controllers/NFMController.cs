using FirstCrawler.Controllers;
using FirstScraper.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FirstScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NFMController : ControllerBase
    {
        private readonly ILogger<NFMController> _logger;
        public NFMController(ILogger<NFMController> logger)
        {
            _logger = logger;
        }
        
      
        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url = "https://www.nfm.com/samsung-85-class-du7200-4k-crystal-uhd-with-hdr-in-titan-gray--smart-tv-66012139/66012139.html")
        {
            var product = new ProductDetails
            {
                Name = string.Empty,
                mrp = string.Empty,
                ImageUrl = Array.Empty<string>(),
                description = Array.Empty<string>(),
                productcategory = Array.Empty<string>(),
                productspecification = new Dictionary<string, string>(),
            };


            try
            {

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(response);

                    _logger.LogInformation(document.DocumentNode.OuterHtml);

                    var productcategory = document.DocumentNode.SelectNodes("//div[contains(@class,\"col\")]//ol[contains(@class,\"breadcrumb\")]/li");
                    if (productcategory != null)
                    {
                        product.productcategory = productcategory
                            .Select(row => $"{row.InnerText.Trim()}")
                            .ToArray();
                    }

                    
                    var nameNode = document.DocumentNode.SelectSingleNode("//h1[contains(@class,\"product-name\")]");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var skuNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"product-sku-container label2\")]//span[2]");
                    if (skuNode != null)
                    {
                        product.sku = skuNode.InnerText.Trim();
                    }

                    var modelNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"product-manufacturer-sku-container label2\")]//span[2]");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"price pdp\")]//span[2]");
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

                    var imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\'slide-image-container\')]//img");
                    if (imageNodes != null)
                    {
                        product.ImageUrl = imageNodes
                            .Select(node => node.GetAttributeValue("src", string.Empty))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .ToArray();
                    }



                    var technicaldataNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'pdp-table')]");
                    if (technicaldataNodes != null)
                    {
                        foreach (var categoryNode in technicaldataNodes)
                        {
                            // Extract the category title
                            var categoryTitleNode = categoryNode.SelectSingleNode(".//div[@class='card-header']");
                            string categoryTitle = categoryTitleNode?.InnerText.Trim() ?? "Unknown Category";

                            // Extract individual specifications within the category
                            var specNodes = categoryNode.SelectNodes(".//div[contains(@class, 'spec-attributes')]//div[contains(@class, 'row no-gutters')]");

                            if (specNodes != null)
                            {
                                foreach (var detailNode in specNodes)
                                {
                                    // Extract key and value nodes
                                    var keyNode = detailNode.SelectSingleNode(".//span[contains(@class, 'attribute')]");
                                    var valueNode = detailNode.SelectSingleNode(".//span[contains(@class, 'value')]");

                                    if (keyNode != null && valueNode != null)
                                    {
                                        string techDataName = keyNode.InnerText.Trim();
                                        string techDataValue = valueNode.InnerText.Trim();

                                        // Store the key-value pair in the product specification dictionary
                                        product.productspecification[techDataName] = techDataValue;
                                    }
                                }
                            }
                        }
                    }


                    var descriptionRow = document.DocumentNode.SelectNodes("//div[contains(@class,\"product-description\")]");
                    if (descriptionRow != null)
                    {
                        product.description = descriptionRow
                            .Select(row =>
                            {
                                string cleanText = Regex.Replace(row.InnerText, @"\s+", " ").Trim();
                                return cleanText;
                            })
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
