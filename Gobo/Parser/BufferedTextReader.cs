namespace Gobo.Parser;

internal class BufferedTextReader
{
    private readonly TextReader textReader;
    private readonly RingBuffer buffer;

    public BufferedTextReader(TextReader textReader, int bufferSize)
    {
        if (bufferSize < 1)
        {
            throw new ArgumentException("Buffer size must be at least 1.");
        }

        this.textReader = textReader;
        buffer = new RingBuffer(bufferSize);

        for (var i = 0; i < bufferSize - 1; i++)
        {
            buffer.Enqueue(textReader.Read());
        }
    }

    public int LookAhead(int offset)
    {
        return buffer[offset];
    }

    public int Read()
    {
        buffer.Enqueue(textReader.Read());
        return buffer[0];
    }
}

internal class RingBuffer
{
    private readonly int[] buffer;
    private readonly int capacity;
    private int count;
    private int head;

    public RingBuffer(int capacity)
    {
        this.capacity = capacity;
        buffer = new int[capacity];
        count = 0;
        head = 0;
    }

    public int Count => count;

    public void Enqueue(int value)
    {
        if (count < capacity)
        {
            buffer[count] = value;
            count++;
        }
        else
        {
            buffer[head] = value;
            head = (head + 1) % capacity;
        }
    }

    public int this[int index]
    {
        get
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("Index is out of range");
            }

            int bufferIndex = (head + index) % capacity;
            return buffer[bufferIndex];
        }
    }
}
