using System;
using Windows.ApplicationModel.Chat;
using Windows.Devices.Sms;
using System;

namespace Plugin.Messaging
{
    internal class SmsTask : ISmsTask
    {
        public SmsTask()
        {
        }

        #region ISmsTask Members

        public bool CanSendSms { get { return true; } }

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

	    public async void SendSmsSilently(string recipient, string message = null)
	    {
		    message = message ?? string.Empty;

		    if (CanSendSms)
		    {
			    SmsTextMessage2 sendingMessage = new SmsTextMessage2();
			    sendingMessage.Body = message;
			    sendingMessage.To = recipient;
			    SmsSendMessageResult result = await SmsDevice2.GetDefault().SendMessageAndGetResultAsync(sendingMessage);
			}
	    }
        #endregion
    }
}