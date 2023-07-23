// using System.Text.Json.Serialization;

// namespace Checkers
// {
//     public class BoardDTO
//     {
//         public List<PieceDTO> Pieces { get; set; }
//         [JsonConverter(typeof(PieceArrayConverter))]
//         public Piece[,] PiecesArray { get; set; }
//         public String CurrentTurnColor { get; set; }
//         public int CurrentTurnCount { get; set; }
//         public List<MovementDTO> CurrentTurnMovements { get; set; }

//         public BoardDTO(Board? board = null)
//         {
//             if (board == null) board = new Board();

//             (CurrentTurnColor, CurrentTurnCount) = (board.CurrentTurnColor.ToString(), board.CurrentTurnCount);

//             PiecesArray = board.Spaces;

//             Pieces = new List<PieceDTO>();

//             for (int y = 0; y < 8; y++)
//             {
//                 for (int x = 0; x < 8; x++)
//                 {
//                     Piece piece = board.Spaces[x, y];
//                     if (!piece.IsNone())
//                     {
//                         Pieces.Add(new PieceDTO(x, y, piece));
//                     }
//                 }
//             }

//             CurrentTurnMovements = new List<MovementDTO>();

//             foreach (Movement movement in board.CurrentTurnMovements)
//             {
//                 CurrentTurnMovements.Add(new MovementDTO(movement));
//             }
//         }
//     }
// }