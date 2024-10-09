using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace CircularQueue.Tests
{
    public class CircularQueueTests
    {
        private CircularQueue<int> queue;

        [SetUp]
        public void SetUp()
        {
            queue = new CircularQueue<int>();
        }

        [Test]
        public void Test_Constructor_Default()
        {
            Assert.AreEqual(0, queue.Count, "Queues count should be zero.");
            Assert.AreEqual(0, queue.StartIndex, "Start index has to be zero.");
            Assert.AreEqual(0, queue.EndIndex, "Zero index has to be zero.");
            Assert.GreaterOrEqual(queue.Capacity, 0, 
                "Queues capacity has to be higher or equal to zero.");
        }

        [Test]
        public void Test_Constructor_WithGivenCapacity()
        {
            CircularQueue<int> circluarQueue = new CircularQueue<int>(10);

            Assert.AreEqual(0, circluarQueue.Count, "Queues count should be zero.");
            Assert.AreEqual(10, circluarQueue.Capacity, "Queues count should be 10.");
            Assert.AreEqual(0, circluarQueue.StartIndex, "Start index has to be zero.");
            Assert.AreEqual(0, circluarQueue.EndIndex, "Zero index has to be zero.");
        }

        [Test]
        public void Test_Method_Enqueue()
        {
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            Assert.AreEqual(3, queue.Count, 
                "Queue count mismatches with actual count.");
            Assert.GreaterOrEqual(queue.Capacity, queue.Count, 
                "Count overshot the queues capacity.");
        }

        [Test]
        public void Test_Method_Enqueue_WithGrow()
        {
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);
            queue.Enqueue(40);
            queue.Enqueue(50);
            queue.Enqueue(60);
            queue.Enqueue(70);
            queue.Enqueue(80);
            queue.Enqueue(90);

            Assert.AreEqual(9, queue.Count, "Queue count mismatches with actual count.");
            Assert.AreEqual(16, queue.Capacity, "Grow did not multiply correctly.");
            Assert.AreEqual(0, queue.StartIndex, "Start index did not reset correctly.");
            Assert.AreEqual(9, queue.EndIndex, "End index did not reset correctly.");
        }

        [Test]
        public void Test_Method_Dequeue()
        {
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            int numOne = queue.Dequeue();
            int numTwo = queue.Dequeue();
            int numThree = queue.Dequeue();

            Assert.AreEqual(10, numOne, "Dequeue element mismatch");
            Assert.AreEqual(20, numTwo, "Dequeue element mismatch");
            Assert.AreEqual(30, numThree, "Dequeue element mismatch");
            Assert.AreEqual(0, queue.Count, "Expected and actual count mismatch.");
        }

        [Test]
        public void Test_Method_Dequeue_Empty()
        {
            Assert.Throws<InvalidOperationException>(
                () => queue.Dequeue(), 
                "Cannot dequeue on an empty queue");
        }

        [Test]
        public void Test_Method_EnqueueDequeue_RangeCross()
        {
            CircularQueue<int> queue = new CircularQueue<int>(5);
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            Assert.AreEqual(10, queue.Dequeue());
            Assert.AreEqual(20, queue.Dequeue());

            queue.Enqueue(40);
            queue.Enqueue(50);
            queue.Enqueue(60);

            Assert.AreEqual(4, queue.Count);
            Assert.AreEqual(5, queue.Capacity);
            Assert.Greater(queue.StartIndex, queue.EndIndex);
        }

        [Test]
        public void Test_Method_Peek()
        {
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);
            queue.Enqueue(40);
            queue.Enqueue(50);

            Assert.AreEqual(10, queue.Peek(), 
                "The queue returned the wrong element at peek.");
        }

        [Test]
        public void Test_Method_Peek_Empty()
        {
            Assert.Throws<InvalidOperationException>(
                () => queue.Peek(), 
                "Cannot peek an empty queue");
        }

        [TestCaseSource(nameof(ArrayCases))]
        public void Test_Method_ToArray(int[] nums, int[] expected)
        {
            foreach (int n in nums) queue.Enqueue(n);

            CollectionAssert.AreEqual(expected, queue.ToArray(), 
                "ToArray does not return correct array.");
        }

        [Test]
        public void Test_Method_ToArray_RangeCross()
        {
            // Arrange:
            // create an array of expected numbers
            // create a new queue of size 5
            int[] expected = { 30, 40, 50, 60 };
            CircularQueue<int> queue = new CircularQueue<int>(5);

            // Act:
            // enqueue 3 elements, dequeue 2, enqueue 3
            // to cross the queues boundary
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);
            queue.Dequeue();
            queue.Dequeue();
            queue.Enqueue(40);
            queue.Enqueue(50);
            queue.Enqueue(60);

            // Assert: collection assert the expected array ant the
            // array returned from "ToArray()"
            CollectionAssert.AreEqual(expected, queue.ToArray(), 
                "ToArray does not return correct array.");
        }
        
        [TestCaseSource(nameof(StringCases))]
        public void Test_Method_ToString(int[] nums, string expected)
        {
            foreach (int n in nums) queue.Enqueue(n);

            Assert.AreEqual(expected, queue.ToString(), 
                "ToString does not return correct string.");
        }

        [Test]
        public void Test_Multiple_Operatios()
        {
            CircularQueue<int> queue = new CircularQueue<int>(2);

            int added = 0;
            int removed = 0;
            int counter = 0;

            for (int i = 0; i < 300; i++)
            {
                Assert.AreEqual(added - removed, queue.Count);

                for (int j = 0; j < 2; j++)
                {
                    queue.Enqueue(++counter);
                    added++;
                    Assert.AreEqual(added - removed, queue.Count);
                }

                int peekEl = queue.Peek();
                Assert.AreEqual(removed + 1, peekEl);

                int dequeueEl = queue.Dequeue();
                removed++;
                Assert.AreEqual(removed, dequeueEl);
                Assert.AreEqual(added - removed, queue.Count);

                int[] expectedArr = Enumerable.Range(removed + 1, added - removed).ToArray();
                string expectedStr = $"[{string.Join(", ", expectedArr)}]";
                CollectionAssert.AreEqual(expectedArr, queue.ToArray());
                Assert.AreEqual(expectedStr, queue.ToString());

                Assert.True(queue.Capacity >= queue.Count);
            }

            Assert.True(queue.Capacity > 2);
        }

        [Test]
        [Timeout(500)]
        public void Test_Performance_MillionItems()
        {
            CircularQueue<int> queue = new CircularQueue<int>();

            int added = 0;
            int removed = 0;
            int counter = 0;

            for (int i = 0; i < 1_000_000 / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    queue.Enqueue(++counter);
                    added++;
                }

                queue.Dequeue();
                removed++;
            }

            Assert.AreEqual(added - removed, queue.Count);
        }

        private static IEnumerable<TestCaseData> StringCases()
        {
            TestCaseData[] testCases = new TestCaseData[]
            {
                new TestCaseData(
                    new int[] {  }, 
                    "[]"
                ),
                new TestCaseData(
                    new int[] { 10, 20, 30, 40, 50 }, 
                    "[10, 20, 30, 40, 50]"
                ),
                new TestCaseData(
                    new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 
                    "[10, 20, 30, 40, 50, 60, 70, 80, 90]"
                )
            };

            foreach (TestCaseData test in testCases)
            {
                yield return test;
            }
        }

        private static IEnumerable<TestCaseData> ArrayCases()
        {
            TestCaseData[] testCases = new TestCaseData[]
            {
                new TestCaseData(
                    new int[] {  }, 
                    new int[] { }
                ),
                new TestCaseData(
                    new int[] { 10, 20, 30, 40, 50 }, 
                    new int[] { 10, 20, 30, 40, 50 }
                ),
                new TestCaseData(
                    new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 
                    new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 }
                )
            };

            foreach (TestCaseData test in testCases)
            {
                yield return test;
            }
        }
    }
}
