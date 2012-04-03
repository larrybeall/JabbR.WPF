using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jabbr.WPF.Infrastructure.Services
{
    public abstract class BaseService
    {
        private readonly SynchronizationContext _uiContext;

        protected BaseService()
        {
            _uiContext = SynchronizationContext.Current;
        }

        protected void PostOnUi(Action toPost)
        {
            if (_uiContext == null)
            {
                toPost();
                return;
            }

            _uiContext.Post(_ => toPost(), null);
        }
    }
}
