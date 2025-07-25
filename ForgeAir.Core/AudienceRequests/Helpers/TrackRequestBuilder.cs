using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.AudienceRequests.Helpers
{
    public class TrackRequestBuilder
    {
        public TrackRequest BuildTrackRequest(string smsBody)
        {
            string[] parts = smsBody.Split("|");

            TrackRequest request = new TrackRequest();
            request.SourceRequest.SourceNumber = parts[0];
            Regex regex = new Regex(@"(.*)\sby\s(.*)");
            Match match = regex.Match(parts[1]);
          //  request.name = match.Groups[3].Value;
            request.RequestedTrack = new Database.Models.Track();

            request.RequestedTrack.Title = match.Groups[1].Value;
         //   request.RequestedTrack.TrackArtists = new List<ArtistTrack>() { new ArtistTrack() { Artist = new Artist() { Name = match.Groups[2].Value } } };
            return request;
        }
    }
}
