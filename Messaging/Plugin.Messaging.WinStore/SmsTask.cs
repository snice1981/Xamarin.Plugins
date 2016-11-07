using System;

namespace Plugin.Messaging
{
    internal class SmsTask : ISmsTask
    {
        public SmsTask()
        {
        }

        #region ISmsTask Members

        public bool CanSendSms => false;

        public bool CanSendSmsSilently => false;

        public void SendSms(string recipient, string message)
        {
            throw new PlatformNotSupportedException("Sending SMS not supported on Windows Store");
        }

        public void SendSmsSilently(string recipient, string message = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}