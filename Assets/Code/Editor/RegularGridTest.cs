using NUnit.Framework;
using UnityEngine;

namespace Code.Oop
{
    public class RegularGridTest
    {
        RegularGrid<string> _grid = new RegularGrid<string>(new Vector2(-30, -50), new Vector2(100, 100), 20, 1000);

        [TearDown]
        public void Clear()
        {
            _grid.Clear();
        }

        [Test]
        public void FoundAtTheSamePoint()
        {
            _grid.Add(new Vector2(35, 25), "A");
            Assert.AreEqual("A", _grid.Search(new Vector2(35, 25)).JoinToString());
        }

        [Test]
        public void FoundAtTheBL()
        {
            _grid.Add(new Vector2(35, 25), "A");
            Assert.AreEqual("A", _grid.Search(new Vector2(28, 19)).JoinToString());
        }

        [Test]
        public void FoundAtTheTL()
        {
            _grid.Add(new Vector2(35, 25), "A");
            Assert.AreEqual("A", _grid.Search(new Vector2(28, 31)).JoinToString());
        }

        [Test]
        public void FoundAtTheBR()
        {
            _grid.Add(new Vector2(35, 25), "A");
            Assert.AreEqual("A", _grid.Search(new Vector2(41, 19)).JoinToString());
        }

        [Test]
        public void FoundAtTheTR()
        {
            _grid.Add(new Vector2(35, 25), "A");
            Assert.AreEqual("A", _grid.Search(new Vector2(41, 31)).JoinToString());
        }

        [Test]
        public void NotFoundBehindBorder()
        {
            _grid.Add(new Vector2(-31, 0), "A");
            Assert.AreEqual("", _grid.Search(new Vector2(-31, 0)));
        }

        [Test]
        public void FoundTwoNeighbours()
        {
            _grid.Add(new Vector2(35, 25), "A");
            _grid.Add(new Vector2(25, 15), "B");
            _grid.Add(new Vector2(25, 5), "C");
            Assert.AreEqual("AB", _grid.Search(new Vector2(33, 23)).JoinToString());
        }

        [Test]
        public void FurthestNeighbourNotFound()
        {
            _grid.Add(new Vector2(35, 25), "A");
            _grid.Add(new Vector2(25, 5), "B");
            Assert.AreEqual("A", _grid.Search(new Vector2(33, 23)).JoinToString());
        }
    }
}