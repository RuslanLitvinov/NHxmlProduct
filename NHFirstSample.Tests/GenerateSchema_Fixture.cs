using NHFirstSample.Domain;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHFirstSample.Tests
{
    //[TestFixture]
    [TestClass]
    public class GenerateSchema_Fixture
    {
        //[Test]
        [TestMethod]
        public void Can_generate_schema()
        {
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Product).Assembly);

            new SchemaExport(cfg).Execute(false, true, false);
        }
    }
}