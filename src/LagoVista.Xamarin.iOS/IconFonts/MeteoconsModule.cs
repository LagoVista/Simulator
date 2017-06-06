﻿using LagoVista.XPlat.Core.Icons;

namespace LagoVista.XPlat.iOS.IconFonts
{
    /// <summary>
    /// Defines the <see cref="MeteoconsModule" /> icon module.
    /// </summary>
    /// <seealso cref="Plugin.Iconize.IconModule" />
    public sealed class MeteoconsModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoconsModule" /> class.
        /// </summary>
        public MeteoconsModule()
            : base("Meteocons", "Meteocons", "iconize-meteocons.ttf", MeteoconsCollection.Icons)
        {
            // Intentionally left blank
        }
    }
}