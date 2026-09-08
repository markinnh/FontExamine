namespace FontExamine.Helper;
/// <summary>
/// Represents what changes between different Font definitions, such as Segoe UI Symbol and Segoe Fluent Icons. This class is used to define the context packet for a specific font definition.
/// </summary>
public class ContextPacketConfig
{
    public string FileName { get; set; } = string.Empty;
    public string FontFamily { get; set; } = string.Empty;
    public IEnumerable<int> GlyphIds { get; set; }
}

public interface IContextPacketConsumer
{
    void ConsumeContextPacket(ContextPacketConfig contextPacket);
}