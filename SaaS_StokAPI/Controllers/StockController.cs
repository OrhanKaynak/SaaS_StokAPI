using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SaaS_StokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllStocks()
        {
            var realStocks = new List<Product>();

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=SaasDb;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlQuery = "SELECT Id, Name, StockQuantity, Price FROM Stocks";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                StockQuantity = reader.GetInt32(2),
                                Price = reader.GetDecimal(3),
                            };
                            realStocks.Add(product);
                        }
                    }
                } 
            }
            return Ok(realStocks);
        }

        [HttpPost]
        public IActionResult AddStock([FromBody] Product newProduct)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=SaasDb;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlQuery = "INSERT INTO Stocks (Name, StockQuantity, Price) VALUES (@Name, @Quantity, @Price)";

                using (SqlCommand command = new SqlCommand(@sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newProduct.Name);
                    command.Parameters.AddWithValue("Quantity", newProduct.StockQuantity);
                    command.Parameters.AddWithValue("Price", newProduct.Price);

                    command.ExecuteNonQuery();
                }
            }
            return Ok("Product Added");
        }
    }
}
