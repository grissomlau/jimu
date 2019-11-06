using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.APM
{
    public interface IJimuApm
    {
        void Write(string name, object value);
        bool IsEnabled(string name);

        string ListenerName { get; }
    }
}
