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

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixture()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
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
        public void StartTestMethod()
        {
            DataLayerStartFixture dataLayerFixcure = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
            {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
                Assert.AreEqual<int>(10, called);
                Assert.IsTrue(dataLayerFixcure.StartCalled);
                Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallsCreatedByStart);
            }
        }

        private class DataLayerConstructorFixture : Data.DataAbstractAPI
        {
            internal bool Disposed = false;
            private Vector2 _dimensions = new Vector2(400, 420);

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                return;
            }

            public override Vector2 CreateVector(float x, float y)
            {
                return new Vector2(x, y);
            }

            public override List<Data.IBall> GetBalls()
            {
                return new List<Data.IBall>();
            }

            public override Vector2 GetDimensions()
            {
                return _dimensions;
            }

            public override void BeginMovement()
            {
                return;
            }
        }

        private class DataLayerDisposeFixture : Data.DataAbstractAPI
        {
            internal bool Disposed { get; private set; } = false;
            private Vector2 _dimensions = new Vector2(400, 420);

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                return;
            }

            public override Vector2 CreateVector(float x, float y)
            {
                return new Vector2(x, y);
            }

            public override List<Data.IBall> GetBalls()
            {
                return new List<Data.IBall>();
            }

            public override void BeginMovement()
            {
                return;
            }

            public override Vector2 GetDimensions()
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixture : Data.DataAbstractAPI
        {
            internal bool StartCalled { get; private set; } = false;
            internal int NumberOfBallsCreatedByStart { get; private set; } = 0;
            private List<Data.IBall> _balls = new List<Data.IBall>();
            private Vector2 _dimensions = new Vector2(400, 420);

            public override void Dispose()
            {
                _balls.Clear();
            }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallsCreatedByStart = numberOfBalls;
                _balls.Clear();

                for (int i = 0; i < numberOfBalls; i++)
                {
                    Vector2 dummyPos = new Vector2(i * 10, i * 10);
                    DataBallFixture newBall = new DataBallFixture(dummyPos, new Vector2(1, 1), 20);
                    _balls.Add(newBall);
                    upperLayerHandler?.Invoke(dummyPos, newBall);
                }
            }

            public override Vector2 CreateVector(float x, float y)
            {
                return new Vector2(x, y);
            }

            public override List<Data.IBall> GetBalls()
            {
                return _balls.ToList();
            }

            public override Vector2 GetDimensions()
            {
                return _dimensions;
            }

            public override void BeginMovement()
            {
                foreach (var ball in _balls.OfType<DataBallFixture>())
                {
                    ball.StartMovement();
                }
            }

            private class DataBallFixture : Data.IBall
            {
                public event EventHandler<Vector2>? NewPositionNotification;

                private Vector2 _velocity;
                private Vector2 _position;
                private readonly double _radius;

                public Vector2 Velocity
                {
                    get => _velocity;
                    set => _velocity = value;
                }
                public Vector2 Position { get => _position; private set => _position = value; }
                public double Radius => _radius;

                public DataBallFixture(Vector2 initialPosition, Vector2 initialVelocity, double radius)
                {
                    _position = initialPosition;
                    _velocity = initialVelocity;
                    _radius = radius;
                }

                public Task Move()
                {
                    _position += _velocity;
                    NewPositionNotification?.Invoke(this, _position);
                    return Task.CompletedTask;
                }

                public Task StartMovement()
                {
                    _position += _velocity;
                    NewPositionNotification?.Invoke(this, _position);
                    return Task.CompletedTask;
                }

                public void StopMovement()
                {
                    return;
                }
            }
        }

        [TestMethod]
        public void StartMethodCreatesBallsAndNotifies()
        {
            DataLayerStartFixture dataLayerFixcure = new DataLayerStartFixture();
            BusinessLogicImplementation businessLogic = new(dataLayerFixcure);

            int numberOfBallsToCreate = 3;
            int notificationCount = 0;
            List<TP.ConcurrentProgramming.BusinessLogic.IBall> receivedBusinessBalls = new();

            businessLogic.Start(numberOfBallsToCreate, (position, ball) =>
            {
                notificationCount++;
                receivedBusinessBalls.Add(ball);
                Assert.IsNotNull(ball);
            });

            Assert.IsTrue(dataLayerFixcure.StartCalled);
            Assert.AreEqual(numberOfBallsToCreate, dataLayerFixcure.NumberOfBallsCreatedByStart);
            Assert.AreEqual(numberOfBallsToCreate, dataLayerFixcure.GetBalls().Count);
            Assert.AreEqual(numberOfBallsToCreate, notificationCount);
            Assert.AreEqual(numberOfBallsToCreate, receivedBusinessBalls.Count);

            foreach (var businessBall in receivedBusinessBalls)
            {
                Assert.IsInstanceOfType(businessBall, typeof(TP.ConcurrentProgramming.BusinessLogic.IBall));
            }
        }

        [TestMethod]
        public void GetDimensionsReturnsCorrectBusinessDimensions()
        {
            DataLayerConstructorFixture dataLayerStub = new DataLayerConstructorFixture();
            BusinessLogicImplementation businessLogic = new(dataLayerStub);

            Vector2 dimensions = businessLogic.GetDimensions();

            Assert.IsNotNull(dimensions);
            Assert.AreEqual(400, dimensions.Y);
            Assert.AreEqual(420, dimensions.X);
        }

        [TestMethod]
        public void CreateVectorTestMethod()
        {
            DataLayerConstructorFixture dataLayerStub = new DataLayerConstructorFixture();
            Vector2 resultVector = dataLayerStub.CreateVector(5.0f, 7.0f);
            Assert.AreEqual(new Vector2(5.0f, 7.0f), resultVector);
        }

    }
}

