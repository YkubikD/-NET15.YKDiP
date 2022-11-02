using main.Pages.List;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace main.Pages.Lists
{
    public class ShowModel : PageModel
    {
        public ProductsInfo ProductsInfo = new();


        public List<ProductsInfo> ProductsList = new();

        public void OnGet()
        {
            String id = Request.Query["id"];  //брать ид браузера

            String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=admin_csv;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";   //путь к SQL
            using SqlConnection connection = new(connectionString);    
            connection.Open();


            SqlCommand commandProduct = new SqlCommand("SELECT * FROM products WHERE lists_id = @id", connection);   //SQL 
            commandProduct.Parameters.AddWithValue("@id", id);
            SqlDataReader readerProduct = commandProduct.ExecuteReader();

            while (readerProduct.Read())
            {
                ProductsInfo ProductsInfo = new ProductsInfo();
                ProductsInfo.id = "" + readerProduct.GetInt32(0);
                ProductsInfo.lists_id = "" + readerProduct.GetInt32(1);
                ProductsInfo.name = readerProduct.GetString(2);
                ProductsInfo.price = readerProduct.GetString(3);
                ProductsInfo.count= readerProduct.GetString(4);
                ProductsList.Add(ProductsInfo);
            }
        }
    }
}
