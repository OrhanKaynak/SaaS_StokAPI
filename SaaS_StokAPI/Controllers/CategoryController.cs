using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SaaS_StokAPI.Models;

namespace SaaS_StokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = new List<Category>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "SELECT Id, CategoryName, Description FROM Categories";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var category = new Category
                            {
                                Id = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                Description = reader.GetString(2)
                            };
                            categories.Add(category);
                        }
                    }
                }
            }
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category newCategory)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlquery = "INSERT INTO Categories (CategoryName, Description) VALUES (@Name, @Desc)";

                using (SqlCommand command = new SqlCommand(sqlquery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newCategory.CategoryName);
                    command.Parameters.AddWithValue("@Desc", newCategory.Description);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok("Category Added");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "UPDATE Categories SET CategoryName = @Name, Description = @Desc WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", updatedCategory.CategoryName);
                    command.Parameters.AddWithValue("@Desc", updatedCategory.Description);

                    var affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows == 0)
                    {
                        return NotFound("Category Couldn't Found");
                    }
                }
            }
            return Ok("Category Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlQuery = "DELETE FROM Categories WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    var affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows == 0)
                    {
                        return NotFound("Category Couldn't Found");
                    }
                }
            }
            return Ok("Category Deleted");
        }
    }
}
