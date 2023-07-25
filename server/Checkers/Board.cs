using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Checkers
{
    public class Board
    {
        [JsonConverter(typeof(PieceArrayConverter))]
        public Piece[,] Pieces { get; private set; }
        // public List<((int X, int Y), Piece)> Pieces { get { return GetAllPieces(); } }
        public PieceColor CurrentTurnColor { get; set; }
        public int CurrentTurnCount { get; set; }
        public List<Movement> CurrentTurnMovements { get; set; }

        public Board(
            List<((int X, int Y), Piece)>? initialPieces = null,
            PieceColor currentTurnColor = PieceColor.Red,
            int currentTurnCount = 0,
            List<Movement>? currentTurnMovements = null)
        {
            Pieces = new Piece[8, 8];
            CurrentTurnColor = currentTurnColor;
            CurrentTurnCount = currentTurnCount;
            if (currentTurnMovements == null) CurrentTurnMovements = new List<Movement>();
            else CurrentTurnMovements = currentTurnMovements;
            if (initialPieces == null) Initialize();
            else Initialize(initialPieces);
        }

        private void Initialize(List<((int X, int Y) coordinates, Piece piece)> initialPieces)
        {
            foreach (((int X, int Y) coordinates, Piece piece) initialPiece in initialPieces)
            {
                var coordinates = initialPiece.coordinates;
                var piece = initialPiece.piece;

                SetCoordinates(coordinates, piece);
            }
        }
        private void Initialize()
        {
            for (int y = 0; y < 8; y++)
            {
                bool yEven = y % 2 == 0;

                PieceColor color = PieceColor.None;
                PieceType type = PieceType.None;

                // bottom 3 rows are red
                if (y < 3) color = PieceColor.Red;
                // top 3 rows are black
                else if (y > 4) color = PieceColor.Black;
                // 2 middle rows are empty

                if (color != PieceColor.None) type = PieceType.Regular;

                for (int x = 0; x < 8; x++)
                {
                    bool xEven = x % 2 == 0;

                    // even rows (y) have even column values (x)
                    // odd rows have odd columns
                    if (xEven == yEven) SetCoordinates(x, y, new Piece(color, type));
                    else ClearCoordinates(x, y);
                }
            }
        }

        public bool CoordinateIsValid(int n) => (n < 8) && (n >= 0);
        public bool CoordinateIsValid(int x, int y) => CoordinateIsValid(x) && CoordinateIsValid(y);
        public bool CoordinateIsValid((int X, int Y) coord) => CoordinateIsValid(coord.X, coord.Y);

        public Piece GetPiece((int X, int Y) coordinates) { return GetPiece(coordinates.X, coordinates.Y); }
        public Piece GetPiece(int x, int y)
        {
            if (!CoordinateIsValid(x, y)) return new Piece();
            var piece = Pieces[x, y];
            if (piece == null) return new Piece();
            else return piece;
        }

        public PieceColor GetPieceColor((int X, int Y) coordinates) { return GetPieceColor(coordinates.X, coordinates.Y); }
        public PieceColor GetPieceColor(int x, int y)
        {
            var piece = GetPiece(x, y);
            return piece.GetPieceColor();
        }

        public PieceType GetPieceType((int X, int Y) coordinates) { return GetPieceType(coordinates.X, coordinates.Y); }
        public PieceType GetPieceType(int x, int y)
        {
            var piece = GetPiece(x, y);
            return piece.GetPieceType();
        }

        public bool MovementIsValid(Movement movement)
        {
            Piece piece = GetPiece(movement.Start);

            if (piece.Color != CurrentTurnColor) return false;

            Piece blockingPiece = GetPiece(movement.End);

            if (!blockingPiece.IsNone()) return false;

            (int X, int Y)? jumpedCoordinates = movement.GetJumpedCoordinates();

            if (jumpedCoordinates != null)
            {
                PieceColor jumpedColor = GetPieceColor(jumpedCoordinates.Value);

                if (!piece.IsOppositeColor(jumpedColor)) return false;
            }

            return true;
        }

        public List<Movement> GetValidMovements(int x, int y) { return GetValidMovements((x, y)); }
        public List<Movement> GetValidMovements((int X, int Y) start)
        {

            Piece piece = GetPiece(start);

            if (piece.IsNone()) return new List<Movement>();

            PieceColor color = piece.GetPieceColor();
            bool isKing = piece.IsKing();

            List<Movement> result = new();

            if (color == PieceColor.None) return result;

            List<(int X, int Y)> deltas = new();

            // Black moves down (negative y)
            if (isKing || (color == PieceColor.Black))
            {
                deltas.Add((-1, -1));
                deltas.Add((1, -1));
            }
            // Red moves up (positive y)
            if (isKing || (color == PieceColor.Red))
            {
                deltas.Add((-1, 1));
                deltas.Add((1, 1));
            }

            foreach ((int X, int Y) in deltas)
            {
                int dx = X;
                int dy = Y;

                int ex = start.X + dx;
                int ey = start.Y + dy;

                Piece otherPiece = GetPiece(ex, ey);
                bool isOpposite = piece.IsOppositeColor(otherPiece);

                if (isOpposite)
                {
                    dx *= 2;
                    dy *= 2;

                    ex = start.X + dx;
                    ey = start.Y + dy;

                    otherPiece = GetPiece(ex, ey);
                }

                if (otherPiece.IsNone() && CoordinateIsValid(ex, ey))
                {
                    (int X, int Y) finalDelta = new(dx, dy);

                    var movement = new Movement(start, finalDelta);

                    if (MovementIsValid(movement)) result.Add(movement);
                }
            }

            return result;
        }

        private void ClearCoordinates((int X, int Y) coordinates) { ClearCoordinates(coordinates.X, coordinates.Y); }
        private void ClearCoordinates(int x, int y)
        {
            Pieces[x, y] = new Piece(PieceColor.None, PieceType.None);
        }

        private void SetCoordinates((int X, int Y) coordinates, Piece piece) { SetCoordinates(coordinates.X, coordinates.Y, piece); }
        private void SetCoordinates(int x, int y, Piece piece)
        {
            Pieces[x, y] = piece;
        }

        public Board ApplyMovement(Movement movement)
        {
            if (!MovementIsValid(movement))
            {
                throw new InvalidOperationException("Movement not allowed: " + movement.ToString());
            }
            else
            {
                Piece piece = GetPiece(movement.Start);

                PieceColor color = piece.GetPieceColor();

                var jumpedCoordinates = movement.GetJumpedCoordinates();

                if (jumpedCoordinates != null)
                {
                    ClearCoordinates(jumpedCoordinates.Value);
                }

                ClearCoordinates(movement.Start);

                SetCoordinates(movement.End, piece);

                if (
                    !piece.IsKing()
                    &&
                    (
                        ((color == PieceColor.Black) && (movement.End.Y == 0))
                        ||
                        ((color == PieceColor.Red) && (movement.End.Y == 7))
                    )
                )
                {
                    piece.ConvertToKing();
                }

                CurrentTurnMovements.Add(movement);

                if (!CanKeepMoving()) SwitchTurns();
            }

            return this;
        }

        public bool CanKeepMoving()
        {
            Movement? latestMovement = CurrentTurnMovements.Last();

            if ((latestMovement == null) || !latestMovement.IsJump()) return false;

            List<Movement> validMovements = GetValidMovements(latestMovement.End);

            foreach (Movement movement in validMovements)
            {
                if (movement.IsJump()) return true;
            }

            return false;
        }

        public PieceColor SwitchTurns()
        {
            if (CurrentTurnMovements.Count == 0)
            {
                throw new InvalidOperationException("Cannot switch turns before making a movement");
            }

            if (CurrentTurnColor == PieceColor.Red) CurrentTurnColor = PieceColor.Black;
            else CurrentTurnColor = PieceColor.Red;

            CurrentTurnCount++;

            CurrentTurnMovements = new List<Movement>();

            return CurrentTurnColor;
        }

        public PieceColor GetWinner()
        {
            var winner = PieceColor.None;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var color = GetPieceColor(x, y);
                    if (winner == PieceColor.None)
                    {
                        winner = color;
                    }
                    else if ((color != PieceColor.None) && (color != winner))
                    {
                        return PieceColor.None;
                    }
                }
            }
            return winner;
        }

        public bool Finished()
        {
            var winner = GetWinner();
            return (winner != PieceColor.None);
        }

        public List<((int X, int Y), Piece)> GetAllPieces()
        {
            List<((int X, int Y), Piece)> result = new();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = GetPiece(x, y);
                    if (!piece.IsNone())
                    {
                        result.Add(((x, y), piece));
                    }
                }
            }

            return result;
        }

        public void Print()
        {
            for (int y = 7; y >= 0; y--)
            {
                bool yEven = y % 2 == 0;
                string line = "" + y + "| ";
                for (int x = 0; x < 8; x++)
                {
                    bool xEven = x % 2 == 0;
                    var color = GetPieceColor(x, y);
                    if (color == PieceColor.Black) line += "X ";
                    else if (color == PieceColor.Red) line += "O ";
                    else if (yEven == xEven) line += "* ";
                    else line += "  ";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("-+----------------");
            Console.WriteLine(" | 0 1 2 3 4 5 6 7");
        }
    }
}