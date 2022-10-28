using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using BoardPosition = System.Collections.Generic.KeyValuePair<int, int>;

namespace ChessGame
{

    public enum SquareColor { White, Black};
    class Square : PictureBox
    {
        #region properties
        private bool _isSelected;
        private Board _parentBoard;
        private SquareColor _color;
        private BoardPosition _positionOnBoard;
        private Piece _piece;
        private int _xBoard;
        private int _yBoard;

        public bool IsSelected 
        { 
            get => _isSelected;
            set
            {
                _isSelected = value;

                if (value == true)
                {
                    _parentBoard.SelectingSquare = this;

                    if (this.Color == SquareColor.White)
                        this.BackColor = System.Drawing.Color.LightCyan;
                    else
                        this.BackColor = System.Drawing.ColorTranslator.FromHtml("#DAE9E9");
                }
                else
                {
                    Color = _color;
                    ParentBoard.UnshowCanMovePath();
                    ParentBoard.SelectingSquare = null;
                }
            }
        }
        public bool HasPiece 
        {
            get
            {
                if (Image == null)
                    return false;
                return true;
            }
        }
        public SquareColor Color
        {
            get => _color;
            set
            {
                if (value == SquareColor.White)
                {
                    _color = SquareColor.White;
                    this.BackColor = Board.squareColor_White;
                }
                else
                {
                    _color = SquareColor.Black;
                    this.BackColor = Board.squareColor_Black;
                }
            }
        }
        public Piece OwnPiece
        { 
            get => _piece;
            set
            {
                _piece = value;
                if (value != null)
                {
                    Image = Image.FromFile(_piece.ImageLink);
                }
            }
        }
        public Board ParentBoard { get => _parentBoard; set => _parentBoard = value; }
        public int YBoard { get => _xBoard; set => _xBoard = value; }
        public int XBoard { get => _yBoard; set => _yBoard = value; }
        public BoardPosition PositionOnBoard { get => _positionOnBoard; set => _positionOnBoard = value; }
        #endregion


        #region constructor
        public Square(Board board) 
        {
            this.ParentBoard = board;
            this.SizeMode = PictureBoxSizeMode.StretchImage;

            this.MouseClick += new MouseEventHandler(Square_Click);
        }
        #endregion

        private void Square_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Square currentSquare = sender as Square;
                Board b = currentSquare.ParentBoard;

                if (!HasSelectedASquare(b) && currentSquare.HasPiece)
                {
                    if ((IsFirstTurn(b) && currentSquare.OwnPiece.Color == b.FirstMoveColor) ||
                       (!IsFirstTurn(b) && currentSquare.OwnPiece.Color == b.ThisTurnColor))
                    {
                        currentSquare.IsSelected = true;
                        b.ShowCanMovePath();
                    }
                }
                else if (HasSelectedASquare(b))
                {
                    if (b.CanMove[XBoard, YBoard])
                    {
                        if (this.HasPiece)
                        {
                            if (this.OwnPiece.Type == PieceType.King)
                            {
                                MessageBox.Show($"Game Over! \nWinner: {b.ThisTurnColor}");
                                b.ParentForm.Close();
                                return;
                            }
                        }

                        b.SelectingSquare.OwnPiece.CurrentPosition = this.PositionOnBoard;
                        this.OwnPiece = b.SelectingSquare.OwnPiece;

                        b.SelectingSquare.OwnPiece = null;
                        b.SelectingSquare.Image = null;
                        b.SelectingSquare.IsSelected = false;
                        ChangeTurn(b);
                    }
                    else if (this.HasPiece)
                    {
                        if (this.OwnPiece.HasSameColor(b.SelectingSquare.OwnPiece))
                        {
                            b.SelectingSquare.IsSelected = false;
                            currentSquare.IsSelected = true;

                            b.ShowCanMovePath();
                        }
                    }
                    else
                    {
                        b.SelectingSquare.IsSelected = false;
                        currentSquare.IsSelected = false;
                    }
                }
            }
        }

        private static bool IsFirstTurn(Board b)
        {
            return b.Turn == 0;
        }

        private static bool HasSelectedASquare(Board b)
        {
            return b.SelectingSquare != null;
        }

        private static void ChangeTurn(Board b)
        {
            if (b.ThisTurnColor == PieceColor.White)
            {

                b.ThisTurnColor = PieceColor.Black;
            }
            else
                b.ThisTurnColor = PieceColor.White;


            if(b.ThisTurnColor == Board.TOP_BOARD_COLOR)
            {
                b.TopPlayerTimer.Stop();
                b.BottomPlayerTimer.Start();
            }
            else
            {
                b.BottomPlayerTimer.Stop();
                b.TopPlayerTimer.Start();
            }

            b.Turn++;
        }
    }
}
