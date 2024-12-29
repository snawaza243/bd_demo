namespace FirstScraper.Models
{
    public class ProductDetails
    {
        public string Name { get; set; }
        
        public string Model { get; set; }
        public string Delivery {  get; set; }

        public string EAN { get; set; }

        public string[] productcategory { get; set; }


        public string mrp { get; set; }
        public string Currency {  get; set; }

        public string priceafterusingcoupons { get; set; }

        public string sellingprice { get; set; }

        public string productdetails { get; set; }

        public string pricediscount { get; set; }
        public string productalertprice { get; set; }

        public string productalertstock { get; set; }
        public string[] ImageUrl { get; set; }

        public string[] ColourAvailibility { get; set; }

        public string[] description { get; set; }

        public string rating { get; set; }

        public string partnumber { get; set; }

        public string Brandofproduct { get; set; }

        public string manufacturingnumber { get; set; }

        public string ShippingWeight { get; set; }



        public string reviews { get; set; }

        public string sku { get; set; }

        public string referenceid { get; set; }

        public string deliveryoffer { get; set; }
        public string[] display { get; set; }

        public string[] processor { get; set; }

        public string[] datacarrier { get; set; }

        public string pricetax { get; set; }

        public string[] camera { get; set; }

        public string[] network { get; set; }

        public string[] portsandinterfaces { get; set; }

        public string[] sendingmessages { get; set; }

        public string[] design { get; set; }

        public string[] Efficiency { get; set; }

        public string[] Multimedia { get; set; }

        public string[] Software { get; set; }

        public string[] Weightanddimensions { get; set; }

        public string[] PackageContents { get; set; }

        public string[] AdditionalInformation { get; set; }

        public string Insurance { get; set; }

        public string paymentmethod { get; set; }

        public string productdelivery { get; set; }

        public string productservice { get; set; }

        public string keypoints { get; set; }

        public string Availibility { get; set; }

        public string[] productinformation { get; set; }

        public string[] highlights { get; set; }
        public string[] scopeofdelivery { get; set; }

        public string[] Feature { get; set; }

        public string[] Parameter { get; set; }

        public string[] shippingestimate { get; set; }



        public Dictionary<string, string> TechnicalData { get; set; }
        public Dictionary<string, string> ExtraDetails { get; set; }
        public Dictionary<string, string> productspecification { get; set; }
    }
}
