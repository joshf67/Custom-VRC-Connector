namespace Joshf67.ServerConnector.Development
{

    /// <summary>
    /// Used to control the depth of debugging
    /// </summary>
    public enum DevelopmentMode
    {
        /// <summary>
        /// Indicates no development mode is enabled
        /// </summary>
        None,
        /// <summary>
        /// Indicates basic development mode is enabled
        /// </summary>
        Basic,
        /// <summary>
        /// Indicates advanced development mode is enabled
        /// </summary>
        Advanced,
        /// <summary>
        /// Indicates that error development modes are enabled
        /// </summary>
        Warning,
        /// <summary>
        /// Indicates all development modes are enabled
        /// </summary>
        All,
    }

    /// <summary>
    /// Used to control the development modes of different classes all in one spot
    /// </summary>
    public static class DevelopmentManager
    {
        /// <summary>
        /// Stores the current development mode for the ByteConverter
        /// </summary>
        public const DevelopmentMode ByteConverterMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the MessagePacker
        /// </summary>
        public const DevelopmentMode MessagePackerMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the Connector
        /// </summary>
        public const DevelopmentMode ConnectorMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the ServerResponse
        /// </summary>
        public const DevelopmentMode ServerResponseMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the StringListener
        /// </summary>
        public const DevelopmentMode StringListenerMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the ImageListener
        /// </summary>
        public const DevelopmentMode ImageListenerMode = DevelopmentMode.None;

        /// <summary>
        /// Stores the current development mode for the Schema
        /// </summary>
        public const DevelopmentMode SchemaMode = DevelopmentMode.None;

        /// <summary>
        /// Checks if the requested development mode is enabled for a specific class type.
        /// </summary>
        /// <param name="requested">The requested development mode.</param>
        /// <param name="actual">The actual development mode for the class type.</param>
        /// <returns>True if the requested mode is enabled, false otherwise.</returns>
        private static bool IsEnabled(DevelopmentMode requested, DevelopmentMode actual)
        {
            // Check if the actual mode for the class type matches the requested mode or is set to "all".
            return actual == requested || actual == DevelopmentMode.All;
        }

        /// <summary>
        /// Tests if the development mode for Converter is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsByteConverterEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ByteConverterMode);
        }

        /// <summary>
        /// Tests if the development mode for MessagePacker is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsMessagePackerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, MessagePackerMode);
        }

        /// <summary>
        /// Tests if the development mode for Connector is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsConnectorEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ConnectorMode);
        }

        /// <summary>
        /// Tests if the development mode for ServerResponse is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsServerResponseEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ServerResponseMode);
        }

        /// <summary>
        /// Tests if the development mode for StringListener is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsStringListenerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, StringListenerMode);
        }

        /// <summary>
        /// Tests if the development mode for ImageListener is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsImageListenerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ImageListenerMode);
        }

        /// <summary>
        /// Tests if the development mode for Schema is enabled
        /// </summary>
        /// <param name="mode"> The mode to test for </param>
        /// <returns> If the current mode is the same, or all is selected </returns>
        public static bool IsSchemaEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, SchemaMode);
        }
    }

}