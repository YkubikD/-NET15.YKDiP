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
            SqlDataReader reader = command.ExecuteReader(); // чмит бзд
            using (StreamWriter writerFile = new(filePath)) // zap file
            {
                writerFile.WriteLine("Lists_id;Name;Price;Count");  // добав 1 стр
                while (reader.Read())
                {
                    ProductsInfo Products = new();      // доба продукт
                    Products.lists_id = "" + reader.GetInt32(1);
                    Products.name = reader.GetString(2);
                    Products.price = reader.GetString(3);
                    Products.count = reader.GetString(4);
                    writerFile.WriteLine(Products.lists_id + ";" + Products.name + ";" + Products.price + ";" + Products.count);
                }
                writerFile.Close();
            }

            var di = new DirectoryInfo(Directory.GetCurrentDirectory() + "/csv"); // показ пути для скачкм

            ListFiles = di.GetFiles().ToList();   //фал лист адд
        }

        public async Task<IActionResult> OnPost(string file, string mode)  // прин название файла и мод
        {
            FileInfo fi = new(Directory.GetCurrentDirectory() + "/csv/" + file);

            // Получаем более актуальную информацию о файле.проверка
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
                // Обработка ситуации отсутствия файла.
            }


            return new EmptyResult();
        }
        private async Task<IActionResult> BytesSend(FileInfo fi)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(fi.FullName);

            var contentType = "text/csv";


            // 1 способ
            return File(bytes, contentType, fi.Name);

            // 2 способ
            //return new FileContentResult(bytes, file_type)
            //{
            //    FileDownloadName = fi.Name
            //};
        }



        private async Task<IActionResult> StreamSend(FileInfo fi)
        {
            return await Task.Run(() =>
            {

                // Открываем поток.
                var stream = fi.OpenRead(); //System.IO.File.OpenRead(path);

                var contentType = "text/csv";


                // 1 способ
                return File(stream, contentType, fi.Name);

                // 2 способ
                //return new FileStreamResult(stream, contentType)
                //{
                //    FileDownloadName = fi.Name
                //};
            });
        }



        private async Task<IActionResult> MemorySend(FileInfo fi)
        {
            var buffer = new MemoryStream();  // выдел памяти 
            using var stream = fi.OpenRead();
            await stream.CopyToAsync(buffer);
            buffer.Position = 0;   // перенос в нач файл

            var contentType = "text/csv";  // определен тип файла

            // 1 способ
            //return File(buffer, contentType, fi.Name);

            // 2 способ
            return new FileStreamResult(buffer, contentType)
            {
                FileDownloadName = fi.Name
            };
        }
    }
}
