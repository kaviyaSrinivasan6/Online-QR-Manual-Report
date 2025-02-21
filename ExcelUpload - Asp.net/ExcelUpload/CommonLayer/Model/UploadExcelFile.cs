using System.Net;

namespace ExcelUpload.CommonLayer.Model
{
    public class UploadExcelFileRequest
    {

        public IFormFile File { get; set; }
    }
    public class UploadExcelFileResponse
    {

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public int DuplicateCount { get; set; }

        public int TotalCount { get; set; } 

        public int TotalRecordCount {  get; set; }

    }
    public class ExcelBulkUploadParameter
    {
        public int SNo { get; set; }
        public string DateOfPurchase { get; set; }
        public string TicketType { get; set; }
        public string TxnNo { get; set; }

        public int TicketAmount { get; set; }

        public string TicketStatus { get; set; }

        public string TicketId { get; set; }

        public int NoOfTickets { get; set; }

        public string RefundInitiatedDate { get; set; }
        public string ReasonForRefund { get; set; }
        public int RefundInitiatedAmount { get; set; }

    }
}