﻿using System;

namespace CLAP
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class VerbInterceptorAttribute : Attribute
    {
    }
}
