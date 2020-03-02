using Android.App;
using Android.Content;
using Android.Net;
using Android.Telephony;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

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

            if (action == MY_SMS_SENT_INTENT_ACTION && result != Result.Ok)
            {
                OnReceiveAction(false);
            }
            else if (action == MY_SMS_DELIVERY_INTENT_ACTION && result != Result.Ok)
            {
                OnReceiveAction(false);
            }
            else if (action == MY_SMS_DELIVERY_INTENT_ACTION && result == Result.Ok)
            {
                OnReceiveAction(true);
            }
        }

        #region ISmsTask Members

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

        public async Task<bool> SendSmsInBackground(string recipient, string message = null, CancellationTokenSource cancellationToken = default)
        {
            if (!CanSendSmsInBackground)
                return false;

            message ??= string.Empty;

            _cancellationToken = cancellationToken;

            _taskCompletionSource = new TaskCompletionSource<bool>();
            Task<bool> task = _taskCompletionSource.Task;

            _smsManager = SmsManager.Default;

            var piSent = PendingIntent.GetBroadcast(CONTEXT, 0, new Intent(MY_SMS_SENT_INTENT_ACTION), 0);
            var piDelivered = PendingIntent.GetBroadcast(CONTEXT, 0, new Intent(MY_SMS_DELIVERY_INTENT_ACTION), 0);

            CONTEXT.RegisterReceiver(this, new IntentFilter(MY_SMS_SENT_INTENT_ACTION));
            CONTEXT.RegisterReceiver(this, new IntentFilter(MY_SMS_DELIVERY_INTENT_ACTION));

            await Task.Run(SetTimer).ConfigureAwait(false);

            await Task.Run(() =>
            {
                _smsManager.SendTextMessage(recipient, null, message, piSent, piDelivered);
            }).ConfigureAwait(false);

            bool result = task.Result;

            ResetTimer();

            return result;
        }

        #endregion

        #region Private Methods

        private void OnReceiveAction(bool setResult)
        {
            bool timedOut = _cancellationToken?.IsCancellationRequested ?? true;

            if (_taskCompletionSource != null && !timedOut)
            {
                _taskCompletionSource.SetResult(setResult);

                UnRegisterBroadcastReceiver();

                ResetTimer();
            }
        }

        private void SetTimer()
        {
            _timer = new Timer()
            {
                AutoReset = true,
                Interval = 1000,
                Enabled = true
            };

            _timer.Elapsed += (sender, args) =>
            {
                if (_cancellationToken?.IsCancellationRequested ?? true)
                {
                    _timer.Stop();

                    _taskCompletionSource?.SetResult(false);

                    UnRegisterBroadcastReceiver();
                }
            };

            _timer.Start();
        }

        private void ResetTimer()
        {
            if (_timer == null)
                return;

            _timer.Stop();

            try
            {
                _timer.Dispose();
            }
            catch
            {
            }
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

        private const string MY_SMS_DELIVERY_INTENT_ACTION = "BSN.Resa.DoctorApp.SMS_Delivery";
        private const string MY_SMS_SENT_INTENT_ACTION = "BSN.Resa.DoctorApp.SMS_SEND";
        private SmsManager _smsManager;
        private volatile TaskCompletionSource<bool> _taskCompletionSource;
        private CancellationTokenSource _cancellationToken;
        private Timer _timer;
        private static readonly Context CONTEXT = Application.Context;

        #endregion
    }
}