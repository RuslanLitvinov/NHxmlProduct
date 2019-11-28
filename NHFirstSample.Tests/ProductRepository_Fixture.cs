using NHFirstSample.Domain;
using NHFirstSample.Repository;
using NHibernate;
using NHibernate.Cfg;
//using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHFirstSample.Tests
{
    //[TestFixture]
    [TestClass]
    public class ProductRepository_Fixture
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        private readonly Product[] _products = new[]
                         {
                     new Product {Name = "Melon", Category = "Fruits"},
                     new Product {Name = "Pear", Category = "Fruits"},
                     new Product {Name = "Milk", Category = "Beverages"},
                     new Product {Name = "Coca Cola", Category = "Beverages"},
                     new Product {Name = "Pepsi Cola", Category = "Beverages"},
                 };

        private void CreateInitialData()
        {
            var generateSchema_Fixture = new GenerateSchema_Fixture();
            generateSchema_Fixture.Can_generate_schema();

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                foreach (var product in _products)
                    session.Save(product);
                transaction.Commit();
            }
        }
        public ProductRepository_Fixture()
        {
            TestFixtureSetUp();
            CreateInitialData();
        }

        //[TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _configuration = new Configuration();
            _configuration.Configure();
            _configuration.AddAssembly(typeof(Product).Assembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        //[Test]
        [TestMethod]
        public void Can_add_new_product()
        {
            var product = new Product { Name = "Apple", Category = "Fruits" };
            Console.WriteLine("1 product.id=" + product.Id);

            IProductRepository repository = new ProductRepository();
            repository.Add(product);
            Console.WriteLine("2 product.id=" + product.Id);

            // использовать сессию, чтобы попытаться загрузить продукт
            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                // Test that the product was successfully inserted
                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(product, fromDb);
                Assert.AreEqual(product.Name, fromDb.Name);
                Assert.AreEqual(product.Category, fromDb.Category);
            }
        }

        //[Test]
        [TestMethod]
        public void Can_update_existing_product()
        {
            var product = _products[0];
            product.Name = "Yellow Pear";
            IProductRepository repository = new ProductRepository();
            repository.Update(product);

            // use session to try to load the product
            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.AreEqual(product.Name, fromDb.Name);
            }
        }

        //[Test]
        [TestMethod]
        public void Can_remove_existing_product()
        {
            var product = _products[0];
            IProductRepository repository = new ProductRepository();
            repository.Remove(product);

            using (ISession session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNull(fromDb);
            }
        }

        //[Test]
        [TestMethod]
        public void Can_get_existing_product_by_id()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetById(_products[1].Id);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Name, fromDb.Name);
        }

        //[Test]
        [TestMethod]
        public void Can_get_existing_product_by_name()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetByName(_products[1].Name);

            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Id, fromDb.Id);
        }

        //[Test]
        [TestMethod]
        public void Can_get_existing_products_by_category()
        {
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.GetByCategory("Fruits");

            Assert.AreEqual(2, fromDb.Count);
            Assert.IsTrue(IsInCollection(_products[0], fromDb));
            Assert.IsTrue(IsInCollection(_products[1], fromDb));
        }

        private bool IsInCollection(Product product, ICollection<Product> fromDb)
        {
            foreach (var item in fromDb)
                if (product.Id == item.Id)
                    return true;
            return false;
        }
    }
}
