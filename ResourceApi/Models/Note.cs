﻿namespace ResourceApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string UserId { get; set; }
    }
}
