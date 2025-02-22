namespace NHSv2.Payments.Domain.Transactions;

public enum TransactionStatus
{
    Pending,
    Success,
    Failed,
    RequiresAction,
}