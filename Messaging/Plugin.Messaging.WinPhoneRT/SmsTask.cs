using System;
using Windows.ApplicationModel.Chat;

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
                var msg = new ChatMessage { Body = message };
                if (!string.IsNullOrWhiteSpace(recipient))
                    msg.Recipients.Add(recipient);

#pragma warning disable 4014
                ChatMessageManager.ShowComposeSmsMessageAsync(msg);
#pragma warning restore 4014
            }
        }

	    public void SendSmsSilently(string recipient, string message = null)
	    {
            throw new NotImplementedException();
	    }
        #endregion
    }
}