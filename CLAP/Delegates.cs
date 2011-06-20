using System;
using System.Collections.Generic;
using System.Text;

namespace CLAP
{
    public delegate void Action();

    public delegate TResult Func<T, TResult>(T arg);
}