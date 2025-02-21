namespace ExcelUpload.CommonLayer.Model
{
    public class ReadRecordRequest
    {
    }
    public class ReadRecordResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public List<ReadRecord> data { get; set; }

        public class ReadRecord
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
}
