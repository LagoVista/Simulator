using LagoVista.XPlat.Core.Icons;


namespace LagoVista.XPlat.Droid.IconFonts
{
    /// <summary>
    /// Defines the <see cref="IoniconsModule" /> icon module.
    /// </summary>
    /// <seealso cref="Plugin.Iconize.IconModule" />
    public sealed class IoniconsModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoniconsModule" /> class.
        /// </summary>
        public IoniconsModule()
            : base("Ionicons", "Ionicons", "Iconize/iconize-ionicons.ttf", IoniconsCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}