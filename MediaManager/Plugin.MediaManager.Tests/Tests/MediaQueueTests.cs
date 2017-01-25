using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Tests.Tests
{
    [TestFixture]
    public class MediaQueueTests
    {
        /// <summary>
        /// Checking the initial setup of an instance.
        /// </summary>
        [Test]
        public void Initialization()
        {
            var queue = new MediaQueue();

            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(null, queue.Current);
            Assert.AreEqual(-1, queue.Index);
            Assert.AreEqual(RepeatType.None, queue.Repeat);
            Assert.AreEqual(false, queue.IsShuffled);
        }

        [TestFixture]
        private class Add
        {
            /// <summary>
            /// Checks adding an item, including all it's triggers.
            /// </summary>
            [Test]
            public void Basic()
            {
                var queue = new MediaQueue();

                IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
                IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

                queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
                queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

                var tracks = new[]
                    {
                        new MediaFile(),
                        new MediaFile(),
                    };

                queue.Add(tracks[0]);

                Assert.AreEqual(1, queue.Count);
                Assert.AreEqual(tracks[0], queue.Current);
                Assert.AreEqual(0, queue.Index);
                Assert.AreEqual(RepeatType.None, queue.Repeat);
                Assert.AreEqual(false, queue.IsShuffled);

                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
                Assert.AreEqual(3, propertyChangedEvents.Count);
                Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems.Count == 1 && e.OldItems == null).Count());
                Assert.AreEqual(1, collectionChangedEvents.Count);

                queue.Add(tracks[1]);

                Assert.AreEqual(2, queue.Count);
                Assert.AreEqual(tracks[0], queue.Current);
                Assert.AreEqual(0, queue.Index);
                Assert.AreEqual(RepeatType.None, queue.Repeat);
                Assert.AreEqual(false, queue.IsShuffled);

                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
                Assert.AreEqual(2, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
                Assert.AreEqual(4, propertyChangedEvents.Count);
                Assert.AreEqual(2, collectionChangedEvents.Where(e => e.NewItems.Count == 1 && e.OldItems == null).Count());
                Assert.AreEqual(2, collectionChangedEvents.Count);
            }
        }

        [TestFixture]
        private class Clear
        {
            /// <summary>
            /// Verify that clearing a view calls all the necessary triggers
            /// </summary>
            [Test]
            public void Basic()
            {
                var queue = new MediaQueue();

                var tracks = new[]
                    {
                        new MediaFile(),
                        new MediaFile(),
                    };

                queue.AddRange(tracks);

                IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
                IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

                queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
                queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

                queue.Clear();

                Assert.AreEqual(0, queue.Count);
                Assert.AreEqual(null, queue.Current);
                Assert.AreEqual(-1, queue.Index);
                Assert.AreEqual(RepeatType.None, queue.Repeat);
                Assert.AreEqual(false, queue.IsShuffled);

                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
                Assert.AreEqual(3, propertyChangedEvents.Count);
                Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems == null).Count());
                Assert.AreEqual(1, collectionChangedEvents.Count);
            }

            /// <summary>
            /// Verify that Shuffle is reset after clearing the queue and that Repeat property is left untouched.
            /// </summary>
            [Test]
            public void ShuffledAndRepeated()
            {
                var queue = new MediaQueue();

                var tracks = new[]
                    {
                        new MediaFile(),
                        new MediaFile(),
                    };

                queue.AddRange(tracks);
                queue.IsShuffled = true;
                queue.Repeat = RepeatType.RepeatOne;

                IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
                IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

                queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
                queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

                queue.Clear();

                Assert.AreEqual(0, queue.Count);
                Assert.AreEqual(null, queue.Current);
                Assert.AreEqual(-1, queue.Index);
                Assert.AreEqual(RepeatType.RepeatOne, queue.Repeat);
                Assert.AreEqual(false, queue.IsShuffled);

                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
                Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Shuffle").Count());
                Assert.AreEqual(4, propertyChangedEvents.Count);
                Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems == null).Count());
                Assert.AreEqual(1, collectionChangedEvents.Count);
            }
        }

		[TestFixture]
		private class ArrayIndex
		{
			[Test]
			public void SetNotCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue[0] = new MediaFile();

				Assert.AreEqual(3, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems?.Count == 1 && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void SetCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				var newMediaFile = new MediaFile();
				queue[1] = newMediaFile;

				Assert.AreEqual(3, queue.Count);
				Assert.AreEqual(newMediaFile, queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(1, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems?.Count == 1 && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}
		
			[Test]
			public void SetOutOfRange()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					queue[3] = new MediaFile();
				});

				Assert.AreEqual(3, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}

			[Test]
			public void SetNull()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				Assert.Throws<ArgumentNullException>(() =>
				{
					queue[1] = null;
				});

				Assert.AreEqual(3, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}
		}

		[TestFixture]
		private class RemoveAndRemoveAt
		{
			[Test]
			public void RemoveItem_PostCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Remove(tracks[2]);

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(1, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void RemoveItem_PreCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Remove(tracks[0]);

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(0, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(2, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Remove_CurrentItem()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Remove(tracks[1]);

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[2], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(2, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Remove_CurrentAndLastItem()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(2);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Remove(tracks[2]);

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(3, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Remove_CurrentOnlyAndLastItem()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(0);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Remove(tracks[0]);

				Assert.AreEqual(0, queue.Count);
				Assert.AreEqual(null, queue.Current);
				Assert.AreEqual(-1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(3, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems == null && e.OldItems?.Count == 1).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void RemoveAt_OutOfRange()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
						new MediaFile(),
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(1);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					queue.RemoveAt(4);
				});

				Assert.AreEqual(3, queue.Count);
				Assert.AreEqual(tracks[1], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}

			[Test]
			public void RemoveAt_OutOfRange_Empty()
			{
				var queue = new MediaQueue();

                IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					queue.RemoveAt(-1);
				});

				Assert.AreEqual(0, queue.Count);
				Assert.AreEqual(null, queue.Current);
				Assert.AreEqual(-1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}
		}

		[TestFixture]
		private class Insert
		{
			[Test]
			public void Insert_PostCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(0);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Insert(1, new MediaFile());

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[0], queue.Current);
				Assert.AreEqual(0, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(1, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems?.Count == 1 && e.OldItems == null).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Insert_PreCurrent()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(0);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				queue.Insert(0, new MediaFile());

				Assert.AreEqual(2, queue.Count);
				Assert.AreEqual(tracks[0], queue.Current);
				Assert.AreEqual(1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(2, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems?.Count == 1 && e.OldItems == null).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Insert_Empty()
			{
				var queue = new MediaQueue();

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				var newMediaFile = new MediaFile();
				queue.Insert(0, newMediaFile);

				Assert.AreEqual(1, queue.Count);
				Assert.AreEqual(newMediaFile, queue.Current);
				Assert.AreEqual(0, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Current").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Index").Count());
				Assert.AreEqual(1, propertyChangedEvents.Where(e => e.PropertyName == "Count").Count());
				Assert.AreEqual(3, propertyChangedEvents.Count);
				Assert.AreEqual(1, collectionChangedEvents.Where(e => e.NewItems?.Count == 1 && e.OldItems == null).Count());
				Assert.AreEqual(1, collectionChangedEvents.Count);
			}

			[Test]
			public void Insert_OutOfRange_Empty()
			{
				var queue = new MediaQueue();

                IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				var newMediaFile = new MediaFile();

				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					queue.Insert(1, newMediaFile);
				});

				Assert.AreEqual(0, queue.Count);
				Assert.AreEqual(null, queue.Current);
				Assert.AreEqual(-1, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}

			[Test]
			public void Insert_OutOfRange()
			{
				var queue = new MediaQueue();

                var tracks = new[]
					{
						new MediaFile(),
					};

				queue.AddRange(tracks);
				queue.SetIndexAsCurrent(0);

				IList<NotifyCollectionChangedEventArgs> collectionChangedEvents = new List<NotifyCollectionChangedEventArgs>();
				IList<PropertyChangedEventArgs> propertyChangedEvents = new List<PropertyChangedEventArgs>();

				queue.CollectionChanged += (sender, e) => collectionChangedEvents.Add(e);
				queue.PropertyChanged += (sender, e) => propertyChangedEvents.Add(e);

				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					queue.Insert(2, new MediaFile());
				});

				Assert.AreEqual(1, queue.Count);
				Assert.AreEqual(tracks[0], queue.Current);
				Assert.AreEqual(0, queue.Index);
				Assert.AreEqual(RepeatType.None, queue.Repeat);
				Assert.AreEqual(false, queue.IsShuffled);

				Assert.AreEqual(0, propertyChangedEvents.Count);
				Assert.AreEqual(0, collectionChangedEvents.Count);
			}
		}

        [TestFixture]
        private class ToggleShuffle
        {
            [Test]
            public void WhenShufflingWorks_OrderIsRandomized()
            {
                var arr = new[]
                              {
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile()
                              };

                Console.WriteLine("Original: {0}", string.Join(",", arr.Select(x => x.Id)));

                var queue = new MediaQueue();
                Console.WriteLine("Current Index: {0}", queue.Index);
                queue.AddRange(arr);

                queue.IsShuffled = true;

                Console.WriteLine("Result: {0}", string.Join(",", queue.Cast<MediaFile>().Select(x => x.Id)));

                CollectionAssert.AllItemsAreUnique(queue.Cast<MediaFile>().Select(x => x.Id));
                CollectionAssert.AreEquivalent(queue.Cast<MediaFile>().Select(x => x.Id), arr.Select(x => x.Id));
                Assert.That(queue.Cast<MediaFile>().Select(x => x.Id), Is.Not.Ordered);
                Assert.AreEqual(arr[queue.Index].Id, queue.Cast<MediaFile>().ElementAt(queue.Index).Id, "The current item has been moved");
                Assert.AreEqual(arr.Length, queue.Count, "The array length is different");
            }

            [Test]
            public void WhenUnshufflingWorks_OrderIsSameAsBefore()
            {
                var arr = new[]
                              {
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile()
                              };

                Console.WriteLine("Original: {0}", string.Join(",", arr.Select(x => x.Id)));

                var queue = new MediaQueue();
                Console.WriteLine("Current Index: {0}", queue.Index);
                queue.AddRange(arr);

                queue.IsShuffled = true;

                Console.WriteLine("Shuffled: {0}", string.Join(",", queue.Cast<MediaFile>().Select(x => x.Id)));

                queue.IsShuffled = false;

                Console.WriteLine("Unshuffled: {0}", string.Join(",", queue.Cast<MediaFile>().Select(x => x.Id)));

                CollectionAssert.AllItemsAreUnique(queue.Cast<MediaFile>().Select(x => x.Id));
                CollectionAssert.AreEqual(queue.Cast<MediaFile>().Select(x => x.Id), arr.Cast<MediaFile>().Select(x => x.Id));
                Assert.AreEqual(arr[queue.Index].Id, queue.Cast<MediaFile>().ElementAt(queue.Index).Id, "The current item has been moved");
                Assert.AreEqual(arr.Length, queue.Count, "The array length is different");
            }

            [Test]
            public void WhenAddingItemsWhileShuffled_OrderIsSameAsBeforeButWithExtraItem()
            {
                var arr = new[]
                              {
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile()
                              };

                Console.WriteLine("Original: {0}", string.Join(",", arr.Select(x => x.Id)));

                var queue = new MediaQueue();
                Console.WriteLine("Current Index: {0}", queue.Index);
                queue.AddRange(arr);

                queue.IsShuffled = true;
                queue.Add(new MediaFile());

                Console.WriteLine("Shuffled: {0}", string.Join(",", queue.Cast<MediaFile>().Select(x => x.Id)));

                queue.IsShuffled = false;

                Console.WriteLine("Unshuffled: {0}", string.Join(",", queue.Cast<MediaFile>().Select(x => x.Id)));

                CollectionAssert.AllItemsAreUnique(queue.Cast<MediaFile>().Select(x => x.Id));
                Assert.AreEqual(arr[0].Id, queue.Cast<MediaFile>().ElementAt(0).Id);
                Assert.AreEqual(arr[1].Id, queue.Cast<MediaFile>().ElementAt(1).Id);
                Assert.AreEqual(arr[2].Id, queue.Cast<MediaFile>().ElementAt(2).Id);
                Assert.AreEqual(arr[3].Id, queue.Cast<MediaFile>().ElementAt(3).Id);
                Assert.AreEqual(arr[4].Id, queue.Cast<MediaFile>().ElementAt(4).Id);
                Assert.AreEqual(arr[5].Id, queue.Cast<MediaFile>().ElementAt(5).Id);
                Assert.AreEqual(arr[6].Id, queue.Cast<MediaFile>().ElementAt(6).Id);
                Assert.AreEqual(arr[7].Id, queue.Cast<MediaFile>().ElementAt(7).Id);
                Assert.AreEqual(arr[8].Id, queue.Cast<MediaFile>().ElementAt(8).Id);
                Assert.AreEqual(arr[9].Id, queue.Cast<MediaFile>().ElementAt(9).Id);
                Assert.AreEqual(arr[10].Id, queue.Cast<MediaFile>().ElementAt(10).Id);
                Assert.AreEqual(arr.Length + 1, queue.Count, "The array length is different");
            }

            [Test]
            public void ShuffleTwice_WhenRandomNumberGeneratorWorks_OrderIsDifferent()
            {
                var arr = new[]
                              {
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile(),
                                  new MediaFile()
                              };

                Console.WriteLine("Original: {0}", string.Join(",", arr.Select(x => x.Id)));

                var queue1 = new MediaQueue();
                queue1.AddRange(arr);

                var queue2 = new MediaQueue();
                queue2.AddRange(arr);

                // Randomize in quick succession to prove that the result is not the same
                // Using Random() here would create two equal lists, hence the use of PCLCrypto.
                queue1.IsShuffled = true;
                queue2.IsShuffled = true;

                Console.WriteLine("Queue1: {0}", string.Join(",", queue1.Cast<MediaFile>().Select(x => x.Id)));
                Console.WriteLine("Queue2: {0}", string.Join(",", queue2.Cast<MediaFile>().Select(x => x.Id)));

                CollectionAssert.AllItemsAreUnique(queue1.Cast<MediaFile>().Select(x => x.Id));
                CollectionAssert.AllItemsAreUnique(queue2.Cast<MediaFile>().Select(x => x.Id));
                CollectionAssert.AreNotEqual(queue1.Cast<MediaFile>(), queue2.Cast<MediaFile>());
                Assert.AreEqual(arr[queue1.Index].Id, queue1.Cast<MediaFile>().ElementAt(queue1.Index).Id, "The current item has been moved");
                Assert.AreEqual(arr[queue2.Index].Id, queue2.Cast<MediaFile>().ElementAt(queue2.Index).Id, "The current item has been moved");
            }
        }
    }
}
