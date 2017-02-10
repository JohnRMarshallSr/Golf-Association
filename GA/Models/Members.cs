using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GA.Data
{
    public partial class Members
    {
        public string DisplayName
        {
            get
            {
                return string.Format("{0}, {1}", Lastname, Firstname);
            }
        }
    }
}