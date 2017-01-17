using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;
using LLibrary;

namespace CPiao.TaskRun
{
    public class DisposableJob : IJob, IDisposable
    {
        static DisposableJob()
        {
            L.Register("[disposable]");
        }

        public void Execute()
        {
            L.Log("[disposable]", "Just executed.");
        }

        public void Dispose()
        {
            L.Log("[disposable]", "Disposed properly.");
        }
    }
}