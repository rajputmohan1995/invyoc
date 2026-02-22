using invyoc.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace invyoc.Services;

public class InvoiceService(IConfiguration configuration) : IInvoiceService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task SaveAsync(InvoiceViewModel invoice)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        await using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            string sql = @"
            INSERT INTO FreeTempInvoices
            (
                InvoiceNumber, InvoiceDate, DueDate, PONumber,
                PaymentTerms, PaymentNotes, Currency, IsPreview,
                Subtotal,

                CompanyName, CompanyEmail, CompanyGSTNo, CompanyLogoBase64,
                CompanyAddress, CompanyCity, CompanyState, CompanyCountry,
                CompanyPincode, CompanyContactNum,

                BillToName, BillToGSTNo,
                BillToAddress, BillToCity, BillToState,
                BillToCountry, BillToPincode, BillToContactNum,

                ShipToName, ShipToGSTNo,
                ShipToAddress, ShipToCity, ShipToState,
                ShipToCountry, ShipToPincode, ShipToContactNum,

                ItemsJson
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @InvoiceNumber, @InvoiceDate, @DueDate, @PONumber,
                @PaymentTerms, @PaymentNotes, @Currency, @IsPreview,
                @Subtotal,

                @CompanyName, @CompanyEmail, @CompanyGSTNo, @CompanyLogoBase64,
                @CompanyAddress, @CompanyCity, @CompanyState, @CompanyCountry,
                @CompanyPincode, @CompanyContactNum,

                @BillToName, @BillToGSTNo,
                @BillToAddress, @BillToCity, @BillToState,
                @BillToCountry, @BillToPincode, @BillToContactNum,

                @ShipToName, @ShipToGSTNo,
                @ShipToAddress, @ShipToCity, @ShipToState,
                @ShipToCountry, @ShipToPincode, @ShipToContactNum,

                @ItemsJson
            );";

            await using var cmd = new SqlCommand(sql, conn, (SqlTransaction)transaction);

            // =========================
            // Core Invoice
            // =========================
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
            cmd.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
            cmd.Parameters.AddWithValue("@DueDate", invoice.DueDate);
            cmd.Parameters.AddWithValue("@PONumber", (object?)invoice.PONumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentTerms", (object?)invoice.PaymentTerms ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentNotes", (object?)invoice.PaymentNotes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Currency", invoice.Currency);
            cmd.Parameters.AddWithValue("@IsPreview", invoice.IsPreview);
            cmd.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);

            // =========================
            // Company (Flattened)
            // =========================
            cmd.Parameters.AddWithValue("@CompanyName", (object?)invoice.Company?.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyEmail", (object?)invoice.Company?.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyGSTNo", (object?)invoice.Company?.GSTNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyLogoBase64", (object?)invoice.Company?.LogoBase64 ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@CompanyAddress", (object?)invoice.Company?.CompanyAddress?.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyCity", (object?)invoice.Company?.CompanyAddress?.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyState", (object?)invoice.Company?.CompanyAddress?.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyCountry", (object?)invoice.Company?.CompanyAddress?.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyPincode", (object?)invoice.Company?.CompanyAddress?.Pincode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyContactNum", (object?)invoice.Company?.CompanyAddress?.ContactNum ?? DBNull.Value);

            // =========================
            // BillTo
            // =========================
            cmd.Parameters.AddWithValue("@BillToName", (object?)invoice.BillTo?.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToGSTNo", (object?)invoice.BillTo?.GSTNo ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@BillToAddress", (object?)invoice.BillTo?.ClientAddress?.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToCity", (object?)invoice.BillTo?.ClientAddress?.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToState", (object?)invoice.BillTo?.ClientAddress?.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToCountry", (object?)invoice.BillTo?.ClientAddress?.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToPincode", (object?)invoice.BillTo?.ClientAddress?.Pincode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BillToContactNum", (object?)invoice.BillTo?.ClientAddress?.ContactNum ?? DBNull.Value);

            // =========================
            // ShipTo
            // =========================
            cmd.Parameters.AddWithValue("@ShipToName", (object?)invoice.ShipTo?.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToGSTNo", (object?)invoice.ShipTo?.GSTNo ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@ShipToAddress", (object?)invoice.ShipTo?.ClientAddress?.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToCity", (object?)invoice.ShipTo?.ClientAddress?.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToState", (object?)invoice.ShipTo?.ClientAddress?.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToCountry", (object?)invoice.ShipTo?.ClientAddress?.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToPincode", (object?)invoice.ShipTo?.ClientAddress?.Pincode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShipToContactNum", (object?)invoice.ShipTo?.ClientAddress?.ContactNum ?? DBNull.Value);

            // =========================
            // Items → JSON
            // =========================
            var itemsJson = JsonSerializer.Serialize(invoice.Items);
            cmd.Parameters.AddWithValue("@ItemsJson", itemsJson);

            await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<SavedInvoiceData>> GetAllAsync(CommonListVM commonListVM)
    {
        var result = new List<SavedInvoiceData>();

        await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await using var command = new SqlCommand("dbo.GetFreeTempInvoices", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Search",
            string.IsNullOrWhiteSpace(commonListVM.Search) ? DBNull.Value : commonListVM.Search);

        command.Parameters.AddWithValue("@Year", commonListVM.FilterYear);
        command.Parameters.AddWithValue("@Month", commonListVM.FilterMonth);


        command.Parameters.AddWithValue("@PageNumber", commonListVM.PageNum);
        command.Parameters.AddWithValue("@PageSize", commonListVM.PageSize);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(ConvertFromSqlReader(reader));
        }

        return result;
    }

    public async Task<SavedInvoiceData> GetByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await using var command = new SqlCommand("SELECT * FROM FreeTempInvoices WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new KeyNotFoundException($"Invoice with Id {id} not found.");

        return ConvertFromSqlReader(reader);
    }

    private static SavedInvoiceData ConvertFromSqlReader(SqlDataReader reader)
    {
        return new SavedInvoiceData
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Timestamp = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

            InvoiceVM = new InvoiceViewModel
            {
                InvoiceNumber = reader["InvoiceNumber"].ToString()!,
                Company = new CompanyInfo
                {
                    Name = reader["CompanyName"]?.ToString(),
                    Email = reader["CompanyEmail"]?.ToString(),
                    GSTNo = reader["CompanyGSTNo"]?.ToString(),
                    LogoBase64 = reader["CompanyLogoBase64"]?.ToString(),
                    CompanyAddress = new AddressInfo
                    {
                        Address = reader["CompanyAddress"]?.ToString(),
                        City = reader["CompanyCity"]?.ToString(),
                        State = reader["CompanyState"]?.ToString(),
                        Country = reader["CompanyCountry"]?.ToString(),
                        Pincode = reader["CompanyPincode"]?.ToString(),
                        ContactNum = reader["CompanyContactNum"]?.ToString()
                    }
                },
                BillTo = new ClientInfoRequired
                {
                    Name = reader["BillToName"]?.ToString(),
                    GSTNo = reader["BillToGSTNo"]?.ToString(),
                    ClientAddress = new AddressInfo
                    {
                        Address = reader["BillToAddress"]?.ToString(),
                        City = reader["BillToCity"]?.ToString(),
                        State = reader["BillToState"]?.ToString(),
                        Country = reader["BillToCountry"]?.ToString(),
                        Pincode = reader["BillToPincode"]?.ToString(),
                        ContactNum = reader["BillToContactNum"]?.ToString()
                    }
                },
                ShipTo = new ClientInfo
                {
                    Name = reader["ShipToName"]?.ToString(),
                    GSTNo = reader["ShipToGSTNo"]?.ToString(),
                    ClientAddress = new AddressInfo
                    {
                        Address = reader["ShipToAddress"]?.ToString(),
                        City = reader["ShipToCity"]?.ToString(),
                        State = reader["ShipToState"]?.ToString(),
                        Country = reader["ShipToCountry"]?.ToString(),
                        Pincode = reader["ShipToPincode"]?.ToString(),
                        ContactNum = reader["ShipToContactNum"]?.ToString()
                    }
                },
                InvoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                PONumber = reader["PONumber"]?.ToString(),
                PaymentTerms = reader["PaymentTerms"]?.ToString(),
                PaymentNotes = reader["PaymentNotes"]?.ToString(),
                Currency = reader["Currency"].ToString()!,
                IsPreview = reader.GetBoolean(reader.GetOrdinal("IsPreview")),
                Items = JsonSerializer.Deserialize<List<InvoiceItemViewModel>>(reader["ItemsJson"]?.ToString() ?? "[]") ?? []
            }
        };
    }
}
