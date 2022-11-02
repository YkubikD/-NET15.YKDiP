using main.Pages.List;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace main.Pages.Lists
{
    public class ExportModel : PageModel
    {
        public List<FileInfo> ListFiles { get; set; }

        public void OnGet()
        {


            string stringid = Request.Query["ids"];

            //var stringid = string.Join(", ", ids);
            String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=admin_csv;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using SqlConnection connection = new(connectionString);
            connection.Open();
            var sql = "SELECT * FROM products WHERE lists_id IN (" + stringid + ")";

            var filePath = Directory.GetCurrentDirectory() + "/csv/export.csv";

            using SqlCommand command = new(sql, connection);
            SqlDataReader reader = command.ExecuteReader(); // ���� ���
            using (StreamWriter writerFile = new(filePath)) // zap file
            {
                writerFile.WriteLine("Lists_id;Name;Price;Count");  // ����� 1 ���
                while (reader.Read())
                {
                    ProductsInfo Products = new();      // ���� �������
                    Products.lists_id = "" + reader.GetInt32(1);
                    Products.name = reader.GetString(2);
                    Products.price = reader.GetString(3);
                    Products.count = reader.GetString(4);
                    writerFile.WriteLine(Products.lists_id + ";" + Products.name + ";" + Products.price + ";" + Products.count);
                }
                writerFile.Close();
            }

            var di = new DirectoryInfo(Directory.GetCurrentDirectory() + "/csv"); // ����� ���� ��� ������

            ListFiles = di.GetFiles().ToList();   //��� ���� ���
        }

        public async Task<IActionResult> OnPost(string file, string mode)  // ���� �������� ����� � ���
        {
            FileInfo fi = new(Directory.GetCurrentDirectory() + "/csv/" + file);

            // �������� ����� ���������� ���������� � �����.��������
            fi.Refresh();

            if (fi.Exists == true)
            {

                return mode switch
                {
                    "memory" => await MemorySend(fi),
                    "stream" => await StreamSend(fi),
                    "bytes" => await BytesSend(fi),
                    _ => new EmptyResult(),
                };
            }
            else
            {
                // ��������� �������� ���������� �����.
            }


            return new EmptyResult();
        }
        private async Task<IActionResult> BytesSend(FileInfo fi)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(fi.FullName);

            var contentType = "text/csv";


            // 1 ������
            return File(bytes, contentType, fi.Name);

            // 2 ������
            //return new FileContentResult(bytes, file_type)
            //{
            //    FileDownloadName = fi.Name
            //};
        }



        private async Task<IActionResult> StreamSend(FileInfo fi)
        {
            return await Task.Run(() =>
            {

                // ��������� �����.
                var stream = fi.OpenRead(); //System.IO.File.OpenRead(path);

                var contentType = "text/csv";


                // 1 ������
                return File(stream, contentType, fi.Name);

                // 2 ������
                //return new FileStreamResult(stream, contentType)
                //{
                //    FileDownloadName = fi.Name
                //};
            });
        }



        private async Task<IActionResult> MemorySend(FileInfo fi)
        {
            var buffer = new MemoryStream();  // ����� ������ 
            using var stream = fi.OpenRead();
            await stream.CopyToAsync(buffer);
            buffer.Position = 0;   // ������� � ��� ����

            var contentType = "text/csv";  // ��������� ��� �����

            // 1 ������
            //return File(buffer, contentType, fi.Name);

            // 2 ������
            return new FileStreamResult(buffer, contentType)
            {
                FileDownloadName = fi.Name
            };
        }
    }
}
