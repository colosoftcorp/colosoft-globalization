using System;

namespace Colosoft
{
    public interface IServerData
    {
        DateTime GetDateTime();

        DateTimeOffset GateDateTimeOffSet();
    }
}
