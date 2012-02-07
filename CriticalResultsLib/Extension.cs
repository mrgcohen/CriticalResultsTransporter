using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
    public class Extension
    {
        static object lockObj = new object();
        static DefaultExtension _DefaultExtension=null;
        public static DefaultExtension DefaultExtension
        {
            get
            {
                lock (lockObj)
                {
                    if (_DefaultExtension==null)
                    {
                        _DefaultExtension = new DefaultExtension();
                    }
                }
                return _DefaultExtension;
            }
            set
            {
                lock (lockObj)
                {
                    _DefaultExtension = value;
                }
            }
        }
    }
}
