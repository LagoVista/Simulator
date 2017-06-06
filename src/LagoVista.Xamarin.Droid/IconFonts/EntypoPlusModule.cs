using LagoVista.XPlat.Core.Icons;


namespace LagoVista.XPlat.Droid.IconFonts
{
    /// <summary>
    /// Defines the <see cref="EntypoPlusModule" /> icon module.
    /// </summary>
    /// <seealso cref="Plugin.Iconize.IconModule" />
    public sealed class EntypoPlusModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntypoPlusModule" /> class.
        /// </summary>
        public EntypoPlusModule()
            : base("entypo-plus", "entypo-plus", "Iconize/iconize-entypoplus.ttf", EntypoPlusCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}