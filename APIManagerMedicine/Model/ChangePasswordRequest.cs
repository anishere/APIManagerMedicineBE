﻿namespace APIManagerMedicine.Model
{
    public class ChangePasswordRequest
    {
        public int UserID { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
