using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.CampusCommunity.DataAccess
{
    public class MccContext : DbContext
    {
        protected MccContext()
        {
        }

        public MccContext(DbContextOptions options) : base(options)
        {
        }
    }
}
