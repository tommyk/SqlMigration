using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    //[TestFixture]
    public class BaseTestClass
    {
        private MockRepository _mockRepository;
        public event EventHandler Setup;

        [SetUp]
        public void ASetupMockRepo()
        {
            Mock = new MockRepository();
            InvokeSetupMethod(new EventArgs());
        }

        public MockRepository Mock { get; protected set; }

        private void InvokeSetupMethod(EventArgs e)
        {
            EventHandler method = Setup;
            if (method != null) method(this, e);
        }
    }
}
