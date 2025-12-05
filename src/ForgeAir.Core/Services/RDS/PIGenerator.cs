using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ForgeAir.Core.Services.RDS
{
    public class PIGenerator
    {
        private readonly Dictionary<string, char> Countries = new Dictionary<string, char>
        {
            { "AL", '9' },
            { "DZ", '2' },
            { "GR", '1' },
            { "IT", '5' }
        };

        private readonly Dictionary<string, char> StationType = new Dictionary<string, char>
        {
            { "Local", '0' },
            { "International", '1' },
            { "National", '2' },
            { "Supra Regional", '3' },
            { "Region1", '4' },
            { "Region2", '5' },
            { "Region3", '6' },
            { "Region4", '7' },
            { "Region5", '8' },
            { "Region6", '9' },
            { "Region7", 'A' },
            { "Region8", 'B' },
            { "Region9", 'C' },
            { "Region10", 'D' },
            { "Region11", 'E' },
            { "Region12", 'F' },
        };

        private readonly char[] AllowedChars =
        {
           '0','1','2','3','4','5','6','7','8','9',
           'A','B','C','D','E','F'
        };

        private readonly string[] BlacklistedPI = { "FFFF", "1000", "DDDD", "A000" };

        // uses OS selected country
        public string GeneratePIOffline()
        {
            string iso = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            string finalPi = "FFFF";
            Random r = new Random();

            if (Countries.TryGetValue(iso, out var prefix))
            {
                // build mutable char array
                char[] piChars = finalPi.ToCharArray();

                // assign positions explicitly
                piChars[0] = prefix;                                // Country code
                piChars[1] = StationType["Region1"];                // Station type (hardcoded for now)
                piChars[2] = AllowedChars[r.Next(0, AllowedChars.Length)]; // random char 1
                piChars[3] = AllowedChars[r.Next(0, AllowedChars.Length)]; // random char 2

                finalPi = new string(piChars);

                // blacklist check
                if (BlacklistedPI.Contains(finalPi))
                {
                    Console.WriteLine("Invalid PI generated. Try again.");
                    return GeneratePIOffline(); // retry recursively
                }

                Console.WriteLine($"Generated PI for {iso}: {finalPi}");
                return finalPi;
            }
            else
            {
                Console.WriteLine($"No RDS prefix found for {iso}");
                return null;
            }
        }
    }
}
