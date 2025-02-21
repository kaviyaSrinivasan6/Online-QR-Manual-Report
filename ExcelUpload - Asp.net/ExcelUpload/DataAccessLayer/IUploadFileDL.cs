using ExcelUpload.CommonLayer.Model;

namespace ExcelUpload.DataAccessLayer
{
    public interface IUploadFileDL
    {
       public Task<UploadExcelFileResponse> UploadExcelFile(UploadExcelFileRequest request, string Path);

        public Task<ReadRecordResponse> ReadRecord();
    }
}
