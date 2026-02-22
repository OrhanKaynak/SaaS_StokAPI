using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SaaS_StokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StockController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks()
        {
            var realStocks = new List<Product>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "SELECT Id, Name, StockQuantity, Price FROM Stocks";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
        public async Task<IActionResult> AddStock([FromBody] Product newProduct)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO Stocks (Name, StockQuantity, Price) VALUES (@Name, @Quantity, @Price)";

                using (SqlCommand command = new SqlCommand(@sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newProduct.Name);
                    command.Parameters.AddWithValue("Quantity", newProduct.StockQuantity);
                    command.Parameters.AddWithValue("Price", newProduct.Price);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok("Product Added");
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> UpdadteStock(int id, [FromBody] Product updatedProduct)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "UPDATE Stocks SET Name = @Name, StockQuantity = @Quantity, Price = @Price WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(@sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", updatedProduct.Name);
                    command.Parameters.AddWithValue("@Quantity", updatedProduct.StockQuantity);
                    command.Parameters.AddWithValue("@Price", updatedProduct.Price);

                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows == 0)
                    {
                        return NotFound("There is no product with this mussel in the stocks");
                    }
                }
            }
            return Ok("Product successfuly updated");
        }

        [HttpDelete ("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "DELETE FROM Stocks WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if(affectedRows == 0)
                    {
                        return NotFound("Product couldn't found.");
                    }
                }
            }
            return Ok("Produvt succsessfully deleted.");
        }
    }
}
