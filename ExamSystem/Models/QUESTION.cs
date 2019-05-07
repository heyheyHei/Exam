using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Models
{
    [Serializable]
    [SugarTable("QUESTION")]
    public class QUESTION : ModelContext, IEqualityComparer<QUESTION>, IComparable<QUESTION>
    {
        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Id", IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Name", IsNullable = false, Length = 2147483647)]
        public string Name { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Desc", IsNullable = true, Length = 2147483647)]
        public string Desc { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Answer", IsNullable = false, Length = 2147483647)]
        public string Answer { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "State", IsNullable = true)]
        public int? State { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "CategoryId", IsNullable = false)]
        public int CategoryId { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public CATEGORY Category
        {
            get
            {
                return base.CreateMapping<CATEGORY>().Single(o => o.Id == this.CategoryId);
            }
        }

        [SugarColumn(IsIgnore = true)]
        public List<CHOISE> Choises
        {
            get
            {
                return base.CreateMapping<CHOISE>().Where(o => o.QuestionId == this.Id).ToList();
            }
        }

        public int CompareTo(QUESTION other)
        {
            return CategoryId - other.CategoryId;
        }

        public bool Equals(QUESTION x, QUESTION y)
        {
            return x.CategoryId == y.CategoryId;
        }

        public int GetHashCode(QUESTION obj)
        {
            return this.GetHashCode();
        }
    }
}
