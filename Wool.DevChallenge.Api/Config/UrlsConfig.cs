namespace Wool.DevChallenge.Api.Config
{
    public class UrlsConfig
    {
        public class ProductsOperations
        {
            public static string GetProducts(string token) => $"/api/resource/products?token={token}";

            public static string GetShopperHistory(string token) => $"/api/resource/shopperHistory?token={token}";
        }

        public class TrolleyOperations
        {
            public static string CalculateTrolleyTotal(string token) => $"/api/resource/trolleyCalculator?token={token}";
        }
    }
}
