using ConDep.Console.Deploy;
using NUnit.Framework;

namespace ConDep.Console.Tests
{
    [TestFixture]
    public class CmdFactoryTests
    {
        [Test]
        public void TestThat_FactoryCanResolveDeploymentCommand()
        {
            var factory = new CmdFactory(new[] { "deploy" });
            Assert.That(factory.Resolve(), Is.InstanceOf<CmdDeployHandler>());
        }

    }
}