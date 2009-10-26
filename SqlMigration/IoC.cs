using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace SqlMigration
{
    public class IoC
    {
        private static readonly IoC _ioc = new IoC();
        private readonly IWindsorContainer _container;

        private IoC()
        {
            _container = new WindsorContainer(new XmlInterpreter());
        }

        private IWindsorContainer Container
        {
            get { return _container; }
        }

        public static IWindsorContainer Current
        {
            get { return _ioc.Container; }
        }
    }
}
