using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Based on ForgeCrowd:
    https://github.com/ChocolateAdventurouz/ForgeCrowd
 */
namespace ForgeAir.Core.Models
{
    public class SMSRequest
    {
        /// <summary>
        /// The number that sends an SMS to the target number that is handled by ForgeCrowd
        /// </summary>
        public required string SourceNumber { get; set; }

        /// <summary>
        /// The content-SMS body text that is received by SourceNumber
        /// </summary>
        public required string Content { get; set; }
    }
}
