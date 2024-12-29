using FirstScraper.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchaferController : ControllerBase
    {
        private readonly ILogger<SchaferController> _logger;
        public SchaferController(ILogger<SchaferController> logger)
        {
            _logger = logger;
        }
        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url = "https://www.schaefer-shop.de/p/ordner-gruener-balken-din-a4-rueckenbreite-80-mm?selectedItem=183980")
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
                    _logger.LogInformation(document.DocumentNode.OuterHtml);

                    // Select each <a> node within the <li> tags inside the <ol> list within <div class="tailwind">
                    var productCategoryNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\'tailwind\')]//ol/li/a");

                    if (productCategoryNodes != null)
                    {
                        // Extract the text of each category link and store it as an array
                        product.productcategory = productCategoryNodes
                            .Select(node => node.InnerText.Trim())
                            .ToArray();
                    }


                    var brandNode = document.DocumentNode.SelectSingleNode("//*[@id=\"main_area\"]/div[4]/div/div[2]/ul/li[6]/a/span/font/font");
                    if (brandNode != null)
                    {
                        product.Brandofproduct = brandNode.InnerText.Trim();
                    }

                    var nameNode = document.DocumentNode.SelectSingleNode("//*[@id=\'article-title\']");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var skuNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\'pda-header-info\')]//span[@class='js-article-id']");
                    if (skuNode != null)
                    {
                        product.sku = skuNode.InnerText.Trim();
                    }


                    var modelNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productModelValue\"]");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//*[@id='article-price']//span[contains(@class,\'price-detail-offer\')]");

                    if (mrp != null)
                    {
                        product.mrp = mrp.InnerText.Trim();
                    }

                    var Currency = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"info-row-item info-row-pricing\")]/span/span[1]");
                    if (Currency != null)
                    {
                        product.Currency = Currency.InnerText.Trim();
                    }

                    var deliveryNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\'pda-cta-delivery\')]//span[contains(@class,\'article-delivery-time\')]");

                    if (deliveryNode != null)
                    {
                        product.Delivery = deliveryNode.InnerText.Trim();
                    }


                    //var pricediscount = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"sa\")]");
                    //if (pricediscount != null)
                    //{
                    //    product.pricediscount = pricediscount.InnerText.Trim();
                    //}
                    var imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\"pda-left\")]//ul[contains(@id,\"pda-gallery-thumbnails\")]//img");
                    if (imageNodes != null)
                    {
                        product.ImageUrl = imageNodes
                            .Select(node => node.GetAttributeValue("src", string.Empty))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .ToArray();
                    }


                    //var displayTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-22']//table//tr");
                    //if (displayTableRows != null)
                    //{
                    //    product.display = displayTableRows
                    //        .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                    //        .ToArray();
                    //}
                    // Locate the rating container
                    var ratingContainer = document.DocumentNode.SelectSingleNode("//div[contains(@class,\'pda-header-rating\')]//ul[contains(@class,\'product-item-title-rating\')]//li[@class='ratingStar']");

                    if (ratingContainer != null)
                    {
                        // Count the number of full stars
                        var fullStars = ratingContainer.SelectNodes(".//i[contains(@class,\'uil-star-active\')]")?.Count ?? 0;

                        // Count the number of half stars
                        var halfStars = ratingContainer.SelectNodes(".//i[contains(@class,\'uil-star-half-alt-active\')]")?.Count ?? 0;

                        double ratingValue = fullStars + (halfStars * 0.5);

                        product.rating = ratingValue.ToString();
                    }


                    // Locate all specification nodes within the target <ul> element
                    var specificationNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\'to-expand\')]//ul[@id='article-features']/li");

                    if (specificationNodes != null)
                    {
                        foreach (var detailNode in specificationNodes)
                        {
                            var keyNode = detailNode.SelectSingleNode(".//span[contains(@class, 'list-label')]");
                            var valueNode = detailNode.SelectSingleNode(".//span[contains(@class, 'list-field')]");

                            if (keyNode != null && valueNode != null)
                            {
                                string specName = keyNode.InnerText.Trim();
                                string specValue = valueNode.InnerText.Trim();

                                // Assuming 'product.productspecification' is a dictionary
                                product.productspecification[specName] = specValue;
                            }
                        }
                    }





                    var descriptionRow = document.DocumentNode.SelectNodes("//div[@id='product-details']//ul//li");
                    if (descriptionRow != null)
                    {
                        product.description = descriptionRow
                            .Select(row => row.InnerText.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                            .ToArray();
                    }


                    var reviews = document.DocumentNode.SelectSingleNode("//*[@id=\"product-specifications-content\"]/div"); // Selects the first <font> with non-empty text
                    if (reviews != null)
                    {
                        product.reviews = reviews.InnerText.Trim();
                    }


                    var processorTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-24']//table//tr");
                    if (processorTableRows != null)
                    {
                        product.processor = processorTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Correctly assigns an array
                    }
                    // Data Carrier Section
                    var dataCarrierRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-25']//table//tr");
                    if (dataCarrierRows != null)
                    {
                        product.datacarrier = dataCarrierRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts data carrier details into an array
                    }

                    // Camera Section
                    var cameraRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-26']//table//tr");
                    if (cameraRows != null)
                    {
                        product.camera = cameraRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts camera details into an array
                    }

                    // Network Section
                    var networkRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo no') and @data-collabs='specification-section-27']//table//tr");
                    if (networkRows != null)
                    {
                        product.network = networkRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts network details into an array
                    }

                    // Ports and Interfaces Section
                    var portsAndInterfacesRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-2']//table//tr");
                    if (portsAndInterfacesRows != null)
                    {
                        product.portsandinterfaces = portsAndInterfacesRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts ports and interfaces details into an array
                    }


                    var sendingMessagesTableRows = document.DocumentNode.SelectNodes("//div[@class='oo' and @data-collabs='specification-section-46']//table//tr");
                    if (sendingMessagesTableRows != null)
                    {
                        var sendingMessagesDetails = sendingMessagesTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray();
                        //  product.sendingmessages = string.Join(", ", sendingMessagesDetails);
                    }

                    var designTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-4']//table//tr");
                    if (designTableRows != null)
                    {
                        var designDetails = designTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray();
                        // product.design = string.Join(", ", designDetails);
                    }

                    var efficiencyTableRows = document.DocumentNode.SelectNodes("//div[@class='oo no' and @data-collabs='specification-section-9']//table//tr");
                    if (efficiencyTableRows != null)
                    {
                        var efficiencyDetails = efficiencyTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray();
                        // product.Efficiency = string.Join(", ", efficiencyDetails);
                    }

                    var multimediaTableRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-30']//table//tr");
                    if (multimediaTableRows != null)
                    {
                        product.Multimedia = multimediaTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts each multimedia detail into an array
                    }

                    var softwareTableRows = document.DocumentNode.SelectNodes("//div[contains(@class,'oo') and @data-collabs='specification-section-32']//table//tr");
                    if (softwareTableRows != null)
                    {
                        product.Software = softwareTableRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts software details into an array
                    }

                    // For Weight and Dimensions section
                    var weightAndDimensionsRows = document.DocumentNode.SelectNodes(
                        "//div[contains(@class,'oo') and @data-collabs='specification-section-6']//table//tr"
                    );
                    if (weightAndDimensionsRows != null)
                    {
                        product.Weightanddimensions = weightAndDimensionsRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts weight and dimensions details into an array
                    }

                    // For Package Contents section
                    var packageContentsRows = document.DocumentNode.SelectNodes(
                        "//div[contains(@class,'oo') and @data-collabs='specification-section-15']//table//tr");
                    if (packageContentsRows != null)
                    {
                        product.PackageContents = packageContentsRows
                            .Select(row => $"{row.SelectSingleNode("./td[1]").InnerText.Trim()}: {row.SelectSingleNode("./td[2]").InnerText.Trim()}")
                            .ToArray(); // Converts package contents details into an array
                    }

                    // For Additional Information section
                    var Insurance = document.DocumentNode.SelectSingleNode("//section[contains(@id,\"section-insurances\")]"); // Selects the first <font> with non-empty text
                    if (Insurance != null)
                    {
                        product.Insurance = Insurance.InnerText.Trim(); // Extract and trim the inner text

                    }

                    var productinformation = document.DocumentNode.SelectNodes("//div[contains(@class,\"col-xs-12\")]//div[contains(@class,\"row row-top-border full-description\")]//ul[7]/li");
                    if (productinformation != null)
                    {
                        product.productinformation = productinformation
                            .Select(row => $"{row.InnerText.Trim()}")
                            .ToArray(); // Correctly assigns an array
                    }

                    var paymentmethod = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui\")]//div[6]//a//span[contains(@class,\"yl\")]"); // Selects the first <font> with non-empty text
                    if (paymentmethod != null)
                    {
                        product.paymentmethod = paymentmethod.InnerText.Trim(); // Extract and trim the inner text
                    }


                    var productdelivery = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui\")]//div[5]//a//span[contains(@class,\"yl\")]"); // Selects the first <font> with non-empty text
                    if (productdelivery != null)
                    {
                        product.productdelivery = productdelivery.InnerText.Trim(); // Extract and trim the inner text
                    }


                    var productservice = document.DocumentNode.SelectSingleNode("//section[contains(@class,\"ui zf\")]//div[contains(@class,\"hz pn\")]"); // Selects the first <font> with non-empty text
                    if (productservice != null)
                    {
                        product.productservice = productservice.InnerText.Trim(); // Extract and trim the inner text
                    }



                    //    var AdditionalInformation = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-7 description_2\")]//p//text()");
                    //    if (AdditionalInformation != null)
                    //    {
                    //        for (int i = 0; i < AdditionalInformation.Count; i += 2)
                    //        {
                    //            // Extracting the techDataName from the current node
                    //            string techDataName = (i < AdditionalInformation.Count) ? AdditionalInformation[i].InnerText.Trim() : string.Empty;

                    //            // Extracting the techDataValue from the next node
                    //            string techDataValue = (i + 1 < AdditionalInformation.Count) ? AdditionalInformation[i + 1].InnerText.Trim() : string.Empty;

                    //            // Only add if both name and value are valid
                    //            if (!string.IsNullOrWhiteSpace(techDataName) && !string.IsNullOrWhiteSpace(techDataValue))
                    //            {
                    //                product.TechnicalData[techDataName] = techDataValue;
                    //            }
                    //        }
                    //    }


                    //    foreach (var entry in product.TechnicalData)
                    //    {
                    //        // Ensure formatting aligns correctly
                    //        Console.WriteLine($"{entry.Key,-25} | {entry.Value}");
                    //    }


                    //    var highlights = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-3 highlights\")]//p");
                    //    if (highlights != null)
                    //    {
                    //        product.highlights = highlights
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //    var scopeofdelivery = document.DocumentNode.SelectNodes("//div[contains(@class,\"col data w-full text-left text-gray-75 product-attribute-value mb-3 mt-3 delivery_scope\")]//p");
                    //    if (scopeofdelivery != null)
                    //    {
                    //        product.scopeofdelivery = scopeofdelivery
                    //            .Select(row => $"{row.InnerText.Trim()}")
                    //            .ToArray(); // Correctly assigns an array
                    //    }

                    //}
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
