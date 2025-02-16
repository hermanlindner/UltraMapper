﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UltraMapper.Tests.Projections
{
    [TestClass]
    public class ProjectionNotNullToNotNull
    {

        [TestMethod]
        public void MapTestPropHasValue()
        {
            var mapper = new Mapper( cfg =>
            {
                cfg.MapTypes<A, D>()
                 .MapMember( a => a.BProp.CProp.Id, d => d.Id );
            } );

            var aObject = new A
            {
                BProp = new B { CProp = new C { Id = 2 } }
            };

            var result = mapper.Map<D>( aObject );
            Assert.AreEqual( 2, result.Id );
        }

        [TestMethod]
        public void MapTestPropDefault()
        {
            var mapper = new Mapper( cfg =>
            {
                cfg.MapTypes<A, D>()
                 .MapMember( a => a.BProp.CProp.Id, d => d.Id );
            } );

            var aObject = new A
            {
                BProp = new B { CProp = new C { Id = 0 } }
            };

            var result = mapper.Map<D>( aObject );
            Assert.AreEqual( 0, result.Id );
        }

        [TestMethod]
        public void MapTestChain1Null()
        {
            var mapper = new Mapper( cfg =>
            {
                cfg.MapTypes<A, D>()
                 .MapMember( a => a.BProp.CProp.Id, d => d.Id );
            } );
            var aObject = new A
            {
                BProp = null
            };
            var result = mapper.Map<D>( aObject );
            Assert.AreEqual( 0, result.Id );
        }

        [TestMethod]
        public void MapTestChain2Null()
        {
            var mapper = new Mapper( cfg =>
            {
                cfg.MapTypes<A, D>()
                 .MapMember( a => a.BProp.CProp.Id, d => d.Id );
            } );

            var aObject = new A
            {
                BProp = new B { CProp = null }
            };
            var result = mapper.Map<D>( aObject );
            Assert.AreEqual( 0, result.Id );
        }

        public class A
        {
            public B BProp { get; set; }
        }

        public class B
        {
            public C CProp { get; set; }
        }

        public class C
        {
            public long Id { get; set; }
        }

        public class D
        {
            public long Id { get; set; }
        }
    }
}
