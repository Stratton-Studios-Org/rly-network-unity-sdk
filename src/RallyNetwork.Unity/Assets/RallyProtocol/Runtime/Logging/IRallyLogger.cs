using System;
using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Logging
{

    public interface IRallyLogger
    {

        public void Log(string message);

        public void LogAssertion(string message);

        public void LogWarning(string message);

        public void LogError(string message);

        public void LogException(Exception exception);


    }

}