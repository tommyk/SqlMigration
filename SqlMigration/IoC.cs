using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.Windsor;

namespace SqlMigration
{
    public class IoC
    {
        private static IoC _ioc = new IoC();
        private IWindsorContainer _container;
        private bool _hasRegisteredComponents = false;

        public IoC()
        {
            _container = new WindsorContainer();
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public static IoC Current
        {
            get { return _ioc; }
        }

        public void SetupWindsorContainer()
        {
            if (!_hasRegisteredComponents)
            {
                //setup the MigrationTask types
                _container.AddComponentLifeStyle("runSqlTask", typeof(RunSqlFileTask), LifestyleType.Transient);
                _container.AddComponentLifeStyle("deploymentTask", typeof(DeploymentTask), LifestyleType.Transient);
                _container.AddComponentLifeStyle("migrateTask", typeof(MigrateDatabaseForwardTask), LifestyleType.Transient);

                //fileIO
                _container.AddComponentLifeStyle("fileIO", typeof(IFileIO), typeof(FileIO), LifestyleType.Transient);
                _container.AddComponentLifeStyle("fileIOWrapper", typeof(IFileIOWrapper), typeof(FileIOWrapper), LifestyleType.Transient);

                //sqlRunner
                _container.AddComponentLifeStyle("sqlRunner", typeof(ISqlRunner), typeof(SqlRunner), LifestyleType.Transient);

                //db connection
                _container.AddComponentLifeStyle("dbConnection", typeof(IDbConnection), typeof(SqlConnection),
                                                 LifestyleType.Transient); 

                //migration types
                _container.AddComponentLifeStyle("tsqlMigration", typeof (TSqlMigration), LifestyleType.Transient);


                //flag we registered
                _hasRegisteredComponents = true;
            }

        }
    }
}
