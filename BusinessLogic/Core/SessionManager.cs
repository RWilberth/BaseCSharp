using BusinessLogic.ObjectMap;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Web;

namespace BusinessLogic.Core
{
    public class SessionManager
    {
        private static ISessionFactory Factory { get; set; }

        public static string ConnectionString { get; set; }
        

        

        static SessionManager()
        {
            ConnectionString = null;
        }
        private static FluentConfiguration GetConfig()
        {
            return Fluently.Configure()
                .ExposeConfiguration(x =>
                {
                    x.SetInterceptor(new SqlStatementInterceptor());
                })
                .Database(MySQLConfiguration.Standard
                .ShowSql()
                .ConnectionString(c => c.Is(ConnectionString)))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>());
        }

        private static ISessionFactory GetFactory<T>() where T : ICurrentSessionContext
        {
            return GetConfig()
                //.Cache(c => c.ProviderClass<SysCacheProvider>().UseQueryCache())
                .CurrentSessionContext<T>().BuildSessionFactory();
        }
        

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static ISession GetCurrentSession()
        {
            string test = "";
            if (Factory == null)
                Factory = test != null ? GetFactory<WebSessionContext>() : GetFactory<ThreadStaticSessionContext>();

            if (CurrentSessionContext.HasBind(Factory))
            {
                ISession currentSession = Factory.GetCurrentSession();
                if (currentSession.IsOpen)
                {
                    return currentSession;
                }
            }

            var session = Factory.OpenSession();
            CurrentSessionContext.Bind(session);

            return session;
        }


        /// <summary>
        /// Closes the session.
        /// </summary>
        public static void CloseSession()
        {
            if (Factory != null && CurrentSessionContext.HasBind(Factory))
            {
                var session = CurrentSessionContext.Unbind(Factory);
                if (session.IsOpen)
                {
                    session.Close();
                }
            }
        }


        /// <summary>
        /// Commits the session.
        /// </summary>
        /// <param name="session">The session.</param>
        public static void CommitSession(ISession session)
        {
            try
            {
                session.Transaction.Commit();
            }
            catch (Exception)
            {
                session.Transaction.Rollback();
                throw;
            }
        }


        /// <summary>
        /// Creates the database from mapping in this assembly
        /// </summary>
        public static void CreateSchemaFromMappings()
        {
            var config = GetConfig();

            new SchemaExport(config.BuildConfiguration()).Create(false, true);
        }
        public static void DropchemaFromMappings()
        {
            FluentConfiguration config = Fluently.Configure();

            new SchemaExport(config.BuildConfiguration()).Drop(true, true);
        }
        public static void UpdateSchemaFromMappings()
        {
            var config = GetConfig();

            new SchemaUpdate(config.BuildConfiguration()).Execute(false, true);
        }

        public static void ExportSchemaFromMappings()
        {
            var config = GetConfig();

            new SchemaExport(config.BuildConfiguration()).SetOutputFile(@"C:\Users\Ensitech\Documents\oma.sql").Execute(true, false, false);//.Create(true,false);
        }

        public class SqlStatementInterceptor : EmptyInterceptor
        {
            public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
            {
                System.Diagnostics.Debug.WriteLine(sql.ToString());
                System.Diagnostics.Trace.WriteLine(sql.ToString());
                return sql;
            }
        }
    }
}
