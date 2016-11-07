using System;
using Microsoft.Phone.Tasks;

namespace Plugin.Messaging
{
    internal class SmsTask : ISmsTask
    {
        public SmsTask()
        {
        }

        #region ISmsTask Members

        public bool CanSendSms => true;

        public bool CanSendSmsSilently => false;

        public void SendSms(string recipient = null, string message = null)
        {
            message = message ?? string.Empty;

            if (CanSendSms)
            {
                SmsComposeTask smsComposeTask = new SmsComposeTask
                                                {
                                                    Body = message
                                                };
                if (!string.IsNullOrWhiteSpace(recipient))
                    smsComposeTask.To = recipient;

                smsComposeTask.Show();
            }
        }

        public void SendSmsSilently(string recipient, string message = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}