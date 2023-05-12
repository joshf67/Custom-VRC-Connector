namespace Joshf67.ServerConnector.Development
{

    /// <summary>
    /// Used to control the depth of debugging
    /// </summary>
    public enum DevelopmentMode
    {
        None,       // Indicates no development mode is enabled
        Basic,      // Indicates basic development mode is enabled
        Advanced,   // Indicates advanced development mode is enabled
        Warning,    // Indicates that error development modes are enabled
        All,        // Indicates all development modes are enabled
    }

    /// <summary>
    /// Used to control the development modes of different classes all in one spot
    /// </summary>
    public static class DevelopmentManager
    {
        // These constant variables control the development mode for each class type.
        public const DevelopmentMode ByteConverterMode = DevelopmentMode.None;
        public const DevelopmentMode MessagePackerMode = DevelopmentMode.None;
        public const DevelopmentMode ConnectorMode = DevelopmentMode.None;
        public const DevelopmentMode ServerResponseMode = DevelopmentMode.None;
        public const DevelopmentMode StringListenerMode = DevelopmentMode.None;
        public const DevelopmentMode ImageListenerMode = DevelopmentMode.None;
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

        // The following functions check if the requested development mode is enabled for the corresponding class type.
        public static bool IsByteConverterEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ByteConverterMode);
        }

        public static bool IsMessagePackerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, MessagePackerMode);
        }

        public static bool IsConnectorEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ConnectorMode);
        }

        public static bool IsServerResponseEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ServerResponseMode);
        }

        public static bool IsStringListenerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, StringListenerMode);
        }

        public static bool IsImageListenerEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, ImageListenerMode);
        }

        public static bool IsSchemaEnabled(DevelopmentMode mode)
        {
            return IsEnabled(mode, SchemaMode);
        }
    }

}