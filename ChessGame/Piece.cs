using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using BoardPosition = System.Collections.Generic.KeyValuePair<int, int>;

namespace ChessGame
{
    public enum PieceColor { White, Black };
    public enum PieceType
    {
        King,
        Bishop,
        Knight,
        Rook,
        Queen,
        Pawn
    }

    class Resources
    {
        public static string IMAGE_KING_WHITE = @"images\king_white.png";
        public static string IMAGE_BISHOP_WHITE = @"images\bishop_white.png";
        public static string IMAGE_KNIGHT_WHITE = @"images\knight_white.png";
        public static string IMAGE_ROOK_WHITE = @"images\rook_white.png";
        public static string IMAGE_QUEEN_WHITE = @"images\queen_white.png";
        public static string IMAGE_PAWN_WHITE = @"images\pawn_white.png";

        public static string IMAGE_KING_BLACK = @"images\king_black.png";
        public static string IMAGE_BISHOP_BLACK = @"images\bishop_black.png";
        public static string IMAGE_KNIGHT_BLACK = @"images\knight_black.png";
        public static string IMAGE_ROOK_BLACK = @"images\rook_black.png";
        public static string IMAGE_QUEEN_BLACK = @"images\queen_black.png";
        public static string IMAGE_PAWN_BLACK = @"images\pawn_black.png";
    }

    class Piece : PictureBox
    {
        private PieceColor _color;
        private string _imageLink;
        private BoardPosition _currentPosition;
        private PieceType _type;

        public PieceColor Color { get => _color; set => _color = value; }
        public string ImageLink { get => _imageLink; set => _imageLink = value; }
        public BoardPosition CurrentPosition 
        { 
            get => _currentPosition;
            set
            {
                _currentPosition = value;
            }
        }
        public PieceType Type { get => _type; set => _type = value; }
        public Piece(Square sq, PieceColor color)
        {
            _currentPosition = sq.PositionOnBoard;
            _color = color;
        }
        public bool HasSameColor(Piece otherPiece)
        {
            if (otherPiece == null)
                return false;

            return this.Color == otherPiece.Color;
        }
    }

    class King : Piece
    {
        public King(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_KING_WHITE;
            else
                ImageLink = Resources.IMAGE_KING_BLACK;

            Type = PieceType.King;
        }
    }
    class Queen : Piece
    {
        public Queen(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_QUEEN_WHITE;
            else
                ImageLink = Resources.IMAGE_QUEEN_BLACK;

            Type = PieceType.Queen;
        }
    }
    class Pawn : Piece
    {
        public Pawn(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_PAWN_WHITE;
            else
                ImageLink = Resources.IMAGE_PAWN_BLACK;

            Type = PieceType.Pawn;
        }
    }
    class Rook : Piece
    {
        public Rook(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_ROOK_WHITE;
            else
                ImageLink = Resources.IMAGE_ROOK_BLACK;

            Type = PieceType.Rook;
        }
    }
    class Knight : Piece
    {
        public Knight(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_KNIGHT_WHITE;
            else
                ImageLink = Resources.IMAGE_KNIGHT_BLACK;

            Type = PieceType.Knight;
        }
    }
    class Bishop : Piece
    {
        public Bishop(Square sq, PieceColor color): base(sq, color)
        {
            if (color == PieceColor.White)
                ImageLink = Resources.IMAGE_BISHOP_WHITE;
            else
                ImageLink = Resources.IMAGE_BISHOP_BLACK;

            Type = PieceType.Bishop;
        }
    }
}
