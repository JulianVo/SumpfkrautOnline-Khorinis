﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinApi.User.Enumeration;

namespace GUC.GUI
{
    public interface InputReceiver
    {
        void KeyPressed(VirtualKeys key);
    }

    public struct ViewPoint
    {
        public int X, Y;

        public ViewPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("ViewPoint({0} {1})", X, Y);
        }
    }

    public abstract class GUCView
    {
        #region Chars

        static double[] charArray;

        static Dictionary<string, double> AllChars = new Dictionary<string, double>() { { "A", 14.998571428571428571428571428571d }, { "B", 11.998857142857142857142857142857d }, { "C", 10.998857142857142857142857142857d }, { "D", 11.998857142857142857142857142857d }, { "E", 11.998857142857142857142857142857d }, { "F", 10.999142857142857142857142857143d }, { "G", 12.998857142857142857142857142857d }, { "H", 11.998857142857142857142857142857d }, { "I", 4.9997142857142857142857142857143d }, { "J", 10.999142857142857142857142857143d }, { "K", 12.998857142857142857142857142857d },
                                                                                        { "L", 10.999142857142857142857142857143d }, { "M", 14.998571428571428571428571428571d }, { "N", 10.999142857142857142857142857143d }, { "O", 12.998857142857142857142857142857d }, { "P", 11.998857142857142857142857142857d }, { "Q", 12.998857142857142857142857142857d }, { "R", 11.998571428571428571428571428571d }, { "S", 11.998857142857142857142857142857d }, { "T", 11.998857142857142857142857142857d }, { "U", 11.998857142857142857142857142857d }, { "V", 11.998857142857142857142857142857d },
                                                                                        { "W", 14.998571428571428571428571428571d }, { "X", 13.998571428571428571428571428571d }, { "Y", 10.999142857142857142857142857143d }, { "Z", 10.999142857142857142857142857143d }, { "a", 10.999142857142857142857142857143d }, { "b", 10.999142857142857142857142857143d }, { "c", 9.9991428571428571428571428571429d }, { "d", 10.999142857142857142857142857143d }, { "e", 10.999142857142857142857142857143d }, { "f", 8.9991428571428571428571428571429d }, { "g", 10.999142857142857142857142857143d },
                                                                                        { "h", 10.999142857142857142857142857143d }, { "i", 5.9994285714285714285714285714286d }, { "j", 4.9997142857142857142857142857143d }, { "k", 9.9991428571428571428571428571429d }, { "l", 4.9997142857142857142857142857143d }, { "m", 12.998857142857142857142857142857d }, { "n", 10.999142857142857142857142857143d }, { "o", 10.999142857142857142857142857143d }, { "p", 10.999142857142857142857142857143d }, { "q", 10.999142857142857142857142857143d }, { "r", 8.9991428571428571428571428571429d },
                                                                                        { "s", 10.999142857142857142857142857143d }, { "t", 7.9994285714285714285714285714286d }, { "u", 10.999142857142857142857142857143d }, { "v", 10.999142857142857142857142857143d }, { "w", 13.998571428571428571428571428571d }, { "x", 10.999142857142857142857142857143d }, { "y", 10.999142857142857142857142857143d }, { "z", 9.9991428571428571428571428571429d }, { "Ä", 14.998571428571428571428571428571d }, { "Ü", 11.998857142857142857142857142857d }, { "Ö", 12.998857142857142857142857142857d },
                                                                                        { "ä", 10.999142857142857142857142857143d }, { "ü", 10.999142857142857142857142857143d }, { "ö", 10.999142857142857142857142857143d }, { "ß", 11.998857142857142857142857142857d }, { "1", 5.9994285714285714285714285714286d }, { "2", 8.9991428571428571428571428571429d }, { "3", 8.9991428571428571428571428571429d }, { "4", 10.999142857142857142857142857143d }, { "5", 7.9994285714285714285714285714286d }, { "6", 7.9994285714285714285714285714286d }, { "7", 8.9991428571428571428571428571429d },
                                                                                        { "8", 7.9994285714285714285714285714286d }, { "9", 7.9994285714285714285714285714286d }, { "0", 8.9991428571428571428571428571429d }, { "!", 3.9997142857142857142857142857143d }, { "\"", 6.9994285714285714285714285714286d }, { "§", 7.9994285714285714285714285714286d }, { "$", 7.9994285714285714285714285714286d }, { "%", 10.999142857142857142857142857143d }, { "&", 11.998857142857142857142857142857d }, { "/", 7.9994285714285714285714285714286d }, { "(", 5.9994285714285714285714285714286d },
                                                                                        { ")", 4.9997142857142857142857142857143d }, { "=", 7.9994285714285714285714285714286d }, { "?", 7.9994285714285714285714285714286d }, { "_", 10.999142857142857142857142857143d }, { "-", 6.9994285714285714285714285714286d }, { ".", 4.9997142857142857142857142857143d }, { ":", 3.9997142857142857142857142857143d }, { ",", 4.9997142857142857142857142857143d }, { ";", 3.9997142857142857142857142857143d }, { "<", 8.9991428571428571428571428571429d }, { ">", 8.9991428571428571428571428571429d },
                                                                                        { "|", 2.9997142857142857142857142857143d }, { "#", 12.998857142857142857142857142857d }, { "'", 4.9997142857142857142857142857143d }, { "+", 8.9991428571428571428571428571429d }, { "~", 9.9991428571428571428571428571429d }, { "{", 5.9994285714285714285714285714286d }, { "}", 5.9994285714285714285714285714286d }, { "@", 13.998571428571428571428571428571d }, {" ", 9.9991428571428571428571428571429d } };
        static Dictionary<char, float> GothicChars = new Dictionary<char, float>() { { 'A', 14.99857f }, { 'B', 11.99886f }, { 'C', 10.99886f }, { 'D', 11.99886f }, { 'E', 11.99886f }, { 'F', 10.99914f }, { 'G', 12.99886f }, { 'H', 11.99886f }, { 'I', 4.99971f }, { 'J', 10.99914f }, { 'K', 12.99886f },
                                                                                         { 'L', 10.99914f }, { 'M', 14.99857f }, { 'N', 10.99914f }, { 'O', 12.99886f }, { 'P', 11.99886f }, { 'Q', 12.99886f }, { 'R', 11.99857f }, { 'S', 11.99886f }, { 'T', 11.99886f }, { 'U', 11.99886f }, { 'V', 11.99886f },
                                                                                         { 'W', 14.99857f }, { 'X', 13.99857f }, { 'Y', 10.99914f }, { 'Z', 10.99914f }, { 'a', 10.99914f }, { 'b', 10.99914f }, { 'c', 9.99914f  }, { 'd', 10.99914f }, { 'e', 10.99914f }, { 'f', 8.99914f }, { 'g', 10.99914f },
                                                                                         { 'h', 10.99914f }, { 'i', 5.99943f }, { 'j', 4.99971f }, { 'k', 9.99914f }, { 'l', 4.99971f }, { 'm', 12.99886f }, { 'n', 10.99914f }, { 'o', 10.99914f }, { 'p', 10.99914f }, { 'q', 10.99914f }, { 'r', 8.99914f },
                                                                                         { 's', 10.99914f }, { 't', 7.99943f }, { 'u', 10.99914f }, { 'v', 10.99914f }, { 'w', 13.99857f }, { 'x', 10.99914f }, { 'y', 10.99914f }, { 'z', 9.99914f }, { 'Ä', 14.99857f }, { 'Ü', 11.99886f }, { 'Ö', 12.99886f },
                                                                                         { 'ä', 10.99914f }, { 'ü', 10.99914f }, { 'ö', 10.99914f }, { 'ß', 11.99886f }, { '1', 5.99943f }, { '2', 8.99914f }, { '3', 8.99914f }, { '4', 10.99914f }, { '5', 7.99943f }, { '6', 7.99943f }, { '7', 8.99914f },
                                                                                         { '8', 7.99943f }, { '9', 7.99943f }, { '0', 8.99914f }, { '!', 3.99971f }, { '"', 6.99943f }, { '§', 7.99943f }, { '$', 7.99943f }, { '%', 10.99914f }, { '&', 11.99886f }, { '/', 7.99943f }, { '(', 5.99943f },
                                                                                         { ')', 4.99971f }, { '=', 7.99943f }, { '?', 7.99943f }, { '_', 10.99914f }, { '-', 6.99943f }, { '.', 4.99971f }, { ':', 3.99971f }, { ',', 4.99971f }, { ';', 3.99971f }, { '<', 8.99914f }, { '>', 8.99914f },
                                                                                         { '|', 2.99971f }, { '#', 12.99886f }, { '\'', 4.99971f }, { '+', 8.99914f }, { '~', 9.99914f }, { '{', 5.99943f }, { '}', 5.99943f }, { '@', 13.99857f }, {' ', 9.99914f } };
        
        static GUCView()
        {
            charArray = new double[AllChars.Keys.Select(s => s[0]).Max() + 1];
            foreach (KeyValuePair<string, double> pair in AllChars)
                charArray[pair.Key[0]] = pair.Value;
        }

        /// <summary> Returns the char width in pixels or 0 if the char is not supported by Gothic. </summary>
        public static double GetCharWidth(char c)
        {
            return c < charArray.Length ? charArray[c] : 0;
        }

        /// <summary> Returns if the char is supported by Gothic. </summary>
        public static bool GothicContainsChar(char c)
        {
            if (c >= charArray.Length)
                return false;

            return charArray[c] != 0;
        }

        /// <summary> Calculates the width of a string in pixels. Non-Gothic-Chars are skipped. </summary>
        public static double StringPixelWidth(string str)
        {
            double width = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int index;
                if ((index = str[i]) < charArray.Length)
                    width += charArray[index];
            }
            return width;
        }

        #endregion

        public abstract void Show();
        public abstract void Hide();

        #region Fonts
        public enum Fonts
        {
            /// <summary> Small text font. </summary>
            Default,
            /// <summary> Highlighted text font, so it's just white. </summary>
            Default_Hi,
            /// <summary> Large text font. </summary>
            Menu,
            /// <summary> Highlighted text font, so it's just white. </summary>
            Menu_Hi,

            Book,
            Book_Hi,

            BookLarge,
            BookLarge_Hi,
        }

        public const int FontsizeDefault = 18;
        public const int FontsizeMenu = 32;

        protected static Dictionary<Fonts, string> fontDict = new Dictionary<Fonts, string>
        {
            { Fonts.Default, "Font_Old_10_White.tga"},
            { Fonts.Default_Hi, "Font_Old_10_White_Hi.tga"},
            { Fonts.Menu, "Font_Old_20_White.tga"},
            { Fonts.Menu_Hi, "Font_Old_20_White_Hi.tga"},

            { Fonts.Book, "Font_10_Book.tga"},
            { Fonts.Book_Hi, "Font_10_Book_Hi.tga"},
            { Fonts.BookLarge, "Font_20_Book.tga"},
            { Fonts.BookLarge_Hi, "Font_20_Book_Hi.tga"},
        };

        public static int GetFontSize(Fonts font)
        {
            return font < Fonts.Menu ? FontsizeDefault : FontsizeMenu;
        }

        #endregion

        #region pixel virtual conversion

        static bool iniRes = false;
        public static ViewPoint GetScreenSize()
        {
            var screen = Gothic.View.zCView.GetScreen();

            var ret = new ViewPoint(screen.pSizeX, screen.pSizeY);

            if (ret.X > 0 && ret.Y > 0)
            {
                iniRes = false;
            }
            else
            {
                var sec = Gothic.zCOption.GetSectionByName("VIDEO");
                ret.X = Convert.ToInt32(sec.GetEntryByName("zVidResFullscreenX").VarValue.ToString());
                ret.Y = Convert.ToInt32(sec.GetEntryByName("zVidResFullscreenY").VarValue.ToString());

                if (!iniRes)
                {
                    Log.Logger.LogWarning("Couldn't find real resolution, using Gothic.ini resolution: " + ret.X + "x" + ret.Y);
                }
                iniRes = true;
            }
            return ret;
        }

        public static ViewPoint PixelToVirtual(int x, int y)
        {
            var res = GetScreenSize();
            return new ViewPoint(x * 0x2000 / res.X, y * 0x2000 / res.Y);
        }

        public static ViewPoint PixelToVirtual(ViewPoint point)
        {
            var res = GetScreenSize();
            return new ViewPoint(point.X * 0x2000 / res.X, point.Y * 0x2000 / res.Y);
        }

        public static int PixelToVirtualX(int x)
        {
            return x * 0x2000 / GetScreenSize().X;
        }

        public static int PixelToVirtualY(int y)
        {
            return y * 0x2000 / GetScreenSize().Y;
        }

        #endregion

        static private GUCVisualText debugText;
        static public GUCVisualText DebugText
        {
            get
            {
                if (debugText == null)
                {
                    var vis = new GUCVisual();
                    debugText = vis.CreateTextCenterX("", 5);
                }
                debugText.Parent.Show();
                return debugText;
            }
        }
    }
}
