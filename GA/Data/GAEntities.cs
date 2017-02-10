using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace GA.Data
{
    public partial class GAEntities
    {
        public override int SaveChanges()
        {
            int rc = 0;
            try
            {
                rc = base.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                rc = -1;
                Debug.Assert(false);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                rc = -1;
                Debug.Assert(false);
                Console.WriteLine(ex.Message);
            }

            return rc;
        }
    }
}