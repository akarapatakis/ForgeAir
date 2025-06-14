using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class StreamingEncoderData
    {

        /// <summary>
        /// The way the data is going to be sent and where
        /// </summary>
        public required Streaming.Enums.DataUpdateMethodEnum Method { get; set; }

        /// <summary>
        /// The data that will be sent to the encoder
        /// </summary>
        public required string Data { get; set; }

        /// <summary>
        /// pre-text that will be sent to the encoder which will take part before Data
        /// </summary>
        public string? PreText {  get; set; }

        /// <summary>
        /// post-text that will be sent to the encoder which will take part after Data
        /// </summary>
        public string? PostText { get; set; }

        /// <summary>
        /// The path that leads to an actual image file to send to the encoder
        /// </summary>
        public Uri? AlbumArtPath { get; set; }

        /// <summary>
        /// The memorystream that contains the raw image which will get sent to a file (and then passed to the encoder)
        /// </summary>
        public MemoryStream? AlbumArtStream { get; set; }

    }
}
