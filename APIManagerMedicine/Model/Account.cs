namespace APIManagerMedicine.Model
{
    public class Account
    {
        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? Avatar { get; set; }

        public string? UserType { get; set; }

        public int ActiveStatus { get; set; }

        public string? VisibleFunction { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? WorkDayofWeek { get; set; }

        public DateTime ActivationDate { get; set; }

        public DateTime DeactivationDate { get; set; }

        public int WorkShedule { get; set; }

        public int ActiveShedule { get; set; }

        public string? MaNV { get; set; }

        public string? MaCN { get; set; }
    }
}
