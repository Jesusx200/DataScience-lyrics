using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDS.Code
{
    public class Lyric
    {
        /// <summary>
        /// The Color of the word
        /// </summary>
        public String word { get; set; }
        /// <summary>
        /// 0 - Neutral,
        /// 1 - Negative,
        /// 2 - Positive
        /// </summary>
        public int value { get; set; }
    }
}