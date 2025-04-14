//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test {
    [TestClass]
    public class BusinessLogicImplementationUnitTest {
        [TestMethod]
        public void ConstructorTestMethod() {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixture())) {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod() {
            DataLayerDisposeFixture dataLayerFixcure = new DataLayerDisposeFixture();
            BusinessLogicImplementation newInstance = new(dataLayerFixcure);
            Assert.IsFalse(dataLayerFixcure.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixcure.Disposed);
        }

        [TestMethod]
        public void StartTestMethod() {
            DataLayerStartFixture dataLayerFixcure = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixcure)) {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
                Assert.AreEqual<int>(1, called);
                Assert.IsTrue(dataLayerFixcure.StartCalled);
                Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
            }
        }

        //[TestMethod]
        //public void HandleCollisions() {
        //    var ball1 = new DataBallFixture {
        //        Position = new TestVector(100, 100),
        //        Velocity = new TestVector(1, 0)
        //    };
        //    var ball2 = new DataBallFixture {
        //        Position = new TestVector(120, 100),
        //        Velocity = new TestVector(-1, 0)
        //    };

        //    var dataLayer = new DataLayerDisposeFixture();
        //    var businessLogic = new BusinessLogicImplementation(dataLayer);
     
        //    businessLogic.HandleCollisions();

        //    Assert.AreEqual(-1, ball1.Velocity.x);
        //    Assert.AreEqual(1, ball2.Velocity.x); ;
        //}



        private class DataLayerConstructorFixture : Data.DataAbstractAPI {
            public override IVector CreateVector(double x, double y) {
                throw new NotImplementedException();
            }

            public override void Dispose() { }

            public override IEnumerable<Data.IBall> GetBalls() {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler) {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixture : Data.DataAbstractAPI {
            internal bool Disposed = false;

            public override IVector CreateVector(double x, double y) {
                throw new NotImplementedException();
            }

            public override void Dispose() {
                Disposed = true;
            }

            public override IEnumerable<Data.IBall> GetBalls() {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler) {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixture : Data.DataAbstractAPI {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler) {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
            }

            public override IEnumerable<Data.IBall> GetBalls() {
                throw new NotImplementedException();
            }

            public override IVector CreateVector(double x, double y) {
                throw new NotImplementedException();
            }

            private record DataVectorFixture : Data.IVector {
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallFixture : Data.IBall {
                public IVector Velocity { get; set; }
                public IVector Position { get; set; }

                public event EventHandler<IVector>? NewPositionNotification = null;
            }
        }

        public class DataBallFixture : Data.IBall {
            public IVector Velocity { get; set; }
            public IVector Position { get; set; }

            public event EventHandler<IVector>? NewPositionNotification = null;
        }
        public record TestVector(double x, double y) : Data.IVector;


    }
}
