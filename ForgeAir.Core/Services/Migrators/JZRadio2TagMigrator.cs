using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.Services.Migrators
{
    public class JZRadio2TagMigrator
    {
        public Track GenerateTrackFromInfoTag(string infoTag)
        {
            //Jazler 2.0.x InfoTag Radio Automation (www.jazler.com) |AMARYLLIS|-|S'AGAPAO|TA KALHTERA|TA KALHTERA|TA KALHTERA|OLA|OLA|True|260,0088|0|255,062|6/1/1900|F:\TRAGOYDIA\AMARYLLIS 2008\S'AGAPAO.mp3||1|1|False|2,059|False|3915|-|-|0|NotEntered||0||80||-|NotEntered|0|||0|0||27/8/2008 5:05:22 ìì|27/8/2008 6:29:22 ðì|False||False|False|13/7/2008 5:23:46 ìì|13/7/2008 5:23:46 ìì|
            //Jazler 2.0.x InfoTag Radio Automation (www.jazler.com) |DOYKAS|KARTALH|NA MHN FOBASAI|MONTERNA REMIX|MONTERNA REMIX|MONTERNA REMIX|OLA|OLA|True|256,3255|0|249,593|23/1/1900|F:\TRAGOYDIA\EPILOGES 2006-7\NA MHN FOBASAI.mp3||3|3|False|0,387|False|1432|-|-|0|NotEntered||0||80||-|NotEntered|0|||0|0||16/10/2008 12:39:41 ðì|11/10/2008 9:03:42 ðì|False|-|False|False|14/5/2007 9:32:09 ìì|14/5/2007 9:32:09 ìì|
            string[] fields = infoTag.Split('|');


            var track = new Track()
            {
                Album = fields[4],
                Title = fields[3],
                TrackArtists = new List<ArtistTrack> { new ArtistTrack { Artist = new Artist() { Name = fields[1] } }, new ArtistTrack { Artist = new Artist() { Name = fields[2] } } },

            };

            return track;
        }
        public List<Artist> GenerateArtistFromInfoTag(string infoTag)
        {

            List<Artist> artistList = new List<Artist>() { };  

            //Jazler 2.0.x InfoTag Radio Automation (www.jazler.com) |AMARYLLIS|-|S'AGAPAO|TA KALHTERA|TA KALHTERA|TA KALHTERA|OLA|OLA|True|260,0088|0|255,062|6/1/1900|F:\TRAGOYDIA\AMARYLLIS 2008\S'AGAPAO.mp3||1|1|False|2,059|False|3915|-|-|0|NotEntered||0||80||-|NotEntered|0|||0|0||27/8/2008 5:05:22 ìì|27/8/2008 6:29:22 ðì|False||False|False|13/7/2008 5:23:46 ìì|13/7/2008 5:23:46 ìì|

            string[] fields = infoTag.Split('|');


            var artist = new Artist()
            {
                Name = fields[1],
            };
            var secondArtist = new Artist()
            {
                Name = fields[2],
            };
            artistList.Add(artist);
            artistList.Add(secondArtist);
            return artistList;
        }
    }
}
