using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmOAuthMvcDemo.Models
{
    public class DynCrmModel
    {
        [Key]
        public int DynCrmModelId { get; set; }
        public Guid LoggedInUserId { get; set; }

        public string LoggedInUserName { get; set; }

        public string BusinessUnitName { get; set; }

        public string DefaultPublisherName { get; set; }

    }

    public class DynCrmModelCtx : DbContext
    {
        public DbSet<DynCrmModel> DynCrmModelItemList { get; set; }
    }
}
