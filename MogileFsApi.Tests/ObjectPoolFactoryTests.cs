using System;
using NUnit.Framework;
using Primelabs.Twingly.MogileFsApi;
using Primelabs.Twingly.MogileFsApi.ObjectPool;
using Rhino.Mocks;

namespace MogileFsApi.Tests
{
    [TestFixture]
    public class ObjectPoolFactoryTests
    {

        [Test]
        public void When_handle_is_borrowed_invalidate_is_set_to_true()
        {
            var mogileFsMock = MockRepository.GenerateStub<IMogileFs>();
            var mogileFsFactory = MockRepository.GenerateStub<IObjectPoolFactory<IMogileFs>>();
            mogileFsFactory.Expect(x => x.CreateInstance())
                .Return(mogileFsMock);

            var pool = new ObjectPool<IMogileFs>(mogileFsFactory);

            using (var obj = pool.Borrow())
            {
                Assert.AreEqual(true, obj.Invalidate);
            }

            mogileFsFactory.AssertWasCalled(x => x.CreateInstance());
        }

        [Test]
        public void Handle_is_invalidated_when_invalidate_is_not_set()
        {
            var mogileFsMock = MockRepository.GenerateStub<IMogileFs>();
            var mogileFsFactory = MockRepository.GenerateStub<IObjectPoolFactory<IMogileFs>>();
            mogileFsFactory.Stub(x => x.CreateInstance()).Return(mogileFsMock);

            var pool = new ObjectPool<IMogileFs>(mogileFsFactory);

            using (pool.Borrow()) {
                
            }
            
            using (pool.Borrow()) {
                
            }

            mogileFsFactory.AssertWasCalled(x => x.CreateInstance(), options => options.Repeat.Twice());
        }

        [Test]
        public void Instance_is_reused_when_invalidate_is_set_to_false()
        {
            var mogileFsMock = MockRepository.GenerateStub<IMogileFs>();
            var mogileFsFactory = MockRepository.GenerateStub<IObjectPoolFactory<IMogileFs>>();
            mogileFsFactory.Stub(x => x.CreateInstance()).Return(mogileFsMock);

            var pool = new ObjectPool<IMogileFs>(mogileFsFactory);

            using (var x = pool.Borrow())
            {
                x.Invalidate = false;
            }

            using (var x = pool.Borrow())
            {
                x.Invalidate = false;
            }

            mogileFsFactory.AssertWasCalled(x => x.CreateInstance(), options => options.Repeat.Once());
        }
    }
}