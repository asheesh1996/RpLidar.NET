using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The lidar point group.
    /// </summary>
    public sealed class LidarPointGroup : IEnumerable<LidarPointGroupItem>
    {
        /// <summary>
        /// Gets or Sets the X.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or Sets the Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LidarPointGroup"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public LidarPointGroup(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or Sets the settings.
        /// </summary>
        public ILidarSettings Settings { get; set; }

        /// <summary>
        /// The dictionary.
        /// </summary>
        private Dictionary<int, LidarPointGroupItem> _dictionary = new Dictionary<int, LidarPointGroupItem>(3600);

        public LidarPointGroupItem this[float angle]
        {
            get
            {
                if (angle < 0)
                    angle = 360 + angle;
                var anglei = (int)(angle * 10);
                if (_dictionary.ContainsKey(anglei))
                {
                    return _dictionary[anglei];
                }

                return null;
            }
        }

        public LidarPointGroupItem this[int angle]
        {
            get
            {
                if (angle < 0)
                    angle = 360 + angle;
                var anglei = angle * 10;
                if (_dictionary.ContainsKey(anglei))
                {
                    return _dictionary[anglei];
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">The point.</param>
        public void Add(LidarPoint point)
        {
            if (point == null || !point.IsValid)
                return;

            var angle = (int)(point.Angle * 10);
            if (_dictionary.ContainsKey(angle))
            {
                var lidarPointGroupItem = _dictionary[angle];
                if (lidarPointGroupItem.Distance > point.Distance)
                {
                    lidarPointGroupItem.Distance = point.Distance;
                    lidarPointGroupItem.Count++;
                }
            }
            else
            {
                _dictionary.Add(angle, new LidarPointGroupItem()
                {
                    Angle = (int)point.Angle,
                    OriginalAngle = point.Angle,
                    Distance = point.Distance,
                    Count = 1,
                });
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>A double.</returns>
        public double Compare(LidarPointGroup group)
        {
            double distance = 0;
            var dictF = new Dictionary<int, int>();
            var points = _dictionary.Values.ToList();
            var lidarPointGroupItems = @group.Items.ToList();
            var intersectCount = 0;
            var pointsCount = points.Count;
            for (var index = 0; index < pointsCount; index++)
            {
                var point = points[index];

                for (var i = 0; i < lidarPointGroupItems.Count; i++)
                {
                    if (dictF.ContainsKey(i))
                        continue;

                    var second = lidarPointGroupItems[i];
                    if (Math.Abs(second.Distance - point.Distance) < 50)
                    {
                        dictF[i] = index;
                        intersectCount++;
                        break;
                    }
                }
            }
            if (intersectCount > 300)
            {
                return 1;
            }
            //if (intersectCount > 50)
            //{
            //	return .3;
            //}

            distance = (double)intersectCount / points.Count;
            return distance;
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <returns><![CDATA[List<LidarPoint>]]></returns>
        public List<LidarPoint> GetPoints()
        {
            var result = new List<LidarPoint>();
            foreach (var point in _dictionary.Values)
            {
                result.Add(new LidarPoint()
                {
                    Angle = point.OriginalAngle,
                    Distance = point.Distance
                });
            }

            return result;
        }

        /// <summary>
        /// Filters the list of lidarpoints.
        /// </summary>
        /// <returns><![CDATA[List<LidarPoint>]]></returns>
        public List<LidarPoint> Filter()
        {
            for (int i = 0; i <= 360; i++)
            {
                if (!_dictionary.ContainsKey(i))
                    _dictionary[i] = null;
            }
            var points = _dictionary.Values.ToList();
            var result = new List<LidarPoint>();
            for (var index = 0; index < points.Count; index++)
            {
                var point = points[index];
                if (point == null)
                    continue;

                var found = false;
                for (int i = index + 1; i < points.Count && i < 5; i++)
                {
                    var pointNext = points[i];
                    if (pointNext != null && Math.Abs(pointNext.Distance - point.Distance) < 300)
                    {
                        result.Add(new LidarPoint()
                        {
                            Angle = point.Angle,
                            Distance = point.Distance
                        });
                        Console.WriteLine($"Backward found near point: {point.ToString()}, {pointNext}");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    for (int i = index - 1; i > -5; i--)
                    {
                        if (i < 0)
                            i = 360 + i;
                        if (points.Count >= i)
                            continue;
                        var pointNext = points[i];
                        if (pointNext != null && Math.Abs(pointNext.Distance - point.Distance) < 300)
                        {
                            result.Add(new LidarPoint() { Angle = point.Angle, Distance = point.Distance });
                            Console.WriteLine($"Backward found near point: {point.ToString()}, {pointNext}");
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    Console.WriteLine($"Filtered point: {point.ToString()}");
                }
            }

            return result;
        }

        /// <summary>
        /// Add range.
        /// </summary>
        /// <param name="points">The points.</param>
        public void AddRange(IEnumerable<LidarPoint> points)
        {
            foreach (var lidarPoint in points)
            {
                Add(lidarPoint);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<LidarPointGroupItem> Items => _dictionary.Values;

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns><![CDATA[IEnumerator<LidarPointGroupItem>]]></returns>
        public IEnumerator<LidarPointGroupItem> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}