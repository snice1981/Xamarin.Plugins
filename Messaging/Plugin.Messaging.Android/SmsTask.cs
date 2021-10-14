using Android.App;
using Android.Content;
using Android.Net;
using Android.Telephony;
using System.Collections.Generic;

namespace Plugin.Messaging
{
    // NOTE: http://developer.xamarin.com/recipes/android/networking/sms/send_an_sms/

    internal class SmsTask : BroadcastReceiver, ISmsTask
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent?.Action;
            var result = ResultCode;
            var recipient = intent.GetStringExtra("recipient");
            var message = intent.GetStringExtra("message");
            int id = intent.GetIntExtra("smsId", -1);

            int? smsId = id == -1 ? null : (int?)id;

            if (action == SMS_SENT_INTENT_ACTION && result != Result.Ok)
            {
                OnSMSSentAction(false, recipient, message, smsId);
            }
            else if (action == SMS_DELIVERY_INTENT_ACTION && result != Result.Ok)
            {
                OnSMSDeliveryAction(false, recipient, message, smsId);
            }
            else if (action == SMS_DELIVERY_INTENT_ACTION && result == Result.Ok)
            {
                _numberOfDeliveredMessageParts++;

                if (_numberOfDeliveredMessageParts >= _numberOfSmsMessageParts)
                {
                    OnSMSDeliveryAction(true, recipient, message, smsId);
                }
            }
        }

        #region ISmsTask Members

        public event SmsDeliveryResult OnSmsDeliveryResult;
        public event SmsDeliveryResult OnSmsSentResult;

        public bool CanSendSms => true;

        public bool CanSendSmsInBackground => true;

        public void SendSms(string recipient = null, string message = null)
        {
            message = message ?? string.Empty;

            if (CanSendSms)
            {
                Uri smsUri;
                if (!string.IsNullOrWhiteSpace(recipient))
                    smsUri = Uri.Parse("smsto:" + recipient);
                else
                    smsUri = Uri.Parse("smsto:");

                var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                smsIntent.PutExtra("sms_body", message);

                smsIntent.StartNewActivity();
            }
        }

        public void SendSmsInBackground(string recipient, string message = null, int? smsId = null)
        {
            if (!CanSendSmsInBackground)
            {
                OnSmsDeliveryResult?.Invoke(isSuccessful: false, recipient, message, smsId);

                return;
            }

            message ??= string.Empty;

            _smsManager = SmsManager.Default;

            CONTEXT.RegisterReceiver(this, new IntentFilter(SMS_SENT_INTENT_ACTION));
            CONTEXT.RegisterReceiver(this, new IntentFilter(SMS_DELIVERY_INTENT_ACTION));

            IList<string> messageParts = _smsManager.DivideMessage(message);

            _numberOfSmsMessageParts = messageParts.Count;

            _numberOfDeliveredMessageParts = 0;

            var sentPendingIntents = new List<PendingIntent>();
            var deliveryPendingIntents = new List<PendingIntent>();

            var sentIntent = new Intent(SMS_SENT_INTENT_ACTION);
            sentIntent.PutExtra("recipient", recipient);
            sentIntent.PutExtra("message", message);            

            var deliveredIntent = new Intent(SMS_DELIVERY_INTENT_ACTION);
            deliveredIntent.PutExtra("recipient", recipient);
            deliveredIntent.PutExtra("message", message);

            if (smsId != null)
            {
                sentIntent.PutExtra("smsId", (int)smsId);
                deliveredIntent.PutExtra("smsId", (int)smsId);
            }

            var sentPendingIntent = PendingIntent.GetBroadcast(CONTEXT, 0, sentIntent, PendingIntentFlags.OneShot);
            var deliveredPendingIntent = PendingIntent.GetBroadcast(CONTEXT, 0, deliveredIntent, PendingIntentFlags.OneShot);

            for (int i = 0; i < _numberOfSmsMessageParts; i++)
            {
                sentPendingIntents.Add(sentPendingIntent);
                deliveryPendingIntents.Add(deliveredPendingIntent);
            }

            _smsManager.SendMultipartTextMessage(recipient, null, messageParts, sentPendingIntents, deliveryPendingIntents);
        }

        #endregion

        #region Private Methods

        private void OnSMSDeliveryAction(bool success, string recipient, string message, int? smsId)
        {
            UnRegisterBroadcastReceiver();

            OnSmsDeliveryResult?.Invoke(isSuccessful:success, recipient, message, smsId);
        }

        private void OnSMSSentAction(bool success, string recipient, string message, int? smsId)
        {
            UnRegisterBroadcastReceiver();

            OnSmsSentResult?.Invoke(isSuccessful: success, recipient, message, smsId);
        }

        private void UnRegisterBroadcastReceiver()
        {
            try
            {
                CONTEXT.UnregisterReceiver(this);
            }
            catch (Java.Lang.IllegalArgumentException)
            {
            }
        }

        #endregion

        #region Private Field

        private static readonly string SMS_DELIVERY_INTENT_ACTION = $"{Application.Context.PackageName}.Action_SMS_Delivery";
        private static readonly string SMS_SENT_INTENT_ACTION = $"{Application.Context.PackageName}.Action_SMS_SEND";
        private SmsManager _smsManager;
        private static readonly Context CONTEXT = Application.Context;
        private int _numberOfSmsMessageParts;
        private int _numberOfDeliveredMessageParts;

        #endregion
    }
}