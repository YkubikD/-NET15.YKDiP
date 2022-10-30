using main.Pages.List;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace main.Pages.Lists
{
    public class CreateModel : PageModel
    {
        public ListInfo listInfo = new();


        public List<ProductsInfo> ProductsList = new();
        public void OnGet()
        {
        }

        public void OnPost(IFormFile file)
        {
            var filePath = UploadFile(file);

            var fileName = file.FileName;



            String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=admin_csv;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using SqlConnection connection = new(connectionString);// соединен
            connection.Open();
            String sql = "INSERT INTO lists (name) VALUES (@name); SELECT SCOPE_IDENTITY()";
            using SqlCommand command = new(sql, connection);// строк  баз
            command.Parameters.AddWithValue("@name", fileName);
            var modified = command.ExecuteScalar();
            command.Parameters.Clear();

            int i = 0;

            using (StreamReader reader = new (filePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }

                    var array = line.Split(';');
                    String sqlProduct = "INSERT INTO products (lists_id, name, price, count) VALUES (@lists_id, @name, @price, @count);";
                    using SqlCommand commandProduct = new(sqlProduct, connection);
                    commandProduct.Parameters.AddWithValue("@lists_id", modified.ToString());
                    commandProduct.Parameters.AddWithValue("@name", array[0]);
                    commandProduct.Parameters.AddWithValue("@price", array[1]);
                    commandProduct.Parameters.AddWithValue("@count", array[2]);
                    commandProduct.ExecuteNonQuery();
                    commandProduct.Parameters.Clear();
                }
            }


            Response.Redirect("/Lists/Index");
        }

        
        public string UploadFile(IFormFile file)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "csv", file.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return filePath;
        }
        

    }
}
