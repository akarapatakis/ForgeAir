using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.TCP.Properties
{
    public static class Commands
    {
        ///
        /// Playback Specific
        ///

        /// <summary>
        /// Returns the current track at the form: Artist - Title
        /// </summary>
        public const string GET_CURRENT_TRACK = "GET_CURRENT_TRACK";
        
        /// <summary>
        /// Returns the artist of the current track
        /// </summary>
        public const string GET_CURRENT_TRACK_ARTIST = "GET_CURRENT_TRACK_ARTIST";

        /// <summary>
        /// Returns the title of the current track
        /// </summary>
        public const string GET_CURRENT_TRACK_TITLE = "GET_CURRENT_TRACK_TITLE";

        ///
        /// Station Information Specific
        ///

        /// <summary>
        /// Returns the name of the station
        /// </summary>
        public const string GET_STATION_NAME = "GET_STATION_NAME";

        /// <summary>
        /// Returns the telephone number of the station
        /// </summary>
        public const string GET_STATION_TELEPHONE = "GET_STATION_TELEPHONE";

        /// <summary>
        /// Returns the email address of the station
        /// </summary>
        public const string GET_STATION_EMAIL = "GET_STATION_EMAIL";

        /// <summary>
        /// Returns the website of the station
        /// </summary>
        public const string GET_STATION_WEBSITE = "GET_STATION_WEBSITE";

        /// <summary>
        /// Returns the slogan of the station
        /// </summary>
        public const string GET_STATION_SLOGAN = "GET_STATION_SLOGAN";

    }
}
