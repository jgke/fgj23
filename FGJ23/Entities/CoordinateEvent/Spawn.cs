using FGJ23.Support;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Entities.CoordinateEvents
{
    public class Spawn : CoordinateEvent, ILoggable
    {
        [Loggable]
        public uint Team;

        public Spawn(ByteString data): base("spawn")
        {
            Team = data[0];
        }
    }
}
