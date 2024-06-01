using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace RallyProtocol.Logging
{

    public interface IRallyLogger : ILogger
    {

        public void Log(string message);

        public void LogWarning(string message);

        public void LogError(string message);

        public void LogException(Exception exception);

    }

}