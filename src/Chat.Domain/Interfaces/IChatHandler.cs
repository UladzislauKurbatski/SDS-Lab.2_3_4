using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Domain.Interfaces
{
    public interface IChatHandler
    {
        void Handle(string username);
    }
}
