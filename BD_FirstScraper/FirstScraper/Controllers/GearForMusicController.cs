using FirstScraper.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GearForMusicController : ControllerBase
    {
        private readonly ILogger<GearForMusicController> _logger;
        public GearForMusicController(ILogger<GearForMusicController> logger)
        {
            _logger = logger;
        }

        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeProduct([FromQuery] string url = "https://www.gear4music.com/Keyboards-and-Pianos/Roland-GOKEYS-3-Music-Creation-Keyboard-Dark-Red/6AB4")
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

                    var productcategory = document.DocumentNode.SelectNodes("//div[contains(@class,\"row\")]//nav[contains(@id,\"inv-breadcrumb\")]/ol/li//span");
                    if (productcategory != null)
                    {
                        product.productcategory = productcategory
                            .Select(row => $"{row.InnerText.Trim()}")
                            .ToArray();
                    }

                    var brandNode = document.DocumentNode.SelectSingleNode("//*[@id=\"main_area\"]/div[4]/div/div[2]/ul/li[6]/a/span/font/font");
                    if (brandNode != null)
                    {
                        product.Brandofproduct = brandNode.InnerText.Trim();
                    }

                    var nameNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"pdp-title\")]/h1");
                    if (nameNode != null)
                    {
                        product.Name = nameNode.InnerText.Trim();
                    }

                    var modelNode = document.DocumentNode.SelectSingleNode("//*[@id=\"productModelValue\"]");
                    if (modelNode != null)
                    {
                        product.Model = modelNode.InnerText.Trim();
                    }

                    var mrp = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"info-row-item info-row-pricing\")]/span/span[2]");
                    if (mrp != null)
                    {
                        product.mrp = mrp.InnerText.Trim();
                    }
                    var Currency = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"info-row-item info-row-pricing\")]/span/span[1]");
                    if (Currency != null)
                    {
                        product.Currency = Currency.InnerText.Trim();
                    }

                    var deliveryNode = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"info-row-delivery-msg info-row-item\")]//div[contains(@class,\"close\")]");
                    if (deliveryNode != null)
                    {
                        product.Delivery = deliveryNode.InnerText.Trim();
                    }

                    //var pricediscount = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"sa\")]");
                    //if (pricediscount != null)
                    //{
                    //    product.pricediscount = pricediscount.InnerText.Trim();
                    //}
                    var imageNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\"switcher hide-mobile images\")]//ul//li//img");
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
                    var rating = document.DocumentNode.SelectSingleNode("//div[contains(@class,\"offer-selector__description-header\")]//div[contains(@class,\"rating-value__wrapper\")]//span[1]");
                    if (rating != null)
                    {
                        product.rating = rating.InnerText.Trim();
                    }

                    // Locate all specification nodes within the target <ul> element
                    var specificationNodes = document.DocumentNode.SelectNodes("//div[contains(@class,\'col-xs-12\')]//div[contains(@class,\'row row-top-border full-description\')]//ul[1]/li");

                    if (specificationNodes != null)
                    {
                        foreach (var detailNode in specificationNodes)
                        {
                            // Select the <strong> node (for the key) and its sibling text node (for the value)
                            var keyNode = detailNode.SelectSingleNode(".//strong");
                            var valueNode = keyNode?.NextSibling?.InnerText?.Trim();

                            if (keyNode != null && !string.IsNullOrEmpty(valueNode))
                            {
                                // Extract the key text, trim colon and whitespace
                                string specName = keyNode.InnerText.Trim(':').Trim();
                                string specValue = valueNode;

                                // Store the specification data in the product object
                                product.productspecification[specName] = specValue;
                            }
                        }
                    }




                    var descriptionRow = document.DocumentNode.SelectNodes("//div[contains(@class,\"slide\")]//h3[1]|//div[contains(@class,\"slide\")]/p");
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
