namespace Plugin.Messaging
{
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
        ///     Gets a value indicating whether the device can send a sms silently
        /// </summary>
        bool CanSendSmsSilently { get; }

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
        ///     Send a sms in background siliently without any prompt or user interaction
        /// </summary>
        /// <param name="recipient">Sms recipient</param>
        /// <param name="message">Sms message</param>
        /// <remarks>
        ///     On Android platform, the android.permission.SEND_SMS needs
        ///     to be added to the Android manifest.
        ///     On UWP platform, the cellular capablity needs to be added.
        /// </remarks>
        void SendSmsSilently(string recipient, string message = null);
    }
}