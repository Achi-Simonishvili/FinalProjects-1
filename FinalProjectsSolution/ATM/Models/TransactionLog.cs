public class TransactionLog
{
    public string UserFullName { get; set; } = "";
    public string Action { get; set; } = ""; 
    public decimal Amount { get; set; } = 0;
    public decimal BalanceAfter { get; set; } = 0;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
