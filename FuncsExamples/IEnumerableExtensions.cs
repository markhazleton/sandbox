
namespace FuncsExamples;
public static class IEnumerableExtensions
{
    public static void TestFlatten()
    {
        var testObject = new TestObject
        {
            ChildObjects = new List<TestObject>
            {
                new TestObject(),
                new TestObject
                {
                    ChildObjects = new List<TestObject>
                    {
                        new TestObject(),
                        new TestObject(),
                        new TestObject
                        {
                            ChildObjects = new List<TestObject>
                            {
                                new TestObject(),
                                new TestObject(),
                                new TestObject()
                            }
                        }
                    }
                },
                new TestObject
                {
                    ChildObjects = new List<TestObject>
                    {
                        new TestObject(),
                        new TestObject(),
                        new TestObject
                        {
                            ChildObjects = new List<TestObject>
                            {
                                new TestObject(),
                                new TestObject(),
                                new TestObject()
                            }
                        }
                    }
                }
            }
        };
        var flattened = testObject.ChildObjects.Flatten(x => x.ChildObjects) ?? new List<TestObject>();
        flattened?.ToList().ForEach(Console.WriteLine);
    }

    private class TestObject
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public List<TestObject> ChildObjects { get; set; } = new List<TestObject>();

        public override string ToString()
        {
            return $"ID:{ID} with {ChildObjects.Count} children";
        }
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
    {
        if (source == null) yield break;
        var queue = new Queue<T>(source);
        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            yield return item;
            if (item == null) { continue; }
            var enumerable = selector(item);
            if (enumerable == null) { continue; }
            enumerable.ToList().ForEach(queue.Enqueue);
        }
    }
}
