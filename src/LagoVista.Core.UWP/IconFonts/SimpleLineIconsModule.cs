﻿using LagoVista.Client.Core.Icons;

namespace LagoVista.Core.UWP.IconFonts
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
            : base("simple-line-icons", "simple-line-icons", "/Assets/Fonts/iconize-simplelineicons.ttf", SimpleLineIconsCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}