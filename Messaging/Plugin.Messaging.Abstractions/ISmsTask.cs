namespace Plugin.Messaging
{
    /// <summary>
    /// Represents a method that will handle an event that is raised when an SMS was or wasn't delivered.
    /// </summary>
    /// <param name="isSuccessful"></param>
    public delegate void SmsDeliveryResult(bool isSuccessful);
    
    /// <summary>
    ///     Abstraction for sending cross-platform sms messages using
    ///     the default sms messenger on the device.
    /// </summary>
    /// <remarks>
    ///     On Android platform, the android.permission.SEND_SMS needs
    ///     to be added to the Android manifest.
    /// </remarks>
    public interface ISmsTask
    {
        /// <summary>
        ///     Gets a value indicating whether the device can send a sms
        /// </summary>
        bool CanSendSms { get; }

        /// <summary>
        ///     Gets a value indicating whether the device can send a sms
        ///     without user interaction.
        /// </summary>
        bool CanSendSmsInBackground { get; }

        /// <summary>
        ///     Send a sms using the default sms messenger on the device
        /// </summary>
        /// <param name="recipient">Sms recipient</param>
        /// <param name="message">Sms message</param>
        /// <remarks>
        ///     On Android platform, the android.permission.SEND_SMS needs
        ///     to be added to the Android manifest.
        /// </remarks>
        void SendSms(string recipient = null, string message = null);

        /// <summary>
        ///     Send a sms in the background without user interaction
        /// </summary>
        /// <param name="recipient">Sms recipient</param>
        /// <param name="message">Sms message</param>
        /// <remarks>
        ///     On UWP platform it requires the cellularMessaging
        ///     capability in the package.appxmanifest file.
        ///     On Android platform, the android.permission.SEND_SMS needs
        ///     to be added to the Android manifest.
        /// </remarks>
        void SendSmsInBackground(string recipient, string message = null);


        /// <summary>
        /// An event raised when the SMS is either delivered successfully or failed to be delivered.
        /// </summary>
        event SmsDeliveryResult OnSmsDeliveryResult;
    }
}