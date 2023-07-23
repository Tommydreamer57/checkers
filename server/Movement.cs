using System.ComponentModel;
using System.Diagnostics;

namespace Checkers
{
    public class Movement
    {
        public (int X, int Y) Start { get; }
        public (int X, int Y) Delta { get; }
        public (int X, int Y) End { get; }

        public Movement((int X, int Y) start, (int X, int Y) delta)
        {
            Start = start;
            Delta = delta;
            End = (Start.X + Delta.X, Start.Y + Delta.Y);

            if (
                (Start.X < 0) || (Start.X > 7)
                ||
                (Start.Y < 0) || (Start.Y > 7)
            )
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Starting coordinates must be between 0 and 7");
            }

            if (
                (End.X < 0) || (End.X > 7)
                ||
                (End.Y < 0) || (End.Y > 7)
            )
            {
                throw new ArgumentException("Cannot move " + Delta.ToString() + " from " + Start.ToString(), nameof(delta));
            }

            if (Math.Abs(Delta.X) != Math.Abs(Delta.Y))
            {
                throw new ArgumentException("Movements must be perfectly diagonal", nameof(delta));
            }

            if (Math.Abs(Delta.X) > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(Delta), "Cannot move more than 2 spaces away");
            }
        }
        public bool IsJump()
        {
            return Math.Abs(Delta.X) == 2;
        }

        public (int X, int Y)? GetJumpedDelta()
        {
            if (IsJump())
            {
                return (Delta.X / 2, Delta.Y / 2);
            }

            return null;
        }

        public (int X, int Y)? GetJumpedCoordinates()
        {
            var jumpedDelta = GetJumpedDelta();

            if (jumpedDelta.HasValue)
            {
                return (Start.X + jumpedDelta.Value.X, Start.Y + jumpedDelta.Value.Y);
            }

            return null;
        }

        public override string ToString()
        {
            return "(" + Start.ToString() + "=>" + End.ToString() + ")";
        }
    }
}