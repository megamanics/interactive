﻿using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Jupyter.Messaging;
using Microsoft.DotNet.Interactive.Jupyter.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DotNet.Interactive.Jupyter.Connection
{
    internal class MessageToCommandAndEventConnector : IKernelCommandAndEventSender, IKernelCommandAndEventReceiver, ICommandExecutionContext, IDisposable
    {
        private readonly Subject<CommandOrEvent> _commandOrEventsSubject;
        private readonly Uri _targetUri;
        private readonly CompositeDisposable _disposables;

        // handlers
        private readonly IKernelCommandToMessageHandler<SubmitCode> _submitCodeHandler;
        private readonly IKernelCommandToMessageHandler<RequestValue> _requestValueHandler;
        private readonly IKernelCommandToMessageHandler<RequestValueInfos> _requestValueInfoHandler;
        private readonly IKernelCommandToMessageHandler<RequestKernelInfo> _requestKernelInfoHandler;

        public MessageToCommandAndEventConnector(IMessageSender messageSender, IMessageReceiver messageReceiver, Uri targetUri)
        {
            _commandOrEventsSubject = new Subject<CommandOrEvent>();
            _targetUri = targetUri;

            _submitCodeHandler = new SubmitCodeHandler(messageSender, messageReceiver);
            _requestValueHandler = new RequestValueHandler(messageSender, messageReceiver);
            _requestValueInfoHandler = new RequestValueInfoHandler(messageSender, messageReceiver);
            _requestKernelInfoHandler = new RequestKernelInfoHandler(messageSender, messageReceiver);

            _disposables = new CompositeDisposable
            {
                _commandOrEventsSubject
            };
        }


        public Uri RemoteHostUri => _targetUri;

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void Publish(KernelEvent kernelEvent)
        {
            var commandOrEvent = new CommandOrEvent(kernelEvent);
            _commandOrEventsSubject.OnNext(commandOrEvent);
        }

        public async Task SendAsync(KernelCommand kernelCommand, CancellationToken cancellationToken)
        {
            switch (kernelCommand)
            {
                case (SubmitCode submitCode):
                    await _submitCodeHandler.HandleCommandAsync(submitCode, this, cancellationToken);
                    break;
                case (RequestValue requestValue):
                    await _requestValueHandler.HandleCommandAsync(requestValue, this, cancellationToken);
                    break;
                case (RequestValueInfos requestValueInfos):
                    await _requestValueInfoHandler.HandleCommandAsync(requestValueInfos, this, cancellationToken);
                    break;
                case (RequestKernelInfo requestKernelInfo):
                    await _requestKernelInfoHandler.HandleCommandAsync(requestKernelInfo, this, cancellationToken);
                    break;
                default:
                    break;
            }
        }

        public Task SendAsync(KernelEvent kernelEvent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<CommandOrEvent> observer)
        {
            return _commandOrEventsSubject.Subscribe(observer);
        }
    }
}