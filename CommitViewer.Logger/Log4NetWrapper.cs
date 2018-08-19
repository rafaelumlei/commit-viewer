using CommitViewer.Logger.Interfaces;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitViewer.Logger
{
    
    /// <summary>
    /// Log4net wrapper to avoid using log4net interfaces in the other projects
    /// </summary>
    public class Log4NetWrapper : ICommitViewerLog
    {

        private readonly ILog logger;

        public Log4NetWrapper(ILog logger)
        {
            this.logger = logger;
        }

        public void Debug(object message)
        {
            this.logger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            this.logger.Debug(message, exception);
        }

        public void Error(object message)
        {
            this.logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            this.logger.Error(message, exception);
        }

        public void Info(object message)
        {
            this.logger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            this.logger.Info(message, exception);
        }

        public void Warn(object message)
        {
            this.logger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            this.logger.Warn(message, exception);
        }
    }
}
