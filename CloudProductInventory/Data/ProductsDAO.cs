using CloudProductInventory.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace CloudProductInventory.Data
{
    public class ProductsDAO
    {
        private readonly string connectionString;
        private readonly ILogger<ProductsDAO> logger;

        public ProductsDAO(
            IConfiguration configuration,
            ILogger<ProductsDAO> logger)
        {
            this.logger = logger;

            string? jawsDbUrl =
                configuration["JAWSDB_URL"];

            if (!string.IsNullOrWhiteSpace(jawsDbUrl))
            {
                if (!Uri.TryCreate(
                    jawsDbUrl,
                    UriKind.Absolute,
                    out Uri? databaseUri))
                {
                    throw new InvalidOperationException(
                        "The JAWSDB_URL value is not valid.");
                }

                string[] userInfo =
                    databaseUri.UserInfo.Split(':', 2);

                if (userInfo.Length != 2)
                {
                    throw new InvalidOperationException(
                        "The JAWSDB_URL credentials are not valid.");
                }

                int port =
                    databaseUri.Port > 0
                    ? databaseUri.Port
                    : 3306;

                MySqlConnectionStringBuilder builder =
                    new MySqlConnectionStringBuilder
                    {
                        Server = databaseUri.Host,

                        Port = (uint)port,

                        UserID = Uri.UnescapeDataString(
                            userInfo[0]),

                        Password = Uri.UnescapeDataString(
                            userInfo[1]),

                        Database = Uri.UnescapeDataString(
                            databaseUri.AbsolutePath.TrimStart('/')),

                        SslMode = MySqlSslMode.Preferred
                    };

                connectionString = builder.ConnectionString;
            }
            else
            {
                connectionString =
                    configuration.GetConnectionString(
                        "DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "Database connection string was not found.");
            }
        }

        // READ - Get all products
        public List<Product> GetAllProducts()
        {
            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName}",
                DateTime.UtcNow.ToString("O"),
                nameof(ProductsDAO),
                nameof(GetAllProducts));

            try
            {
                List<Product> products =
                    new List<Product>();

                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql =
                        "SELECT * FROM products";

                    using (MySqlCommand cmd =
                           new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product =
                                new Product
                                {
                                    ProductID =
                                        Convert.ToInt32(
                                            reader["ProductID"]),

                                    Name =
                                        reader["Name"]
                                            .ToString() ?? "",

                                    Description =
                                        reader["Description"]
                                            .ToString() ?? "",

                                    Price =
                                        Convert.ToDecimal(
                                            reader["Price"]),

                                    Quantity =
                                        Convert.ToInt32(
                                            reader["Quantity"]),

                                    Category =
                                        reader["Category"]
                                            .ToString() ?? "",

                                    ImageURL =
                                        reader["ImageURL"]
                                            .ToString() ?? ""
                                };

                            products.Add(product);
                        }
                    }
                }

                logger.LogInformation(
                    "{Timestamp} | EXIT | {ClassName}.{MethodName} | ProductsReturned: {ProductCount}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(GetAllProducts),
                    products.Count);

                return products;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(GetAllProducts),
                    ex.Message);

                throw;
            }
        }

        // CREATE - Add one product
        public void AddProduct(Product product)
        {
            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName}",
                DateTime.UtcNow.ToString("O"),
                nameof(ProductsDAO),
                nameof(AddProduct));

            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO products
                        (
                            Name,
                            Description,
                            Price,
                            Quantity,
                            Category,
                            ImageURL
                        )
                        VALUES
                        (
                            @Name,
                            @Description,
                            @Price,
                            @Quantity,
                            @Category,
                            @ImageURL
                        )";

                    using (MySqlCommand cmd =
                           new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Name",
                            product.Name);

                        cmd.Parameters.AddWithValue(
                            "@Description",
                            product.Description);

                        cmd.Parameters.AddWithValue(
                            "@Price",
                            product.Price);

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            product.Quantity);

                        cmd.Parameters.AddWithValue(
                            "@Category",
                            product.Category);

                        cmd.Parameters.AddWithValue(
                            "@ImageURL",
                            product.ImageURL ?? "");

                        int rowsAffected =
                            cmd.ExecuteNonQuery();

                        logger.LogInformation(
                            "{Timestamp} | EXIT | {ClassName}.{MethodName} | RowsAffected: {RowsAffected}",
                            DateTime.UtcNow.ToString("O"),
                            nameof(ProductsDAO),
                            nameof(AddProduct),
                            rowsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(AddProduct),
                    ex.Message);

                throw;
            }
        }

        // READ ONE - Get one product for Edit or Delete
        public Product? GetProductById(int id)
        {
            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName} | ProductID: {ProductID}",
                DateTime.UtcNow.ToString("O"),
                nameof(ProductsDAO),
                nameof(GetProductById),
                id);

            try
            {
                Product? product = null;

                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT *
                        FROM products
                        WHERE ProductID = @ProductID";

                    using (MySqlCommand cmd =
                           new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            id);

                        using (MySqlDataReader reader =
                               cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product =
                                    new Product
                                    {
                                        ProductID =
                                            Convert.ToInt32(
                                                reader["ProductID"]),

                                        Name =
                                            reader["Name"]
                                                .ToString() ?? "",

                                        Description =
                                            reader["Description"]
                                                .ToString() ?? "",

                                        Price =
                                            Convert.ToDecimal(
                                                reader["Price"]),

                                        Quantity =
                                            Convert.ToInt32(
                                                reader["Quantity"]),

                                        Category =
                                            reader["Category"]
                                                .ToString() ?? "",

                                        ImageURL =
                                            reader["ImageURL"]
                                                .ToString() ?? ""
                                    };
                            }
                        }
                    }
                }

                logger.LogInformation(
                    "{Timestamp} | EXIT | {ClassName}.{MethodName} | ProductID: {ProductID} | Found: {ProductFound}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(GetProductById),
                    id,
                    product != null);

                return product;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | ProductID: {ProductID} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(GetProductById),
                    id,
                    ex.Message);

                throw;
            }
        }

        // UPDATE - Save changes to one product
        public void UpdateProduct(Product product)
        {
            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName} | ProductID: {ProductID}",
                DateTime.UtcNow.ToString("O"),
                nameof(ProductsDAO),
                nameof(UpdateProduct),
                product.ProductID);

            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE products
                        SET
                            Name = @Name,
                            Description = @Description,
                            Price = @Price,
                            Quantity = @Quantity,
                            Category = @Category,
                            ImageURL = @ImageURL
                        WHERE ProductID = @ProductID";

                    using (MySqlCommand cmd =
                           new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            product.ProductID);

                        cmd.Parameters.AddWithValue(
                            "@Name",
                            product.Name);

                        cmd.Parameters.AddWithValue(
                            "@Description",
                            product.Description);

                        cmd.Parameters.AddWithValue(
                            "@Price",
                            product.Price);

                        cmd.Parameters.AddWithValue(
                            "@Quantity",
                            product.Quantity);

                        cmd.Parameters.AddWithValue(
                            "@Category",
                            product.Category);

                        cmd.Parameters.AddWithValue(
                            "@ImageURL",
                            product.ImageURL ?? "");

                        int rowsAffected =
                            cmd.ExecuteNonQuery();

                        logger.LogInformation(
                            "{Timestamp} | EXIT | {ClassName}.{MethodName} | ProductID: {ProductID} | RowsAffected: {RowsAffected}",
                            DateTime.UtcNow.ToString("O"),
                            nameof(ProductsDAO),
                            nameof(UpdateProduct),
                            product.ProductID,
                            rowsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | ProductID: {ProductID} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(UpdateProduct),
                    product.ProductID,
                    ex.Message);

                throw;
            }
        }

        // DELETE - Remove one product
        public void DeleteProduct(int id)
        {
            logger.LogInformation(
                "{Timestamp} | ENTRY | {ClassName}.{MethodName} | ProductID: {ProductID}",
                DateTime.UtcNow.ToString("O"),
                nameof(ProductsDAO),
                nameof(DeleteProduct),
                id);

            try
            {
                using (MySqlConnection conn =
                       new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        DELETE FROM products
                        WHERE ProductID = @ProductID";

                    using (MySqlCommand cmd =
                           new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@ProductID",
                            id);

                        int rowsAffected =
                            cmd.ExecuteNonQuery();

                        logger.LogInformation(
                            "{Timestamp} | EXIT | {ClassName}.{MethodName} | ProductID: {ProductID} | RowsAffected: {RowsAffected}",
                            DateTime.UtcNow.ToString("O"),
                            nameof(ProductsDAO),
                            nameof(DeleteProduct),
                            id,
                            rowsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Timestamp} | EXCEPTION | {ClassName}.{MethodName} | ProductID: {ProductID} | {ErrorMessage}",
                    DateTime.UtcNow.ToString("O"),
                    nameof(ProductsDAO),
                    nameof(DeleteProduct),
                    id,
                    ex.Message);

                throw;
            }
        }
    }
}