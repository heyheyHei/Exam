using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Models
{
    [Serializable]
    [SugarTable("CATEGORY")]
    public class CATEGORY
    {
        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Id", IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "Name", IsNullable = false, Length = 22)]
        public string Name { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "State", IsNullable = true)]
        public int? State { get; set; }


        ///<summary>
        ///
        ///</summary>
        [SugarColumn(ColumnName = "CreateTime", IsNullable = true)]
        public DateTime? CreateTime { get; set; }


    }
}
