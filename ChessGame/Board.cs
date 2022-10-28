using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using BoardPosition = System.Collections.Generic.KeyValuePair<int, int>;
using SysColor = System.Drawing.Color;
using HexColor = System.Drawing.ColorTranslator;

namespace ChessGame
{
    class Board
    {
        #region constant
        private const int DEFAULT_BOARD_HEIGHT = 8;
        private const int DEFAULT_BOARD_WIDTH = 8;
        private const int DEFAULT_SQUARE_WIDTH = 70;
        private const int DEFAULT_SQUARE_HEIGHT = 70;
        private const int MINIMUM_COL = 0;
        private const int MINIMUM_ROW = 0;
        private const int DEFAULT_PLAY_TIME = 10 * 60; // 10 minutes
        public static PieceColor TOP_BOARD_COLOR = PieceColor.Black;
        #endregion

        
        #region Color
        private SysColor _canMoveColor_White = HexColor.FromHtml("#E0FFFF");
        private SysColor _canMoveColor_Black = HexColor.FromHtml("#DAE9E9");
        private SysColor _canKillColor_White = HexColor.FromHtml("#F25A5A");
        private SysColor _canKillColor_Black = HexColor.FromHtml("#E39797");
        public static SysColor squareColor_White = HexColor.FromHtml("#FFFFFF");
        public static SysColor squareColor_Black = HexColor.FromHtml("#D3D3D3");
        #endregion


        #region properties
        private Form _parentForm;
        private Square[,] _boardView = new Square[DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT];
        private int _topPlayerTime = DEFAULT_PLAY_TIME;
        private int _bottomPlayerTime = DEFAULT_PLAY_TIME;
        private Label _topPlayerTimerView;
        private Label _bottomPlayerTimerView;
        private Timer _topPlayerTimer;
        private Timer _bottomPlayerTimer;
        private Button _startButton;

        private bool[,] _canMove = new bool[DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT];
        private PieceColor _firstMoveColor = PieceColor.Black;
        private PieceColor _thisTurnColor = PieceColor.Black;
        private int _turn = 0;

        private Piece[,] _pieces = new Piece[4, 8];
        private Square _selectingSquare;

        public Square SelectingSquare 
        { 
            get => _selectingSquare;
            set
            {
                _selectingSquare = value;
            }
        }

        public bool[,] CanMove { get => _canMove; set => _canMove = value; }
        public Form ParentForm { get => _parentForm; set => _parentForm = value; }
        public PieceColor FirstMoveColor { get => _firstMoveColor; }
        public PieceColor ThisTurnColor { get => _thisTurnColor; set => _thisTurnColor = value; }
        public int Turn { get => _turn; set => _turn = value; }
        public Timer TopPlayerTimer { get => _topPlayerTimer; }
        public Timer BottomPlayerTimer { get => _bottomPlayerTimer; }
        #endregion


        #region constructor
        public Board(Form f, int squareWidth = DEFAULT_SQUARE_WIDTH, int squareHeight = DEFAULT_SQUARE_HEIGHT)
        {
            _parentForm = f;

            int topBoardInit = (f.ClientRectangle.Height - DEFAULT_BOARD_HEIGHT * squareHeight) / 2,
                leftBoardInit = topBoardInit;

            int left;
            int top = topBoardInit;

            for (int i = 0; i < DEFAULT_BOARD_WIDTH; i++)
            {
                left = leftBoardInit;

                for (int j = 0; j < DEFAULT_BOARD_HEIGHT; j++)
                {
                    _boardView[i, j] = new Square(this)
                    {
                        YBoard = j,
                        XBoard = i,
                        Width = squareWidth,
                        Height = squareHeight,
                        Left = left,
                        Top = top,
                        PositionOnBoard = new BoardPosition(i, j),
                    };

                    if (IsWhiteSquare(i, j))
                        _boardView[i, j].Color = SquareColor.White;
                    else
                        _boardView[i, j].Color = SquareColor.Black;

                    left += squareWidth;

                    _parentForm.Controls.Add(_boardView[i, j]);
                }

                top += squareHeight;
            }

            ShowPieces();
            ShowStartButton(squareWidth, leftBoardInit);
            ShowTimerView(squareWidth, squareHeight, leftBoardInit, topBoardInit);
        }

        #endregion




        #region show controls
        
        #region show pieces
        private void ShowPieces()
        {
            ShowPawn();
            ShowRook();
            ShowKnight();
            ShowBishop();
            ShowQueen();
            ShowKing();
        }

        private void ShowKing()
        {
            _boardView[0, 4].Image = Image.FromFile(Resources.IMAGE_KING_BLACK);
            _boardView[7, 4].Image = Image.FromFile(Resources.IMAGE_KING_WHITE);

            _pieces[0, 4] = new King(_boardView[0, 4], PieceColor.Black);
            _pieces[3, 4] = new King(_boardView[7, 4], PieceColor.White);

            _boardView[0, 4].OwnPiece = _pieces[0, 4];
            _boardView[7, 4].OwnPiece = _pieces[3, 4];
        }

        private void ShowQueen()
        {
            _boardView[0, 3].Image = Image.FromFile(Resources.IMAGE_QUEEN_BLACK);
            _boardView[7, 3].Image = Image.FromFile(Resources.IMAGE_QUEEN_WHITE);

            _pieces[0, 3] = new Queen(_boardView[0, 3], PieceColor.Black);
            _pieces[3, 3] = new Queen(_boardView[7, 3], PieceColor.White);

            _boardView[0, 3].OwnPiece = _pieces[0, 3];
            _boardView[7, 3].OwnPiece = _pieces[3, 3];
        }
        private void ShowBishop()
        {
            _boardView[0, 2].Image = Image.FromFile(Resources.IMAGE_BISHOP_BLACK);
            _boardView[0, 5].Image = Image.FromFile(Resources.IMAGE_BISHOP_BLACK);
            _boardView[7, 2].Image = Image.FromFile(Resources.IMAGE_BISHOP_WHITE);
            _boardView[7, 5].Image = Image.FromFile(Resources.IMAGE_BISHOP_WHITE);

            _pieces[0, 2] = new Bishop(_boardView[0, 2], PieceColor.Black);
            _pieces[0, 5] = new Bishop(_boardView[0, 5], PieceColor.Black);
            _pieces[3, 2] = new Bishop(_boardView[7, 2], PieceColor.White);
            _pieces[3, 5] = new Bishop(_boardView[7, 5], PieceColor.White);

            _boardView[0, 2].OwnPiece = _pieces[0, 2];
            _boardView[0, 5].OwnPiece = _pieces[0, 5];
            _boardView[7, 2].OwnPiece = _pieces[3, 2];
            _boardView[7, 5].OwnPiece = _pieces[3, 5];
        }

        private void ShowKnight()
        {
            _boardView[0, 1].Image = Image.FromFile(Resources.IMAGE_KNIGHT_BLACK);
            _boardView[0, 6].Image = Image.FromFile(Resources.IMAGE_KNIGHT_BLACK);
            _boardView[7, 1].Image = Image.FromFile(Resources.IMAGE_KNIGHT_WHITE);
            _boardView[7, 6].Image = Image.FromFile(Resources.IMAGE_KNIGHT_WHITE);

            _pieces[0, 1] = new Knight(_boardView[0, 1], PieceColor.Black);
            _pieces[0, 6] = new Knight(_boardView[0, 7], PieceColor.Black);
            _pieces[3, 1] = new Knight(_boardView[7, 1], PieceColor.White);
            _pieces[3, 6] = new Knight(_boardView[7, 6], PieceColor.White);

            _boardView[0, 1].OwnPiece = _pieces[0, 1];
            _boardView[0, 6].OwnPiece = _pieces[0, 6];
            _boardView[7, 1].OwnPiece = _pieces[3, 1];
            _boardView[7, 6].OwnPiece = _pieces[3, 6];
        }

        private void ShowRook()
        {
            _boardView[0, 0].Image = Image.FromFile(Resources.IMAGE_ROOK_BLACK);
            _boardView[0, 7].Image = Image.FromFile(Resources.IMAGE_ROOK_BLACK);
            _boardView[7, 0].Image = Image.FromFile(Resources.IMAGE_ROOK_WHITE);
            _boardView[7, 7].Image = Image.FromFile(Resources.IMAGE_ROOK_WHITE);

            _pieces[0, 0] = new Rook(_boardView[0, 0], PieceColor.Black);
            _pieces[0, 7] = new Rook(_boardView[0, 7], PieceColor.Black);
            _pieces[3, 0] = new Rook(_boardView[7, 0], PieceColor.White);
            _pieces[3, 7] = new Rook(_boardView[7, 7], PieceColor.White);

            _boardView[0, 0].OwnPiece = _pieces[0, 0];
            _boardView[0, 7].OwnPiece = _pieces[0, 7];
            _boardView[7, 0].OwnPiece = _pieces[3, 0];
            _boardView[7, 7].OwnPiece = _pieces[3, 7];
        }

        private void ShowPawn()
        {
            for (int j = 0; j < DEFAULT_BOARD_WIDTH; j++)
            {
                _boardView[1, j].Image = Image.FromFile(Resources.IMAGE_PAWN_BLACK);
                _boardView[6, j].Image = Image.FromFile(Resources.IMAGE_PAWN_WHITE);

                _pieces[1, j] = new Pawn(_boardView[1, j], PieceColor.Black);
                _pieces[2, j] = new Pawn(_boardView[6, j], PieceColor.White);

                _boardView[1, j].OwnPiece = _pieces[1, j];
                _boardView[6, j].OwnPiece = _pieces[2, j];
            }
        }
        #endregion

        private void ShowStartButton(int squareWidth, int leftBoardInit)
        {
            int yCoord = 5,
                xCoord = leftBoardInit + (squareWidth * (DEFAULT_BOARD_WIDTH - 1)) / 2;

            _startButton = new Button()
            {
                Text = "Start",
                Width = 80,
                Height = 30,
                Location = new Point(xCoord, yCoord),
            };

            _startButton.MouseClick += new MouseEventHandler(StartButtonMouseClick);

            _parentForm.Controls.Add(_startButton);
        }
        private void ShowTimerView(int squareWidth, int squareHeight, int leftBoardInit, int topBoardInit)
        {
            InitTimer(squareWidth, topBoardInit);

            int yTopTimerView = topBoardInit + squareWidth,
                xTopTimerView = leftBoardInit + (squareWidth * (DEFAULT_BOARD_WIDTH + 1)),
                yBottomTimerView = topBoardInit + squareWidth * 7,
                xBottomTimerView = xTopTimerView,
                fontSize = 12;

            TimeSpan topPlayerTimeSpan = TimeSpan.FromSeconds(_topPlayerTime);
            TimeSpan bottomPlayerTimeSpan = TimeSpan.FromSeconds(_bottomPlayerTime);

            string topPlayerTimerViewText = topPlayerTimeSpan.ToString(@"mm\:ss");
            string bottomPlayerTimerViewText = bottomPlayerTimeSpan.ToString(@"mm\:ss");

            _topPlayerTimerView = new Label()
            {
                Text = topPlayerTimerViewText,
                Width = 100,
                Height = 20,
                Font = new Font("Arial", fontSize),
                Location = new Point(xTopTimerView, yTopTimerView),
            };

            _bottomPlayerTimerView = new Label()
            {
                Text = bottomPlayerTimerViewText,
                Width = 100,
                Height = 20,
                Font = new Font("Arial", fontSize),
                Location = new Point(xBottomTimerView, yBottomTimerView),
            };

            _parentForm.Controls.Add(_topPlayerTimerView);
            _parentForm.Controls.Add(_bottomPlayerTimerView);
        }

        private void InitTimer(int squareWidth, int leftBoardInit)
        {
            _topPlayerTimer = new Timer()
            {
                Interval = 1000,
                
            };
            
            
            _bottomPlayerTimer = new Timer()
            {
                Interval = 1000,

            };

            _topPlayerTimer.Tick += new EventHandler(TopTimerOnTick);
            _bottomPlayerTimer.Tick += new EventHandler(BottomTimerOnTick);

        }


        private void StartButtonMouseClick(object sender, MouseEventArgs e)
        {
            _topPlayerTimer.Start();
        }

        private void TopTimerOnTick(object sender, EventArgs e)
        {
            _topPlayerTime--;

            TimeSpan topPlayerTimeSpan = TimeSpan.FromSeconds(_topPlayerTime);
            string topPlayerTimerViewText = topPlayerTimeSpan.ToString(@"mm\:ss");

            _topPlayerTimerView.Text = topPlayerTimerViewText;
        }

        private void BottomTimerOnTick(object sender, EventArgs e)
        {
            _bottomPlayerTime--;

            TimeSpan bottomPlayerTimeSpan = TimeSpan.FromSeconds(_bottomPlayerTime);
            string bottomPlayerTimerViewText = bottomPlayerTimeSpan.ToString(@"mm\:ss");

            _bottomPlayerTimerView.Text = bottomPlayerTimerViewText;
        }

        #endregion

        #region Set the path a chess can move
        public void SetCanMovePath()
        {
            if (SelectingSquare.OwnPiece.Type == PieceType.King)
            {
                SetKingCanMovePath();
                return;
            }
            if (SelectingSquare.OwnPiece.Type == PieceType.Rook)
            {
                SetRookCanMovePath();
                return;
            }
            if (SelectingSquare.OwnPiece.Type == PieceType.Bishop)
            {
                SetBishopCanMovePath();
                return;
            }
            if (SelectingSquare.OwnPiece.Type == PieceType.Pawn)
            {
                SetPawnCanMovePath();
                return;
            }
            if (SelectingSquare.OwnPiece.Type == PieceType.Knight)
            {
                SetKnightCanMovePath();
                return;
            }
            if (SelectingSquare.OwnPiece.Type == PieceType.Queen)
            {
                SetQueenCanMovePath();
                return;
            }
        }

        public void ShowCanMovePath()
        {
            Square currentSquare;
            SetCanMovePath();

            for (int i = 0; i < DEFAULT_BOARD_WIDTH; i++)
            {
                for (int j = 0; j < DEFAULT_BOARD_HEIGHT; j++)
                {
                    currentSquare = _boardView[i, j];

                    if (_canMove[i,j])
                    {
                        if (currentSquare.HasPiece)
                            currentSquare.BackColor = GetCanKillColor(currentSquare);
                        else
                            currentSquare.BackColor = GetCanMoveColor(currentSquare);
                    }
                }
            }
        }

        
        public void UnshowCanMovePath()
        {
            for (int i = 0; i < DEFAULT_BOARD_WIDTH; i++)
            {
                for (int j = 0; j < DEFAULT_BOARD_HEIGHT; j++)
                {
                    if (_canMove[i, j])
                    {
                        if (_boardView[i, j].Color == SquareColor.White)
                            _boardView[i, j].Color = SquareColor.White;
                        else
                            _boardView[i, j].Color = SquareColor.Black;
                    }
                }
            }
        }

        private void SetKingCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetAroundCanMovePath();
        }

        private void SetRookCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetStraightCanMovePath();
        }

        private void SetBishopCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetDiagonalCanMovePath();
        }

        private void SetPawnCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetPawnOwnCanMovePath();
        }

        private void SetKnightCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetKnightOwnCanMovePath();
        }

        private void SetQueenCanMovePath()
        {
            InitializeFalseValue(_canMove);
            SetDiagonalCanMovePath();
            SetStraightCanMovePath();
        }

        private SysColor GetCanMoveColor(Square square)
        {
            if (square.Color == SquareColor.White)
                return _canMoveColor_White;

            return _canMoveColor_Black;
        }

        private SysColor GetCanKillColor(Square square)
        {
            if (square.Color == SquareColor.White)
                return _canKillColor_White;

            return _canKillColor_Black;
        }


        private void SetAroundCanMovePath()
        {
            int x = SelectingSquare.XBoard,
                y = SelectingSquare.YBoard,
                currentCheckingX, currentCheckingY;
            Square currentCheckingSquare;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    currentCheckingX = x + i;
                    currentCheckingY = y + j;

                    if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                    {
                        currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                        CheckSquareAndSetCanMove(currentCheckingSquare);
                    }
                }
            }
        }

        private void SetStraightCanMovePath()
        {
            int x = SelectingSquare.XBoard,
                y = SelectingSquare.YBoard;
            BoardPosition currentPosition = new BoardPosition(x, y);

            SetRowCanMovePath(currentPosition);
            SetColumnCanMovePath(currentPosition);
        }

        private void SetDiagonalCanMovePath()
        {
            int x = SelectingSquare.XBoard,
                y = SelectingSquare.YBoard;
            BoardPosition currentPosition = new BoardPosition(x, y);

            SetMainDiagonalCanMovePath(currentPosition);
            SetCounterDiagonalCanMovePath(currentPosition);
        }

        private void SetPawnOwnCanMovePath()
        {
            int step,
                x = SelectingSquare.XBoard,
                y = SelectingSquare.YBoard,
                currentCheckingX, currentCheckingY;
            Square currentCheckingSquare;

            if (SelectingSquare.OwnPiece.Color == TOP_BOARD_COLOR)
                step = 1;
            else
                step = -1;


            // go up 2
            currentCheckingX = x + 2 * step;
            currentCheckingY = y;
            if (IsInBoardIndex(currentCheckingX, currentCheckingY) && IsPawnAtInitialPosition(SelectingSquare.OwnPiece))
            {
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                if (IsObstacle(currentCheckingSquare))
                    _canMove[currentCheckingX, currentCheckingY] = false;
                else
                    CheckSquareAndSetCanMove(currentCheckingSquare);
            }

            // go up 1
            currentCheckingX = x + step;
            
            if(IsInBoardIndex(currentCheckingX, currentCheckingY))
            {
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                if (IsObstacle(currentCheckingSquare))
                    _canMove[currentCheckingX, currentCheckingY] = false;
                else
                    CheckSquareAndSetCanMove(currentCheckingSquare);
            }



            // go up right
            currentCheckingY = y + 1;

            if (IsInBoardIndex(currentCheckingX, currentCheckingY))
            {
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                if (IsObstacle(currentCheckingSquare))
                    _canMove[currentCheckingX, currentCheckingY] = true;
            }


            // go up left
            currentCheckingY = currentCheckingY - 2;

            if (IsInBoardIndex(currentCheckingX, currentCheckingY))
            {
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                if (IsObstacle(currentCheckingSquare))
                    _canMove[currentCheckingX, currentCheckingY] = true;
            }
        }

        private void SetKnightOwnCanMovePath()
        {
            int step,
                x = SelectingSquare.XBoard,
                y = SelectingSquare.YBoard,
                currentCheckingX, currentCheckingY;
            Square currentCheckingSquare;

            for (int i = -2; i <= 2; i++)
            {
                if (i == 0)
                    continue;

                if (i % 2 == 0)
                    step = -1;
                else
                    step = -2;

                currentCheckingX = x - i;
                currentCheckingY = y + step;

                if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                    CheckSquareAndSetCanMove(currentCheckingSquare);
                }

                currentCheckingY = y - step;

                if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];
                    CheckSquareAndSetCanMove(currentCheckingSquare);
                }

            }
        }

        private void SetRowCanMovePath(BoardPosition coord)
        {
            int x = coord.Key,
                y = coord.Value,
                currentCheckingX, currentCheckingY;
            Square currentCheckingSquare;

            for (int i = x - 1; i >= MINIMUM_COL; i--)
            {
                currentCheckingX = i;
                currentCheckingY = y;

                if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                    _canMove[currentCheckingX, currentCheckingY] = true;

                    if (IsObstacle(currentCheckingSquare))
                        break;

                    if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                    {
                        _canMove[currentCheckingX, currentCheckingY] = false;
                        break;
                    }
                }
                
            }

            for (int i = x  + 1; i < DEFAULT_BOARD_WIDTH; i++)
            {
                currentCheckingX = i;
                currentCheckingY = y;

                if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                    _canMove[currentCheckingX, currentCheckingY] = true;

                    if (IsObstacle(currentCheckingSquare))
                        break;

                    if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                    {
                        _canMove[currentCheckingX, currentCheckingY] = false;
                        break;
                    }
                }
            }
        }

        private void SetColumnCanMovePath(BoardPosition coord)
        {
            int x = coord.Key,
                y = coord.Value,
                currentCheckingX, currentCheckingY;
            Square currentCheckingSquare;

            for (int i = y - 1; i >= MINIMUM_ROW; i--)
            {
                currentCheckingX = x;
                currentCheckingY = i;

                if (IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                    _canMove[currentCheckingX, currentCheckingY] = true;

                    if (IsObstacle(currentCheckingSquare))
                        break;

                    if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                    {
                        _canMove[currentCheckingX, currentCheckingY] = false;
                        break;
                    }
                }
                
            }

            for (int i = y + 1; i < DEFAULT_BOARD_WIDTH; i++)
            {
                currentCheckingX = x;
                currentCheckingY = i;

                if(IsInBoardIndex(currentCheckingX, currentCheckingY))
                {
                    currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                    _canMove[currentCheckingX, currentCheckingY] = true;

                    if (IsObstacle(currentCheckingSquare))
                        break;

                    if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                    {
                        _canMove[currentCheckingX, currentCheckingY] = false;
                        break;
                    }
                }
            }
        }

        private void SetMainDiagonalCanMovePath(BoardPosition currentPosition)
        {
            int x = currentPosition.Key,
                y = currentPosition.Value,
                currentCheckingX, currentCheckingY,
                MinTopLeft = Math.Min(x, y),
                MinTopRight = Math.Min(x, 7 - y);
            Square currentCheckingSquare;


            for (int i = 1; i <= MinTopLeft; i++)
            {
                currentCheckingX = x - i;
                currentCheckingY = y - i;
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                _canMove[currentCheckingX, currentCheckingY] = true;

                if (IsObstacle(currentCheckingSquare))
                    break;

                if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                {
                    _canMove[currentCheckingX, currentCheckingY] = false;
                    break;
                }
            }

            for (int i = 1; i <= MinTopRight; i++)
            {
                currentCheckingX = x - i;
                currentCheckingY = y + i;
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                _canMove[currentCheckingX, currentCheckingY] = true;

                if (IsObstacle(currentCheckingSquare))
                    break;

                if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                {
                    _canMove[currentCheckingX, currentCheckingY] = false;
                    break;
                }
            }
        }

        private void SetCounterDiagonalCanMovePath(BoardPosition currentPosition)
        {
            int x = currentPosition.Key,
                y = currentPosition.Value,
                currentCheckingX, currentCheckingY,
                MinBotLeft = Math.Min(7 - x, y),
                MinBotRight = Math.Min(7 - x, 7 - y);
            Square currentCheckingSquare;


            for (int i = 1; i <= MinBotLeft; i++)
            {
                currentCheckingX = x + i;
                currentCheckingY = y - i;
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                _canMove[currentCheckingX, currentCheckingY] = true;

                if (IsObstacle(currentCheckingSquare))
                    break;

                if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                {
                    _canMove[currentCheckingX, currentCheckingY] = false;
                    break;
                }
            }

            for (int i = 1; i <= MinBotRight; i++)
            {
                currentCheckingX = x + i;
                currentCheckingY = y + i;
                currentCheckingSquare = _boardView[currentCheckingX, currentCheckingY];

                _canMove[currentCheckingX, currentCheckingY] = true;

                if (IsObstacle(currentCheckingSquare))
                    break;

                if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                {
                    _canMove[currentCheckingX, currentCheckingY] = false;
                    break;
                }
            }
        }

        private void CheckSquareAndSetCanMove(Square currentCheckingSquare)
        {
            int currentCheckingX = currentCheckingSquare.XBoard,
                currentCheckingY = currentCheckingSquare.YBoard;

            if (HasSameColor(currentCheckingSquare.OwnPiece, SelectingSquare.OwnPiece))
                _canMove[currentCheckingX, currentCheckingY] = false;
            else
                _canMove[currentCheckingX, currentCheckingY] = true;
        }

        private void InitializeFalseValue(bool[,] boolArray)
        {
            int nRows = boolArray.GetLength(0),
                nCols = boolArray.GetLength(1);

            for (int row = 0; row < nRows; row++)
                for (int col = 0; col < nCols; col++)
                {
                    boolArray[row, col] = false;
                }
        }
        #endregion


        #region Condition field
        private static bool IsWhiteSquare(int i, int j)
        {
            return (i % 2 != 0 && j % 2 != 0) || (i % 2 == 0 && j % 2 == 0);
        }
        private bool HasSameColor(Piece currentCheckingPiece, Piece selectingPiece)
        {
            if (currentCheckingPiece == null)
                return false;
            return currentCheckingPiece.Color == selectingPiece.Color;
        }

        private bool IsObstacle(Square checkingChess)
        {
            return checkingChess.OwnPiece != null && checkingChess.OwnPiece.Color != SelectingSquare.OwnPiece.Color;
        }

        private bool IsInBoardIndex(int currentCheckingX, int currentCheckingY)
        {
            return IsInRange_Equal(currentCheckingX, 0, DEFAULT_BOARD_WIDTH - 1) && IsInRange_Equal(currentCheckingY, 0, DEFAULT_BOARD_HEIGHT - 1);
        }

        private bool IsInRange_NotEqual(int value, int minValue, int maxValue)
        {
            return minValue < value && value < maxValue;
        }        
        private bool IsInRange_Equal(int value, int minValue, int maxValue)
        {
            return minValue <= value && value <= maxValue;
        }

        private bool IsPawnAtInitialPosition(Piece checkingPiece)
        {
            if (checkingPiece.Type == PieceType.Pawn)
            {
                if (checkingPiece.Color == PieceColor.White &&
                    checkingPiece.CurrentPosition.Key == 6)
                    return true;

                if (checkingPiece.Color == PieceColor.Black && 
                    checkingPiece.CurrentPosition.Key == 1)
                    return true;
            }

            return false;
        }
        
        #endregion
    }
}
