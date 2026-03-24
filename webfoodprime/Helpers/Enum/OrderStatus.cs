namespace webfoodprime.Helpers.Enum
{
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,   // đã nhận, đang làm
        Ready = 2,       // 🔥 đã làm xong, chờ shipper
        Delivering = 3,
        Completed = 4,
        Cancelled = 5
    }
}
