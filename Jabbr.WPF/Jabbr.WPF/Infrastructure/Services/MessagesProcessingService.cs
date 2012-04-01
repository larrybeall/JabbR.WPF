﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabbr.WPF.Infrastructure.Models;
using Jabbr.WPF.Messages;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using HtmlToXamlConversion;
using System.Threading.Tasks;
using Jabbr.WPF.Users;
using JabbrModels = JabbR.Client.Models;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class MessageProcessingService
    {
        private readonly SynchronizationContext _uiContext;
        private readonly ServiceLocator _serviceLocator;

        public MessageProcessingService(ServiceLocator serviceLocator)
        {
            _uiContext = SynchronizationContext.Current;
            _serviceLocator = serviceLocator;
        }

        public event EventHandler<MessageProcessedEventArgs> MessageProcessed;

        public void ProcessMessageAsync(string room, JabbrModels.Message message)
        {
            Message msg = new Message(message);
            ProcessMessageAsync(room, msg);
        }

        public void ProcessMessageAsync(string room, Message message)
        {
            Task<MessageProcessedEventArgs> task = new Task<MessageProcessedEventArgs>(() =>
            {
                var msgVm = CreateMessageViewModel(message);

                return new MessageProcessedEventArgs(msgVm, room);
            });

            task.ContinueWith((completedTask) => OnMessageProcessed(completedTask.Result, _uiContext),
                                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith((errorTask) =>
            {
                AggregateException exception = errorTask.Exception;

                if (exception == null)
                    return;

                foreach (var innerException in exception.InnerExceptions)
                {
                    System.Diagnostics.Trace.WriteLine(innerException.Message);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            task.Start();
        }

        public ChatMessageViewModel ProcessMessage(JabbrModels.Message message)
        {
            Message msg = new Message(message);
            return ProcessMessage(msg);
        }

        public ChatMessageViewModel ProcessMessage(Message message)
        {
            return CreateMessageViewModel(message);
        }

        private ChatMessageViewModel CreateMessageViewModel(Message message)
        {
            string content = ProcessEmoji(message.Content);
            var inlines = ParseMessage(content);
            var msgVm = _serviceLocator.GetViewModel<ChatMessageViewModel>();

            msgVm.IsNotifying = false;

            msgVm.Content = inlines;
            msgVm.RawContent = message.Content;
            msgVm.MessageDateTime = message.When.LocalDateTime;
            msgVm.MessageId = message.Id;
            msgVm.Username = message.User.Name;
            msgVm.GravatarHash = message.User.Hash;

            msgVm.IsNotifying = true;

            return msgVm;
        }

        private static string ProcessEmoji(string content)
        {
            return content;
        }

        private static InlineCollection ParseMessage(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            string xamlString = HtmlToXamlConverter.ConvertHtmlToXaml(content, false);
            var parsedXaml = XamlReader.Parse(xamlString);
            var xamlLines = ((Paragraph)((Section)parsedXaml).Blocks.FirstBlock).Inlines;

            return xamlLines;
        }

        private void OnMessageProcessed(MessageProcessedEventArgs args, SynchronizationContext context = null)
        {
            if (context != null)
            {
                context.Post(d => OnMessageProcessed(args), null);
                return;
            }

            var handler = MessageProcessed;
            if (handler != null)
                handler(this, args);
        }
    }
}
