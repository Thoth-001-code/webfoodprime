namespace webfoodprime.DTOs.Admin
{
    public class DashboardDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ReadyOrders { get; set; }
        public int DeliveringOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

        public List<RevenueByDateDTO> RevenueByDate { get; set; }
    }
}
