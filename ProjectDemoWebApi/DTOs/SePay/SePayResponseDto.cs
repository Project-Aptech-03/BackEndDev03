using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectDemoWebApi.DTOs.SePay
{
    public class SePayTransactionResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("error")]
        public object? Error { get; set; }

        [JsonPropertyName("messages")]
        public SePayMessages Messages { get; set; } = new();

        [JsonPropertyName("transactions")]
        public List<SePayTransaction> Transactions { get; set; } = new();
    }

    public class SePayMessages
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class SePayTransaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("bank_brand_name")]
        public string BankBrandName { get; set; } = string.Empty;

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; } = string.Empty;

        [JsonPropertyName("transaction_date")]
        public string TransactionDate { get; set; } = string.Empty;

        [JsonPropertyName("amount_out")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal AmountOut { get; set; }

        [JsonPropertyName("amount_in")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal AmountIn { get; set; }

        [JsonPropertyName("accumulated")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal Accumulated { get; set; }

        [JsonPropertyName("transaction_content")]
        public string TransactionContent { get; set; } = string.Empty;

        [JsonPropertyName("reference_number")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("sub_account")]
        public string? SubAccount { get; set; }

        [JsonPropertyName("bank_account_id")]
        public string BankAccountId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Custom converter to handle decimal values that come as strings from SePay API
    /// </summary>
    public class DecimalStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (decimal.TryParse(stringValue, out var result))
                {
                    return result;
                }
                return 0m; // Default value if parsing fails
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }
            
            return 0m; // Default value for other token types
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("F2"));
        }
    }
}