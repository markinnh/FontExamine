using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Helper;

public class MenuTagConfig
{
    public required string Id { get; set; }
    public required string Page { get; set; }
    public bool HasContextPacket { get; set; }
    public ContextPacketConfig? ContextPacket { get; set; }
}
