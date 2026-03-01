using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Dbo
{
    public class Organizationlevels
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual bool IsRowDeleted { get; set; }
    }
}
