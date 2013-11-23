using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using HB.UpgradeService.Models;

namespace HB.UpgradeService.DAL
{
    /// <summary>
    /// 数据库操作的基类，提供数据增、删、查的通用方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDB<T> : IDisposable where T : class
    {
       // private static ILog log = log4net.LogManager.GetLogger("GlobalExceptionLogger");
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBContainer"].ConnectionString;
        protected DBContainer database;
        private bool saveChanges
        {
            get
            {
                var modify = database.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted);
                if (modify.Count() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public BaseDB()
        {
            database = new DBContainer(connectionString);
        }
        /// <summary>
        /// 需要在上下文查询中加nolock时调用
        /// </summary>
        public void Nolock()
        {
            database.ExecuteStoreCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED", null);
        }


        /// <summary>
        /// 取指定类型的数据表
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <returns></returns>
        public IQueryable<R> Get<R>() where R : class
        {
            return database.CreateObjectSet<R>().AsQueryable<R>();
        }


        public IQueryable<T> Get()
        {
            return Get<T>();
        }

        public void Add(T t)
        {
            Add(new List<T>() { t });
        }

        public void Add(IList<T> ts)
        {
            ObjectSet<T> result = database.CreateObjectSet<T>();
            foreach (T t in ts)
            {
                result.AddObject(t);
            }
        }
        public void Delete(IList<T> ts)
        {
            ObjectSet<T> result = database.CreateObjectSet<T>();
            foreach (T t in ts)
            {
                result.DeleteObject(t);
            }
        }
        public void Delete(T t)
        {
            Delete(new List<T> { t });
        }

        /// <summary>
        /// 保存数据更改
        /// </summary>
        public void SubmitChanges()
        {
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    database.SaveChanges();
                    scope.Complete();
                }
                catch (System.Data.OptimisticConcurrencyException)
                {
                    try
                    {
                        //处理冲突
                        foreach (ObjectStateEntry entry in database.ObjectStateManager.GetObjectStateEntries(
                            EntityState.Added | EntityState.Modified | EntityState.Deleted))
                        {
                            database.Refresh(RefreshMode.ClientWins, entry);
                        }
                        database.SaveChanges();
                    }
                    catch (Exception error)
                    {
                        
                    }
                }
                catch (Exception error)
                {
                     
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~BaseDB()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                if (saveChanges)
                {
                    SubmitChanges();
                }
                if (database.Connection.State != ConnectionState.Closed)
                {
                    database.Connection.Close();
                }
                database.Dispose();
                database = null;
            }
        }
    }
}
