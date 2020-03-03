using Android.App;
using Android.Content;
using Android.Net;
using Android.Telephony;

namespace Plugin.Messaging
{
    // NOTE: http://developer.xamarin.com/recipes/android/networking/sms/send_an_sms/

    internal class SmsTask : BroadcastReceiver, ISmsTask
    {
        public SmsTask()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent?.Action;
            var result = ResultCode;

            if (action == SMS_SENT_INTENT_ACTION && result != Result.Ok)
            {
                OnReceiveAction(false);
            }
            else if (action == SMS_DELIVERY_INTENT_ACTION && result != Result.Ok)
            {
                OnReceiveAction(false);
            }
            else if (action == SMS_DELIVERY_INTENT_ACTION && result == Result.Ok)
            {
                OnReceiveAction(true);
            }
        }

        #region ISmsTask Members

        public event SmsDeliveryResult OnSmsDeliveryResult;

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

        public void SendSmsInBackground(string recipient, string message = null)
        {
            if (!CanSendSmsInBackground)
            {
                OnSmsDeliveryResult?.Invoke(isSuccessful: false);

                return;
            }

            message ??= string.Empty;

            _smsManager = SmsManager.Default;

            var piSent = PendingIntent.GetBroadcast(CONTEXT, 0, new Intent(SMS_SENT_INTENT_ACTION), 0);
            var piDelivered = PendingIntent.GetBroadcast(CONTEXT, 0, new Intent(SMS_DELIVERY_INTENT_ACTION), 0);

            CONTEXT.RegisterReceiver(this, new IntentFilter(SMS_SENT_INTENT_ACTION));
            CONTEXT.RegisterReceiver(this, new IntentFilter(SMS_DELIVERY_INTENT_ACTION));

            _smsManager.SendTextMessage(recipient, null, message, piSent, piDelivered);
        }

        #endregion

        #region Private Methods

        private void OnReceiveAction(bool result)
        {
            UnRegisterBroadcastReceiver();

            OnSmsDeliveryResult?.Invoke(isSuccessful:result);
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

        private const string SMS_DELIVERY_INTENT_ACTION = "BSN.Resa.DoctorApp.SMS_Delivery";
        private const string SMS_SENT_INTENT_ACTION = "BSN.Resa.DoctorApp.SMS_SEND";
        private SmsManager _smsManager;
        private static readonly Context CONTEXT = Application.Context;

        #endregion
    }
}