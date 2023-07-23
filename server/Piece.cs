using System.Diagnostics;

namespace Checkers
{
    public enum PieceColor
    {
        None,
        Red,
        Black
    }
    public enum PieceType
    {
        None,
        Regular,
        King
    }
    public class Piece
    {
        public PieceColor Color { get; private set; }
        public PieceType Type { get; private set; }

        public Piece(PieceColor color = PieceColor.None, PieceType type = PieceType.None)
        {
            Color = color;
            Type = type;

            if ((color == PieceColor.None) && (type != PieceType.None))
            {
                throw new ArgumentException("Cannot mix PieceColor None with PieceType not None");
            }

            if ((color != PieceColor.None) && (type == PieceType.None))
            {
                throw new ArgumentException("Cannot mix PieceColor not None with PieceType None");
            }

        }
        public PieceColor GetPieceColor()
        {
            return Color;
        }
        public PieceType GetPieceType()
        {
            return Type;
        }
        public bool IsKing()
        {
            return Type == PieceType.King;
        }
        public bool IsNone()
        {
            return Type == PieceType.None;
        }
        public void ConvertToKing()
        {
            Type = PieceType.King;
        }
        public bool IsOppositeColor(PieceColor? otherColor)
        {
            return (
                (otherColor != null)
                &&
                !IsNone()
                &&
                !(otherColor == PieceColor.None)
                &&
                (otherColor != Color)
            );
        }
        public bool IsOppositeColor(Piece? otherPiece)
        {
            if (otherPiece == null)
            {
                return false;
            }
            else
            {
                return IsOppositeColor(otherPiece.GetPieceColor());
            }
        }
    }
}