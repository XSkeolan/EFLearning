﻿namespace Messenger.Options
{
    public class JwtOptions
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int SessionExpires { get; set; }
        public int EmailLinkExpires { get; set; }
    }
}
