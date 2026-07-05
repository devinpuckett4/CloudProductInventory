using CloudProductInventory.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CloudProductInventory.Data
{
    public class ProductsDAO
    {
        private readonly string connectionString;

        public ProductsDAO(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string was not found.");
        }

        // READ - Get all products
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT * FROM Products";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductID = Convert.ToInt32(reader["ProductID"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToDecimal(reader["Price"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Category = reader["Category"].ToString() ?? "",
                            ImageURL = reader["ImageURL"].ToString() ?? ""
                        };

                        products.Add(product);
                    }
                }
            }

            return products;
        }

        // CREATE - Add one product
        public void AddProduct(Product product)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"INSERT INTO Products
                               (Name, Description, Price, Quantity, Category, ImageURL)
                               VALUES
                               (@Name, @Description, @Price, @Quantity, @Category, @ImageURL)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ImageURL", product.ImageURL ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // READ ONE - Get one product for Edit or Delete
        public Product? GetProductById(int id)
        {
            Product? product = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT * FROM Products WHERE ProductID = @ProductID";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["Name"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Category = reader["Category"].ToString() ?? "",
                                ImageURL = reader["ImageURL"].ToString() ?? ""
                            };
                        }
                    }
                }
            }

            return product;
        }

        // UPDATE - Save changes to one product
        public void UpdateProduct(Product product)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"UPDATE Products
                               SET Name = @Name,
                                   Description = @Description,
                                   Price = @Price,
                                   Quantity = @Quantity,
                                   Category = @Category,
                                   ImageURL = @ImageURL
                               WHERE ProductID = @ProductID";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ImageURL", product.ImageURL ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE - Remove one product
        public void DeleteProduct(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = "DELETE FROM Products WHERE ProductID = @ProductID";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}