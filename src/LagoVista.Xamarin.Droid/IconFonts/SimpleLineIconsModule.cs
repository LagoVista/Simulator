using LagoVista.XPlat.Core.Icons;

namespace LagoVista.XPlat.Droid.IconFonts
{
    /// <summary>
    /// Defines the <see cref="SimpleLineIconsModule" /> icon module.
    /// </summary>
    /// <seealso cref="Plugin.Iconize.IconModule" />
    public sealed class SimpleLineIconsModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLineIconsModule" /> class.
        /// </summary>
        public SimpleLineIconsModule()
            : base("Simple Line Icons", "simple-line-icons", "Resources/Fonts/iconize-simplelineicons.ttf", SimpleLineIconsCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}