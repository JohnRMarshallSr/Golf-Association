using GA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GA.Models
{
    public class Winnings
    {
        public int MemberId { get; set; }
        public Members Member { get; set; }

        public decimal Tournament { get; set; }
        public decimal SideGame { get; set; }
        public decimal Total
        {
            get
            {
                return Tournament + SideGame;
            }
        }
        public decimal SixMan { get; set; }

        public Winnings()
        {
            Tournament = 0;
            SideGame = 0;
            SixMan = 0;
        }
    }
}