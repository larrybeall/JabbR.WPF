using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabbr.WPF
{
    public class JabbrManager
    {
        private static volatile JabbrManager _instance;
        private readonly static object _syncRoot = new object();

        private readonly JabbRClient _client;

        private JabbrManager()
        {
            _client = new JabbRClient("http://jabbr.net");
        }

        public static JabbrManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new JabbrManager();
                    }
                }

                return _instance;
            }
        }

        public JabbRClient Client
        {
            get { return _client; }
        }
    }
}
