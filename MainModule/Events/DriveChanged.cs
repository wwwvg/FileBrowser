using MainModule.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Events
{
    internal class DriveChanged : PubSubEvent<DriveModel>
    {

    }
}
