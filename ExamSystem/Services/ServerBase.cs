using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SqlSugar;
using System.Linq.Expressions;

namespace ExamSystem.Services
{
    public class ServerBase<T> where T : class, new()
    {
        protected static string ConnString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        private static SqlSugarClient Client = new SqlSugarClient(
            new ConnectionConfig()
            {
                ConnectionString = ConnString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });

        public IEnumerable<T> GetList()
        {
            return Client.Queryable<T>().ToList();
        }
        public int GetCount()
        {
            return Client.Queryable<T>().Count();
        }
        public T GetOne(Expression<Func<T, bool>> where)
        {
            return Client.Queryable<T>().Single(where);
        }
        public bool Add(T entity)
        {
            return Client.Insertable<T>(entity).ExecuteCommand() > 0;
        }
        public bool Add(T[] entitys)
        {
            return Client.Insertable<T>(entitys).ExecuteCommand() > 0;
        }
        public bool Update(T entity)
        {
            return Client.Updateable(entity).ExecuteCommand() > 0;
        }
        public bool Update(List<T> entitys)
        {
            return Client.Updateable(entitys).ExecuteCommand() > 0;
        }
        public bool Delete(T entity)
        {
            return Client.Deleteable(entity).ExecuteCommand() > 0;
        }
        public bool DeleteByID(dynamic Id)
        {
            return Client.Deleteable<T>().In(Id).ExecuteCommand() > 0;
        }
        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = Client.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }

    }
}
