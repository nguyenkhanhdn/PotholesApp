using lobe;
using lobe.ImageSharp;
using Microsoft.AspNetCore.Mvc;
using PotholesApp.Models;
using SixLabors.ImageSharp.PixelFormats;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PotholesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Cam()
        {
            return View();
        }
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string map, IFormFile fileUpload)
        {
            try
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileUpload.FileName;
                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                string relativeImg2 = Path.Combine("uploads", fileName);

                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    fileUpload.CopyTo(stream);
                }
                var result = Predict(SavePath);

                if (result == "huhai")
                {
                    var data = map.Split(';');
                    var lati = data[0];
                    var lng = data[1];
                    var loc = String.Format("https://maps.google.com/?q={0},{1}", lati, lng);

                    AddPothole(lati, lng, loc, relativeImg2);
                }
                ViewBag.Result = result;
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        [HttpPost]
        public string UploadWebCamImage(string mydata)//Binary -> nội dung của bức ảnh
        {
            string[] dat = mydata.Split(';');
            //Full path of image

            var fileName = "hole" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "") + ".png";
            //Get url To Save
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            string relativeImg2 = Path.Combine("uploads", fileName);

            using (FileStream fs = new FileStream(SavePath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(dat[0]);
                    bw.Write(data);//Ghi xuống đĩa, lưu trong thư mục UploadedFiles
                    bw.Close(); //Mở file thì có đóng file -> lỗi
                }
            }
            //Sau khi upload gọi hàm nhận diện 
            string predictResult = Predict(SavePath);
            if (predictResult == "huhai")
            {
                var lati = dat[1];
                var lng = dat[2];
                var loc = String.Format("https://maps.google.com/?q={0},{1}", lati, lng);
                AddPothole(lati, lng, loc, relativeImg2);
            }
            return predictResult; //Hàm trả về là nhãn: huhai, xuocnhe, binh thuong
        }
        //private string conString = "Server=tcp:khanhn.database.windows.net,1433;Initial Catalog=ehandbook;Persist Security Info=False;User ID=khanhn;Password= Abc123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        string conString = @"server=(local)\SQLExpress;Database=PotholeDetectorApp;uid=sa;pwd=123456";
        public string AddPothole(
        string latitude,
        string longitude,
        string location,
        string photo)
        {
            string msg = "OK";
            try
            {
                //string connectionString = @"server=(local)\SQLExpress;Database=PotholeDetectorApp;uid=sa;pwd=123456";
                SqlConnection con = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand("addhole", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lat", latitude);
                cmd.Parameters.AddWithValue("@long", longitude);
                cmd.Parameters.AddWithValue("@loc", location);
                cmd.Parameters.AddWithValue("@img", photo);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                msg = "Error";
            }
            return msg;
        }
        private string Predict(string fileName)
        {
            try
            {
                string signatureFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/model/signature.json");
                var imageToClassify = fileName;//File ảnh cần chẩn đoán

                lobe.ImageClassifier.Register("onnx", () => new OnnxImageClassifier());
                var classifier = ImageClassifier.CreateFromSignatureFile(new FileInfo(signatureFilePath));

                //Phân loại ảnh  
                var results = classifier.Classify(SixLabors.ImageSharp.Image.Load(imageToClassify).CloneAs<Rgb24>());
                return results.Prediction.Label;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Problem()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Report()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Report(string map, IFormFile fileUpload)
        {
            try
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileUpload.FileName;
                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                string relativeImg2 = Path.Combine("uploads", fileName);

                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    fileUpload.CopyTo(stream);
                }
                var data = map.Split(';');
                var lati = data[0];
                var lng = data[1];
                var loc = String.Format("https://maps.google.com/?q={0},{1}", lati, lng);

                AddPothole(lati, lng, loc, relativeImg2);
                
                ViewBag.Result = "Báo cáo thành công, cảm ơn bạn đã dành thời gian!";
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}