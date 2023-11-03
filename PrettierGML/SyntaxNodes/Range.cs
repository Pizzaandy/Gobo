namespace PrettierGML.SyntaxNodes
{
    public readonly struct Range
    {
        public static readonly Range Invalid = new(-1, -2);

        /// <summary>
        /// The start of the interval (inclusive).
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// The end of the interval (inclusive).
        /// </summary>
        public readonly int Stop;

        /// <summary>
        /// Return the number of elements between a and b inclusively.
        /// </summary>
        public readonly int Length
        {
            get
            {
                if (Stop < Start)
                {
                    return 0;
                }

                return Stop - Start + 1;
            }
        }

        public Range(int start, int stop)
        {
            Start = start;
            Stop = stop;
        }

        public bool Contains(Range other)
        {
            if (other.Start >= Start)
            {
                return other.Stop <= Stop;
            }

            return false;
        }

        public override readonly bool Equals(object? o)
        {
            if (o is not Range range)
            {
                return false;
            }

            if (Start == range.Start)
            {
                return Stop == range.Stop;
            }

            return false;
        }

        public override string ToString()
        {
            return $"({Start}, {Stop})";
        }

        public override readonly int GetHashCode()
        {
            return (23 * 31 + Start) * 31 + Stop;
        }

        public static bool operator ==(Range left, Range right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range left, Range right)
        {
            return !(left == right);
        }
    }
}
