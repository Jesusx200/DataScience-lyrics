using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDS
{
    public class Words
    {
         /// <summary>
         /// Year
         /// </summary>
        public String category { get; set; }
        /// <summary>
        /// For all is the count
        /// For the other cases the max
        /// Negative - Neutral - Positive
        /// </summary>
        public int[] values { get; set; }

        public Words(string category, int[] values)
        {
            this.category = category;
            this.values = values;
        }

    }
}