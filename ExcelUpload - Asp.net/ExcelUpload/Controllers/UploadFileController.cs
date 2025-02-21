using ExcelUpload.CommonLayer.Model;
using ExcelUpload.DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {

        public readonly IUploadFileDL _uploadFileDL;
        public UploadFileController(IUploadFileDL uploadFileDL)
        {

            _uploadFileDL = uploadFileDL;
        }

        

        [HttpGet]
        public async Task<IActionResult> ReadRecord()
        {
            ReadRecordResponse response = new ReadRecordResponse();

            try
            {

                response = await _uploadFileDL.ReadRecord();

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadExcelFileRequest request)
        {

            UploadExcelFileResponse response = new UploadExcelFileResponse();
            string Path = "UploadFileFolder/" + request.File.FileName;
            try
            {
                using (FileStream stream = new FileStream(Path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }
                response = await _uploadFileDL.UploadExcelFile(request, Path);

                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);

                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

    }
}
