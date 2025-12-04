using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Messages;
using CQRS.Core.Infrastructure;

namespace CQRS.Core.Commands
{
    public abstract class BaseCommand : Message, ICommand
    {
    }
}
