using System;
using System.Threading;
using System.Threading.Tasks;
using Tizen.Messaging.Messages;

namespace Plugin.Messaging
{
	internal class SmsTask : ISmsTask
	{
		public SmsTask()
		{
		}

		public bool CanSendSms => true;

	    public bool CanSendSmsInBackground => false;

	    public async void SendSms(string recipient = null, string message = null)
		{
			message = message ?? string.Empty;

			if (CanSendSms)
			{
				var msg = new SmsMessage();
				if (!string.IsNullOrWhiteSpace(recipient))
				{
					msg.To.Add(new MessagesAddress(recipient));
				}
				msg.Text = message;
				msg.SimId = SimSlotId.Sim1;
				await MessagesManager.SendMessageAsync(msg, true);
			}
		}

		public Task<bool> SendSmsInBackground(string recipient, string message = null, CancellationTokenSource cancellationToken = default)
		{
			throw new PlatformNotSupportedException("Sending SMS in background not supported on Tizen");
		}
	}
}
