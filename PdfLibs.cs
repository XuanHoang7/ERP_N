using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System;
using System.Linq;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace NETCORE3
{
    public class PdfLibs
    {
        public enum Alignment_X
        {
            LEFT = 0,
            CENTER = 1,
            RIGHT = 2,
            JUSTIFIED = 3,
        }
        public enum Alignment_Y
        {
            TOP = 4,
            MIDDLE = 5,
            BOTTOM = 6,
            BASELINE = 7,
        }
        public enum FontStyle
        {
            NOMAL = 0,
            BOLD = 1,
            ITALIC = 2,
            BOLD_ITALIC = 3,
            UNDERLINE = 4,
            BOLD_UNDERLINE = 5,
            ITALIC_UNDERLINE = 6,
            BOLD_ITALIC_UNDERLINE = 7,
            STRIKETHRU = 8, //Gạch ngang đè lên ký tự
        }
        public enum FontFamily
        {
            TIMES_ROMAN,
            SYMBOL,
        }
        private static BaseFont CreateFontFamily(FontFamily fontFamily)
        {
            return fontFamily switch
            {
                FontFamily.SYMBOL => BaseFont.CreateFont("Uploads/Font/Symbola.ttf", BaseFont.IDENTITY_H, true),
                _ => BaseFont.CreateFont("Uploads/Font/times.ttf", BaseFont.IDENTITY_H, true),
            };
        }
        public enum Color
        {
            BLACK,
            WHITE,
            RED,
            LIME,
            BLUE,
            YELLOW,
            CYAN,
            FUCHSIA,
            SILVER,
            GRAY,
            MAROON,
            OLIVE,
            GREEN,
            PURPLE,
            TEAL,
            NAVY,
            LIGHTGRAY,
            DARKCYAN,
        }
        private static BaseColor CreateColor(Color color)
        {
            return color switch
            {
                Color.BLACK => new BaseColor(0, 0, 0),
                Color.WHITE => new BaseColor(255, 255, 255),
                Color.RED => new BaseColor(255, 0, 0),
                Color.LIME => new BaseColor(0, 255, 0),
                Color.BLUE => new BaseColor(0, 0, 255),
                Color.YELLOW => new BaseColor(255, 255, 0),
                Color.CYAN => new BaseColor(0, 255, 255),
                Color.FUCHSIA => new BaseColor(255, 0, 255),
                Color.SILVER => new BaseColor(192, 192, 192),
                Color.GRAY => new BaseColor(128, 128, 128),
                Color.MAROON => new BaseColor(128, 0, 0),
                Color.OLIVE => new BaseColor(128, 128, 0),
                Color.GREEN => new BaseColor(0, 128, 0),
                Color.PURPLE => new BaseColor(128, 0, 128),
                Color.TEAL => new BaseColor(0, 128, 128),
                Color.NAVY => new BaseColor(0, 0, 128),
                Color.LIGHTGRAY => new BaseColor(211, 211, 211),
                Color.DARKCYAN => new BaseColor(0, 128, 128),
                _ => new BaseColor(0, 0, 0),
            };
        }
        public enum BackgroundColor
        {
            BLACK,
            WHITE,
            RED,
            LIME,
            BLUE,
            YELLOW,
            CYAN,
            FUCHSIA,
            SILVER,
            GRAY,
            MAROON,
            OLIVE,
            GREEN,
            PURPLE,
            TEAL,
            NAVY,
            LIGHTGRAY,
            DARKCYAN,
            COLORDANHGIANHANSU,
            COLORDANHGIANHANSU2,
            COLORDANHGIANHANSU3,
            XANHDAM,
            XANHTRUNG,
            XANHNHAT,
        }
        private static BaseColor CreateBackgroundColor(BackgroundColor color)
        {
            return color switch
            {
                BackgroundColor.BLACK => new BaseColor(0, 0, 0),
                BackgroundColor.WHITE => new BaseColor(255, 255, 255),
                BackgroundColor.RED => new BaseColor(255, 0, 0),
                BackgroundColor.LIME => new BaseColor(0, 255, 0),
                BackgroundColor.BLUE => new BaseColor(0, 0, 255),
                BackgroundColor.YELLOW => new BaseColor(255, 255, 0),
                BackgroundColor.CYAN => new BaseColor(0, 255, 255),
                BackgroundColor.FUCHSIA => new BaseColor(255, 0, 255),
                BackgroundColor.SILVER => new BaseColor(192, 192, 192),
                BackgroundColor.GRAY => new BaseColor(128, 128, 128),
                BackgroundColor.MAROON => new BaseColor(128, 0, 0),
                BackgroundColor.OLIVE => new BaseColor(128, 128, 0),
                BackgroundColor.GREEN => new BaseColor(0, 128, 0),
                BackgroundColor.PURPLE => new BaseColor(128, 0, 128),
                BackgroundColor.TEAL => new BaseColor(0, 128, 128),
                BackgroundColor.NAVY => new BaseColor(0, 0, 128),
                BackgroundColor.LIGHTGRAY => new BaseColor(242, 242, 242),
                BackgroundColor.DARKCYAN => new BaseColor(0, 128, 128),
                BackgroundColor.COLORDANHGIANHANSU => new BaseColor(218, 238, 243),
                BackgroundColor.COLORDANHGIANHANSU2 => new BaseColor(255, 255, 235),
                BackgroundColor.COLORDANHGIANHANSU3 => new BaseColor(255, 255, 204),
                BackgroundColor.XANHDAM => new BaseColor(189, 214, 238),
                BackgroundColor.XANHTRUNG => new BaseColor(225, 235, 243),
                BackgroundColor.XANHNHAT => new BaseColor(255, 255, 204),
                _ => new BaseColor(0, 0, 0),
            };
        }
        public enum Border
        {
            NONE = 0,
            TOP = 1,
            BOTTOM = 2,
            TOP_BOTTOM = 3,
            LEFT = 4,
            TOP_LEFT = 5,
            BOTTOM_LEFT = 6,
            TOP_BOTTOM_LEFT = 7,
            RIGHT = 8,
            TOP_RIGHT = 9,
            BOTTOM_RIGHT = 10,
            TOP_BOTTOM_RIGHT = 11,
            LEFT_RIGHT = 12,
            TOP_LEFT_RIGHT = 13,
            BOTTOM_LEFT_RIGHT = 14,
            BOX = 15,
        }
        public static PdfPCell CreateCell(string _content, float _fontSize, int _colspan, float[] _padding, Border _border = Border.NONE, Alignment_X _x = Alignment_X.LEFT, Alignment_Y _y = Alignment_Y.TOP, FontStyle _fontStyle = FontStyle.NOMAL, FontFamily _fontFamily = FontFamily.TIMES_ROMAN, Color _color = Color.BLACK, BackgroundColor _backgroundColor = BackgroundColor.WHITE, float _lineHeight = 1f)
        {
            PdfPCell Cell = new(new Phrase(_content, new Font(CreateFontFamily(_fontFamily), _fontSize, (int)_fontStyle, CreateColor(_color))))
            {
                HorizontalAlignment = (int)_x,
                VerticalAlignment = (int)_y,
                BackgroundColor = CreateBackgroundColor(_backgroundColor),
                Colspan = _colspan,
                Border = (int)_border,
            };
            Cell.SetLeading(0f, _lineHeight);
            switch (_padding.Length)
            {
                case 1:
                    Cell.Padding = _padding[0];
                    break;
                case 4:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingRight = _padding[1];
                    Cell.PaddingBottom = _padding[2];
                    Cell.PaddingLeft = _padding[3];
                    break;
                case 2:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingBottom = _padding[0];
                    Cell.PaddingLeft = _padding[1];
                    Cell.PaddingRight = _padding[1];
                    break;
                default:
                    break;
            }
            return Cell;
        }
        public static PdfPCell CreateCell(string[] _content, FontStyle[] _fontStyle, float _fontSize, int _colspan, float[] _padding, Border _border = Border.NONE, Alignment_X _x = Alignment_X.LEFT, Alignment_Y _y = Alignment_Y.TOP, FontFamily _fontFamily = FontFamily.TIMES_ROMAN, Color _color = Color.BLACK, BackgroundColor _backgroundColor = BackgroundColor.WHITE, float _lineHeight = 1f)
        {
            Phrase phrase = new Phrase();
            for (int i = 0; i < _content.Length; ++i)
            {
                phrase.Add(new Chunk(_content[i], new Font(CreateFontFamily(_fontFamily), _fontSize, (int)_fontStyle[i], CreateColor(_color))));
            }
            PdfPCell Cell = new PdfPCell(phrase)
            {
                HorizontalAlignment = (int)_x,
                VerticalAlignment = (int)_y,
                BackgroundColor = CreateBackgroundColor(_backgroundColor),
                Colspan = _colspan,
                Border = (int)_border,
            };
            Cell.SetLeading(0f, _lineHeight);
            switch (_padding.Length)
            {
                case 1:
                    Cell.Padding = _padding[0];
                    break;
                case 4:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingRight = _padding[1];
                    Cell.PaddingBottom = _padding[2];
                    Cell.PaddingLeft = _padding[3];
                    break;
                case 2:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingBottom = _padding[0];
                    Cell.PaddingLeft = _padding[1];
                    Cell.PaddingRight = _padding[1];
                    break;
                default:
                    break;
            }
            return Cell;
        }
        public static PdfPCell CreateCellSymbol(string[] _content, float _fontSize, int _colspan, float[] _padding, Border _border, Alignment_X _x, Alignment_Y _y, FontStyle[] _fontStyle, FontFamily[] _fontFamily, Color _color = Color.BLACK, BackgroundColor _backgroundColor = BackgroundColor.WHITE, float _lineHeight = 1f)
        {
            Phrase phrase = new Phrase();
            for (int i = 0; i < _content.Length; ++i)
            {
                phrase.Add(new Chunk(_content[i], new Font(CreateFontFamily(_fontFamily[i]), _fontSize, (int)_fontStyle[i], CreateColor(_color))));
            }
            PdfPCell Cell = new PdfPCell(phrase)
            {
                HorizontalAlignment = (int)_x,
                VerticalAlignment = (int)_y,
                BackgroundColor = CreateBackgroundColor(_backgroundColor),
                Colspan = _colspan,
                Border = (int)_border,
            };
            Cell.SetLeading(0f, _lineHeight);
            switch (_padding.Length)
            {
                case 1:
                    Cell.Padding = _padding[0];
                    break;
                case 4:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingRight = _padding[1];
                    Cell.PaddingBottom = _padding[2];
                    Cell.PaddingLeft = _padding[3];
                    break;
                case 2:
                    Cell.PaddingTop = _padding[0];
                    Cell.PaddingBottom = _padding[0];
                    Cell.PaddingLeft = _padding[1];
                    Cell.PaddingRight = _padding[1];
                    break;
                default:
                    break;
            }
            return Cell;
        }
        public static PdfPTable CreateTable(int column)
        {
            PdfPTable Table = new(column)
            {
                WidthPercentage = 100
            };
            float width = 100f / column;
            float[] columnWidth = Enumerable.Repeat(width, column).ToArray();
            Table.SetWidths(columnWidth);
            return Table;
        }
        public static PdfPCell CreateCell(string _content, string _BIU, float _fontSize, bool _border, string _candoc, string _canngang, int _colspan) // BIU: "B", "BI", "IU", "bui", "ub", ...
        {
            //Đặt font chữ
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Font/times.ttf");
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, true);
            //Định dạng in đậm, nghiêng, gạch chân
            int type = 0;
            if (_BIU.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) type += 1;
            if (_BIU.IndexOf('I', StringComparison.OrdinalIgnoreCase) > -1) type += 2;
            if (_BIU.IndexOf('U', StringComparison.OrdinalIgnoreCase) > -1) type += 4;
            PdfPCell Cell = new(new Phrase(_content, new Font(titleFont, _fontSize, type, new BaseColor(0, 0, 0))));
            //Viền
            if (!_border) Cell.Border = 0;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = 0;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = 2; //Element.ALIGN_RIGHT
            else Cell.HorizontalAlignment = 1;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = 4;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = 6;
            else Cell.VerticalAlignment = 5;
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
        public static PdfPCell CreateCell(string _content, string _BIU, float _fontSize, bool _border, string _candoc, string _canngang, int _colspan, float[] _padding) // BIU: "B", "BI", "IU", "bui", "ub", ...
        {
            //Đặt font chữ
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Font/times.ttf");
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, true);
            //Định dạng in đậm, nghiêng, gạch chân
            int type = 0;
            if (_BIU.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) type += 1;
            if (_BIU.IndexOf('I', StringComparison.OrdinalIgnoreCase) > -1) type += 2;
            if (_BIU.IndexOf('U', StringComparison.OrdinalIgnoreCase) > -1) type += 4;
            PdfPCell Cell = new(new Phrase(_content, new Font(titleFont, _fontSize, type, BaseColor.BLACK)));
            //Viền
            if (!_border) Cell.Border = 0;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = 0;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = 2;
            else Cell.HorizontalAlignment = 1;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_TOP;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            else Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                Cell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingBottom = _padding[0];
                Cell.PaddingLeft = _padding[1];
                Cell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingRight = _padding[1];
                Cell.PaddingBottom = _padding[2];
                Cell.PaddingLeft = _padding[3];
            }
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
        public static PdfPCell CreateCell(string[] _content, string[] _BIU, float _fontSize, bool _border, string _candoc, string _canngang, int _colspan)
        {
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Font/times.ttf");
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Phrase phrase = new Phrase();
            for (int i = 0; i < _content.Length; ++i)
            {
                int type = 0;
                if (_BIU[i].IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) type += 1;
                if (_BIU[i].IndexOf('I', StringComparison.OrdinalIgnoreCase) > -1) type += 2;
                if (_BIU[i].IndexOf('U', StringComparison.OrdinalIgnoreCase) > -1) type += 4;
                phrase.Add(new Chunk(_content[i], new Font(titleFont, _fontSize, type, BaseColor.BLACK)));
            }
            PdfPCell Cell = new PdfPCell(phrase);
            //Viền
            if (!_border) Cell.Border = PdfPCell.NO_BORDER;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            else Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_TOP;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            else Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
        public static PdfPCell CreateCell(string[] _content, string[] _BIU, float[] _fontSize, bool _border, string _candoc, string _canngang, int _colspan, float[] _padding)
        {
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Font/times.ttf");
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Phrase phrase = new Phrase();
            for (int i = 0; i < _content.Length; ++i)
            {
                int type = 0;
                if (_BIU[i].IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) type += 1;
                if (_BIU[i].IndexOf('I', StringComparison.OrdinalIgnoreCase) > -1) type += 2;
                if (_BIU[i].IndexOf('U', StringComparison.OrdinalIgnoreCase) > -1) type += 4;
                phrase.Add(new Chunk(_content[i], new Font(titleFont, _fontSize[i], type, BaseColor.BLACK)));
            }
            PdfPCell Cell = new PdfPCell(phrase);
            //Viền
            if (!_border) Cell.Border = PdfPCell.NO_BORDER;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            else Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_TOP;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            else Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                Cell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingBottom = _padding[0];
                Cell.PaddingLeft = _padding[1];
                Cell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingRight = _padding[1];
                Cell.PaddingBottom = _padding[2];
                Cell.PaddingLeft = _padding[3];
            }
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
        public static PdfPCell CreateCell(string[] _content, string[] _BIU, float _fontSize, bool _border, string _candoc, string _canngang, int _colspan, float[] _padding)
        {
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Font/times.ttf");
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Phrase phrase = new Phrase();
            for (int i = 0; i < _content.Length; ++i)
            {
                int type = 0;
                if (_BIU[i].IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) type += 1;
                if (_BIU[i].IndexOf('I', StringComparison.OrdinalIgnoreCase) > -1) type += 2;
                if (_BIU[i].IndexOf('U', StringComparison.OrdinalIgnoreCase) > -1) type += 4;
                phrase.Add(new Chunk(_content[i], new Font(titleFont, _fontSize, type, BaseColor.BLACK)));
            }
            PdfPCell Cell = new PdfPCell(phrase);
            //Viền
            if (!_border) Cell.Border = PdfPCell.NO_BORDER;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            else Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_TOP;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            else Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                Cell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingBottom = _padding[0];
                Cell.PaddingLeft = _padding[1];
                Cell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingRight = _padding[1];
                Cell.PaddingBottom = _padding[2];
                Cell.PaddingLeft = _padding[3];
            }
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
        public static PdfPCell CreateImageCell(string imagePathOrBase64, float _width, float _height, bool _border, int _colspan)
        {
            Image img;
            if (IsBase64String(imagePathOrBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imagePathOrBase64);
                using MemoryStream inputStream = new(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            else
            {
                imagePathOrBase64 ??= "";
                if (imagePathOrBase64.Length > 0 && imagePathOrBase64[0] == '/')
                {
                    imagePathOrBase64 = imagePathOrBase64[1..];
                }
                string pathImage = Path.Combine(Directory.GetCurrentDirectory(), imagePathOrBase64).Replace("\\", "/").Replace("//", "/");
                if (!File.Exists(pathImage))
                {
                    pathImage = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Image/default.png").Replace("\\", "/").Replace("//", "/");
                }
                if (Path.GetExtension(pathImage) == ".png")
                {
                    img = Image.GetInstance(pathImage);
                }
                else
                {
                    using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(pathImage);
                    using MemoryStream outputStream = new();
                    image.Save(outputStream, new PngEncoder());
                    byte[] pngBytes = outputStream.ToArray();
                    img = Image.GetInstance(pngBytes);
                }
            }
            img.ScaleToFit(_width, _height);
            PdfPCell imgCell = new PdfPCell(img);
            imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
            imgCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            imgCell.Colspan = _colspan;
            if (!_border)
            {
                imgCell.Border = PdfPCell.NO_BORDER;
            }
            return imgCell;
        }
        private static bool IsBase64String(string s)
        {
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static PdfPCell CreateImageCell(string imagePathOrBase64, float _width, float _height, bool _border, int _colspan, float[] _padding)
        {
            Image img;
            if (IsBase64String(imagePathOrBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imagePathOrBase64);
                using MemoryStream inputStream = new(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            else
            {
                imagePathOrBase64 ??= "";
                if (imagePathOrBase64.Length > 0 && imagePathOrBase64[0] == '/')
                {
                    imagePathOrBase64 = imagePathOrBase64[1..];
                }
                string pathImage = Path.Combine(Directory.GetCurrentDirectory(), imagePathOrBase64).Replace("\\", "/").Replace("//", "/");
                if (!File.Exists(pathImage))
                {
                    pathImage = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Image/default.png").Replace("\\", "/").Replace("//", "/");
                }
                if (Path.GetExtension(pathImage) == ".png")
                {
                    img = Image.GetInstance(pathImage);
                }
                else
                {
                    using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(pathImage);
                    using MemoryStream outputStream = new();
                    image.Save(outputStream, new PngEncoder());
                    byte[] pngBytes = outputStream.ToArray();
                    img = Image.GetInstance(pngBytes);
                }
            }
            img.ScaleToFit(_width, _height);
            PdfPCell imgCell = new PdfPCell(img);
            imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
            imgCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                imgCell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingBottom = _padding[0];
                imgCell.PaddingLeft = _padding[1];
                imgCell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingRight = _padding[1];
                imgCell.PaddingBottom = _padding[2];
                imgCell.PaddingLeft = _padding[3];
            }
            imgCell.Colspan = _colspan;
            if (!_border)
            {
                imgCell.Border = PdfPCell.NO_BORDER;
            }
            return imgCell;
        }
        public static PdfPCell CreateImageCellChuKyDanhGiaNhanSu(string imagePathOrBase64, float _width, bool _border, int _colspan, float[] _padding, bool canhLeTrai = false)
        {
            Image img;
            if (IsBase64String(imagePathOrBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imagePathOrBase64);
                using MemoryStream inputStream = new(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            else
            {
                imagePathOrBase64 ??= "";
                if (imagePathOrBase64.Length > 0 && imagePathOrBase64[0] == '/')
                {
                    imagePathOrBase64 = imagePathOrBase64[1..];
                }
                string pathImage = Path.Combine(Directory.GetCurrentDirectory(), imagePathOrBase64).Replace("\\", "/").Replace("//", "/");
                if (!File.Exists(pathImage))
                {
                    pathImage = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Image/default.png").Replace("\\", "/").Replace("//", "/");
                }
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(pathImage);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            float width = img.Width;
            float height = img.Height;
            img.ScaleToFit(_width, height * _width / width);
            PdfPCell imgCell = new(img)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            if (canhLeTrai) imgCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                imgCell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingBottom = _padding[0];
                imgCell.PaddingLeft = _padding[1];
                imgCell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingRight = _padding[1];
                imgCell.PaddingBottom = _padding[2];
                imgCell.PaddingLeft = _padding[3];
            }
            imgCell.Colspan = _colspan;
            if (!_border)
            {
                imgCell.Border = PdfPCell.NO_BORDER;
            }
            return imgCell;
        }
        public static PdfPCell CreateImageCell(string imagePathOrBase64, float _width, bool _border, int _colspan, float[] _padding)
        {
            Image img;
            if (IsBase64String(imagePathOrBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imagePathOrBase64);
                using MemoryStream inputStream = new(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            else
            {
                imagePathOrBase64 ??= "";
                if (imagePathOrBase64.Length > 0 && imagePathOrBase64[0] == '/')
                {
                    imagePathOrBase64 = imagePathOrBase64[1..];
                }
                string pathImage = Path.Combine(Directory.GetCurrentDirectory(), imagePathOrBase64).Replace("\\", "/").Replace("//", "/");
                if (!File.Exists(pathImage))
                {
                    pathImage = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Image/default.png").Replace("\\", "/").Replace("//", "/");
                }
                if (Path.GetExtension(pathImage) == ".png")
                {
                    img = Image.GetInstance(pathImage);
                }
                else
                {
                    using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(pathImage);
                    using MemoryStream outputStream = new();
                    image.Save(outputStream, new PngEncoder());
                    byte[] pngBytes = outputStream.ToArray();
                    img = Image.GetInstance(pngBytes);
                }
            }
            float width = img.Width;
            float height = img.Height;
            if (height > width && _width >= 120f)
            {
                return CreateImageCellScaleHeight(imagePathOrBase64, _width, _border, _colspan, _padding);
            }
            img.ScaleToFit(_width, height * _width / width);
            PdfPCell imgCell = new(img)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            //Padding
            if (_padding.Length == 1) //Chung
            {
                imgCell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingBottom = _padding[0];
                imgCell.PaddingLeft = _padding[1];
                imgCell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingRight = _padding[1];
                imgCell.PaddingBottom = _padding[2];
                imgCell.PaddingLeft = _padding[3];
            }
            imgCell.Colspan = _colspan;
            if (!_border)
            {
                imgCell.Border = PdfPCell.NO_BORDER;
            }
            return imgCell;
        }
        public static PdfPCell CreateImageCellScaleHeight(string imagePathOrBase64, float _height, bool _border, int _colspan, float[] _padding)
        {
            Image img;
            if (IsBase64String(imagePathOrBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imagePathOrBase64);
                using MemoryStream inputStream = new(imageBytes);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            else
            {
                imagePathOrBase64 ??= "";
                if (imagePathOrBase64.Length > 0 && imagePathOrBase64[0] == '/')
                {
                    imagePathOrBase64 = imagePathOrBase64[1..];
                }
                string pathImage = Path.Combine(Directory.GetCurrentDirectory(), imagePathOrBase64).Replace("\\", "/").Replace("//", "/");
                if (!File.Exists(pathImage))
                {
                    pathImage = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Image/default.png").Replace("\\", "/").Replace("//", "/");
                }
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(pathImage);
                using MemoryStream outputStream = new();
                image.Save(outputStream, new PngEncoder());
                byte[] pngBytes = outputStream.ToArray();
                img = Image.GetInstance(pngBytes);
            }
            float width = img.Width;
            float height = img.Height;
            if (height < width && _height >= 300f)
            {
                return CreateImageCell(imagePathOrBase64, _height, _border, _colspan, _padding);
            }
            img.ScaleToFit(width * _height / height, _height);
            PdfPCell imgCell = new PdfPCell(img);
            imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
            imgCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                imgCell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingBottom = _padding[0];
                imgCell.PaddingLeft = _padding[1];
                imgCell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                imgCell.PaddingTop = _padding[0];
                imgCell.PaddingRight = _padding[1];
                imgCell.PaddingBottom = _padding[2];
                imgCell.PaddingLeft = _padding[3];
            }
            imgCell.Colspan = _colspan;
            if (!_border)
            {
                imgCell.Border = PdfPCell.NO_BORDER;
            }
            return imgCell;
        }
        public static PdfPCell CreateCellKyTuDapAn(int thuTu, float _fontSize, bool _border, string _candoc, string _canngang, int _colspan, float[] _padding, bool isCorrect, int isChon) // BIU: "B", "BI", "IU", "bui", "ub", ...
        {
            //Đặt font chữ
            string fontPath = "Uploads/Font/times.ttf";
            var color = BaseColor.BLACK;
            string _content;
            if (thuTu < 1) thuTu = 1;
            else if (thuTu > 6) thuTu = 6;
            float line = 1.2f;
            if (isChon == 1)
            {
                string[] str = new string[] { "\u24B6", "\u24B7", "\u24B8", "\u24B9", "\u24BA", "\u24BB" }; //A,B,C,D,E,F, được khoanh tròn
                _content = str[thuTu - 1];
                _padding[0] = 0f;
                fontPath = "Uploads/Font/Symbola.ttf";
                _fontSize *= 1.5f;
                if (isCorrect) color = BaseColor.BLUE;
                else color = BaseColor.RED;
                line = 1.12f;
            }
            else
            {
                string[] str = new string[] { "A", "B", "C", "D", "E", "F" };
                _content = str[thuTu - 1];
            }
            BaseFont titleFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //Định dạng in đậm type = 1
            PdfPCell Cell = new(new Phrase(_content, new Font(titleFont, _fontSize, 1, color)));
            Cell.SetLeading(0f, line);
            //Viền
            if (!_border) Cell.Border = PdfPCell.NO_BORDER;
            //Căn ngang
            if (_canngang.IndexOf('L', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            else if (_canngang.IndexOf('R', StringComparison.OrdinalIgnoreCase) > -1) Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            else Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //Căn dọc
            if (_candoc.IndexOf('T', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_TOP;
            else if (_candoc.IndexOf('B', StringComparison.OrdinalIgnoreCase) > -1) Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            else Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //Padding
            if (_padding.Length == 1) //Chung
            {
                Cell.Padding = _padding[0];
            }
            else if (_padding.Length == 2) //Top-Bot Right-Left
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingBottom = _padding[0];
                Cell.PaddingLeft = _padding[1];
                Cell.PaddingRight = _padding[1];
            }
            else if (_padding.Length == 4) //Top Right Bottom Left (Chiều kim đồng hồ)
            {
                Cell.PaddingTop = _padding[0];
                Cell.PaddingRight = _padding[1];
                Cell.PaddingBottom = _padding[2];
                Cell.PaddingLeft = _padding[3];
            }
            //Gộp ô
            Cell.Colspan = _colspan;
            return Cell;
        }
    }
}