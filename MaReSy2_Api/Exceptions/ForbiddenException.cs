﻿namespace MaReSy2_Api.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string? message) : base(message)
        {
        }
    }
}
