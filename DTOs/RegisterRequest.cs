﻿namespace ALLINONEPROJECTWITHOUTJS.DTOs
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
        public string PasswordHash { get; set; }
    }
}
