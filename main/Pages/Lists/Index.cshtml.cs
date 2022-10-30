using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace main.Pages.List
{
    public class IndexModel : PageModel
    {
        public List<ListInfo> ListLists = new ();
        public void OnGet()
        {
            String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=admin_csv;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using SqlConnection connection = new(connectionString);
            connection.Open();
            String sql = "SELECT * FROM lists";
            SqlCommand cmd = new(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ListInfo listInfo = new ();
                listInfo.id = "" + reader.GetInt32(0);
                listInfo.name = reader.GetString(1);
                listInfo.created_at = reader.GetDateTime(2).ToString();

                ListLists.Add(listInfo);

            }
        }
    }

    public class ListInfo
    {
        public String? id;
        public String? name;
        public String? created_at;
    }

    public class ProductsInfo
    {
        public String? id;
        public String? lists_id;
        public String? name;
        public String? price;
        public String? count;
    }
}
