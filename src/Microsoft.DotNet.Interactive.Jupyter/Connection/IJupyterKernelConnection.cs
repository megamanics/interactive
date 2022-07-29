﻿using Microsoft.DotNet.Interactive.Jupyter.Messaging;
using System;
using System.Threading.Tasks;

namespace Microsoft.DotNet.Interactive.Jupyter.Connection
{
    internal interface IJupyterKernelConnection : IDisposable
    {
        Task StartAsync();

        IMessageSender Sender { get; }

        IMessageReceiver Receiver { get; }
    }
}