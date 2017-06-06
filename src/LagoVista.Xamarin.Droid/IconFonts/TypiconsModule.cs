using LagoVista.XPlat.Core.Icons;


namespace LagoVista.XPlat.Droid.IconFonts
{
    /// <summary>
    /// Defines the <see cref="TypiconsModule" /> icon module.
    /// </summary>
    /// <seealso cref="Plugin.Iconize.IconModule" />
    public sealed class TypiconsModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypiconsModule" /> class.
        /// </summary>
        public TypiconsModule()
            : base("Typicons", "Typicons", "Iconize/iconize-typicons.ttf", TypiconsCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}