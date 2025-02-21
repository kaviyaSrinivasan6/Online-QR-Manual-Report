using ExcelUpload.DataAccessLayer;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUploadFileDL, UploadFileDL>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services before building the app
builder.Services.AddCors(options => options.AddPolicy(name: "RefundOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }
));

var app = builder.Build();

// Create the database schema
CreateDatabaseScheme(builder.Configuration.GetConnectionString("MySQLString"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("RefundOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void CreateDatabaseScheme(string connectionString)
{
    var connectionStringWithoutDb = connectionString.Replace("Database=refundsDB;", "");

    using var connection = new MySqlConnection(connectionStringWithoutDb);
    connection.Open();

    var createDatabase = "CREATE DATABASE IF NOT EXISTS refundsDB;";
    using var createDbCmd = new MySqlCommand(createDatabase, connection);
    createDbCmd.ExecuteNonQuery();

    var useDatabase = "USE refundsDB;";
    using var useDbCmd = new MySqlCommand(useDatabase, connection);
    useDbCmd.ExecuteNonQuery();

    var createTable = @"
        CREATE TABLE IF NOT EXISTS refundData(
            Id INT PRIMARY KEY AUTO_INCREMENT,
            sNo INT(100),
            dateOfPurchase VARCHAR(100),
            ticketType VARCHAR(200),
            txnNo VARCHAR(100),
            ticketAmount INT(100),
            ticketStatus VARCHAR(200),
            ticketId VARCHAR(100),
            noOfTickets INT(100),
            refundInitiatedDate VARCHAR(100),
            reasonForRefund VARCHAR(200),
            refundInitiatedAmount INT(100)
        );";
    using var createTableCmd = new MySqlCommand(createTable, connection);
    createTableCmd.ExecuteNonQuery();
}
