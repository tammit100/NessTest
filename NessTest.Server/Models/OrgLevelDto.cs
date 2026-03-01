namespace Models
{
    public class OrgLevelDto
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual bool IsRowDeleted { get; set; }
    }
}
