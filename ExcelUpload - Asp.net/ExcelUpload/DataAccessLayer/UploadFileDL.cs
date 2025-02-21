using ExcelDataReader;
using ExcelUpload.CommonLayer.Model;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using static ExcelUpload.CommonLayer.Model.ReadRecordResponse;

namespace ExcelUpload.DataAccessLayer
{
    public class UploadFileDL : IUploadFileDL
    {

        public readonly IConfiguration _Configuration;
        public readonly MySqlConnection _mySqlConnection;


        public UploadFileDL(IConfiguration Configuration)
        {
            _Configuration = Configuration;
            _mySqlConnection = new MySqlConnection(_Configuration["ConnectionStrings:MySQLString"]);
        }

        public async Task<ReadRecordResponse> ReadRecord()
        {
            ReadRecordResponse response = new ReadRecordResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if(_mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"SELECT * FROM refundsDB.refundData";


                using (MySqlCommand sqlcommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlcommand.CommandType = CommandType.Text;
                    sqlcommand.CommandTimeout = 180;

                    using (DbDataReader sqlDataReader = await sqlcommand.ExecuteReaderAsync())
                    {
                        if (sqlDataReader.HasRows)
                        {
                            response.data = new List<ReadRecord>();

                            while (await sqlDataReader.ReadAsync())
                            {

                                ReadRecord getdata = new ReadRecord();
                                getdata.SNo = sqlDataReader["sNo"] != DBNull.Value ? Convert.ToInt32(sqlDataReader["SNo"]) : -1;
                                getdata.DateOfPurchase = sqlDataReader["dateOfPurchase"] != DBNull.Value ? Convert.ToString(sqlDataReader["dateOfPurchase"]) : "-1";
                                getdata.TicketType = sqlDataReader["ticketType"] != DBNull.Value ? Convert.ToString(sqlDataReader["ticketType"]) : "-1";
                                getdata.TxnNo = sqlDataReader["txnNo"] != DBNull.Value ? Convert.ToString(sqlDataReader["txnNo"]) : "-1";
                                getdata.TicketAmount = sqlDataReader["ticketAmount"] != DBNull.Value ? Convert.ToInt32(sqlDataReader["ticketAmount"]) :-1 ;
                                getdata.TicketStatus = sqlDataReader["ticketStatus"] != DBNull.Value ? Convert.ToString(sqlDataReader["ticketStatus"]) : "-1";
                                getdata.TicketId  = sqlDataReader["ticketId"] != DBNull.Value ? Convert.ToString(sqlDataReader["ticketId"]) : "-1";
                                getdata.NoOfTickets = sqlDataReader["noOfTickets"] != DBNull.Value ? Convert.ToInt32(sqlDataReader["noOfTickets"]) : -1;
                                getdata.RefundInitiatedDate = sqlDataReader["refundInitiatedDate"] != DBNull.Value ? Convert.ToString(sqlDataReader["refundInitiatedDate"]) : "-1";
                                getdata.ReasonForRefund = sqlDataReader["reasonForRefund"] != DBNull.Value ? Convert.ToString(sqlDataReader["reasonForRefund"]) : "-1";
                                getdata.RefundInitiatedAmount = sqlDataReader["refundInitiatedAmount"] != DBNull.Value ? Convert.ToInt32(sqlDataReader["refundInitiatedAmount"]) : -1;
                                response.data.Add(getdata);
                            }
                        }
                        else
                        {
                            response.Message = "Record Not Found";
                        }
                    }
                }




            }
            catch (Exception ex) { 

                    response.IsSuccess = false;
                    response.Message = ex.Message;

            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UploadExcelFileResponse> UploadExcelFile(UploadExcelFileRequest request, string Path)
        {
            UploadExcelFileResponse response = new UploadExcelFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();

            response.IsSuccess = true;
            response.Message = "Successful";
            response.DuplicateCount = 0;
            response.TotalCount = 0;
            response.TotalRecordCount = 0;

            try
            {
                if (_mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                if (request.File.FileName.ToLower().Contains(".xlsx"))
                {
                    FileStream stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    DataSet dataset = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true,
                            }
                        }
                    );

                    response.TotalRecordCount = dataset.Tables[0].Rows.Count;
                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();
                        rows.SNo = dataset.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToInt32(dataset.Tables[0].Rows[i].ItemArray[0]) : -1;
                        rows.DateOfPurchase = dataset.Tables[0].Rows[i].ItemArray[1] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[1]) : "-1";
                        rows.TicketType = dataset.Tables[0].Rows[i].ItemArray[2] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[2]) : "-1";
                        rows.TxnNo = dataset.Tables[0].Rows[i].ItemArray[3] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[3]) : "-1";
                        rows.TicketAmount = dataset.Tables[0].Rows[i].ItemArray[4] != null ? Convert.ToInt32(dataset.Tables[0].Rows[i].ItemArray[4]) : -1;
                        rows.TicketStatus = dataset.Tables[0].Rows[i].ItemArray[5] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[5]) : "-1";
                        rows.TicketId = dataset.Tables[0].Rows[i].ItemArray[6] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[6]) : "-1";
                        rows.NoOfTickets = dataset.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToInt32(dataset.Tables[0].Rows[i].ItemArray[7]) : -1;
                        rows.RefundInitiatedDate = dataset.Tables[0].Rows[i].ItemArray[8] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[8]) : "-1";
                        rows.ReasonForRefund = dataset.Tables[0].Rows[i].ItemArray[9] != null ? Convert.ToString(dataset.Tables[0].Rows[i].ItemArray[9]) : "-1";
                        rows.RefundInitiatedAmount = dataset.Tables[0].Rows[i].ItemArray[10] != null ? Convert.ToInt32(dataset.Tables[0].Rows[i].ItemArray[10]) : -1;
                        Parameters.Add(rows);
                    }
                    stream.Close();

                    response.TotalCount = Parameters.Count;

                    if (Parameters.Count > 0)
                    {
                        string SqlQuery = @"INSERT INTO refundsDB.refundData 
                                    (sNo, dateOfPurchase, ticketType, txnNo, ticketAmount, ticketStatus, ticketId, noOfTickets, refundInitiatedDate, reasonForRefund, refundInitiatedAmount) VALUES
                                    (@SNo, @DateOfPurchase, @TicketType, @TxnNo, @TicketAmount, @TicketStatus, @TicketId, @NoOfTickets, @RefundInitiatedDate, @ReasonForRefund, @RefundInitiatedAmount )";

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            
                            string checkDuplicateQuery = "SELECT COUNT(*) FROM refundsDB.refundData WHERE ticketId = @TicketId";
                            MySqlCommand checkCommand = new MySqlCommand(checkDuplicateQuery, _mySqlConnection);
                            checkCommand.Parameters.AddWithValue("@TicketId", rows.TicketId);

                            int existingRecords = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                            if (existingRecords > 0)
                            {
                               
                                response.DuplicateCount++;
                                continue;
                            }

                
                            using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                            {
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@SNo", rows.SNo);
                                sqlCommand.Parameters.AddWithValue("@DateOfPurchase", rows.DateOfPurchase);
                                sqlCommand.Parameters.AddWithValue("@TicketType", rows.TicketType);
                                sqlCommand.Parameters.AddWithValue("@TxnNo", rows.TxnNo);
                                sqlCommand.Parameters.AddWithValue("@TicketAmount", rows.TicketAmount);
                                sqlCommand.Parameters.AddWithValue("@TicketStatus", rows.TicketStatus);
                                sqlCommand.Parameters.AddWithValue("@TicketId", rows.TicketId);
                                sqlCommand.Parameters.AddWithValue("@NoOfTickets", rows.NoOfTickets);
                                sqlCommand.Parameters.AddWithValue("@RefundInitiatedDate", rows.RefundInitiatedDate);
                                sqlCommand.Parameters.AddWithValue("@ReasonForRefund", rows.ReasonForRefund);
                                sqlCommand.Parameters.AddWithValue("@RefundInitiatedAmount", rows.RefundInitiatedAmount);

                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                             
                                    return response;
                                }
                            }
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Incorrect File";
      
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            response.Message = response.DuplicateCount > 0
                         ? $"Upload completed with {response.DuplicateCount}  duplicates out of {response.TotalCount} records."
                         : $"Upload successful with {response.TotalCount} records and no duplicates.";

            return response;
        }

    }
}
