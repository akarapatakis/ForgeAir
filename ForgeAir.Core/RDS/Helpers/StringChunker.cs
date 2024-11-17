using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.Helpers
{
    public class StringChunker
    {
        int psChunkSize = 8;
        int rtChunkSize = 64;

        public async IAsyncEnumerable<string> ChunkPS(string input)
        {
            for (int i = 0; i < input.Length; i += psChunkSize)
            {
                string chunk = input.Substring(i, Math.Min(psChunkSize, input.Length - i));
                await Task.Delay(Shared.RDSParams.Instance.psHoldInterval); // Add delay of 2000ms or as specified
                yield return chunk; // Return the current chunk
            }
        }

        public async IAsyncEnumerable<string> ChunkRT(string input)
        {
            for (int i = 0; i < input.Length; i += rtChunkSize)
            {
                string chunk = input.Substring(i, Math.Min(rtChunkSize, input.Length - i));
                await Task.Delay(Shared.RDSParams.Instance.rtHoldInterval); // Add delay of 2000ms or as specified
                yield return chunk; // Return the current chunk
            }
        }
    }
}
