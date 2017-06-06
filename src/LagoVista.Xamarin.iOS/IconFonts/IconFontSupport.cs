using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.XPlat.iOS.IconFonts
{
    public static class IconFontSupport
    {
        public static void RegisterFonts()
        {
            LagoVista.XPlat.Core.Icons.Iconize.With(new EntypoPlusModule())
                .With(new FontAwesomeModule())
                .With(new IoniconsModule())
                .With(new MaterialModule())
                .With(new MeteoconsModule())
                .With(new SimpleLineIconsModule())
                .With(new TypiconsModule())
                .With(new WeatherIconsModule());
        }
    }
}