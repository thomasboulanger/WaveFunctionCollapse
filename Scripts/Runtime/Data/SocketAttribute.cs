using NaughtyAttributes;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class SocketAttribute : DrawerAttribute
{
    public ESocketMode mode;
    public SocketAttribute (ESocketMode mode)
    {
        this.mode = mode;
    }

    public enum ESocketMode
    {
        Side, Vertical
    }
}
