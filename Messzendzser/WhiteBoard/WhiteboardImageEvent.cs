﻿using Messzendzser.Model.Managers.Media;
using Org.BouncyCastle.Utilities;
using SkiaSharp;
using System.Text.Json;

namespace Messzendzser.WhiteBoard
{
    public class WhiteboardImageEvent : WhiteboardEvent
    {
        public WhiteboardImageEvent() : base(EventType.Image)
        {
        }

        public Whiteboard Board { get; set; }

        private SKBitmap HighlightWord(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);

            SKBitmap bitmap = SKBitmap.Decode(ms);
            var canvas = new SKCanvas(bitmap);
            

            return bitmap;
        }

        public override SKCanvas Draw(SKCanvas canvas)
        {
            MediaManager mm = new MediaManager();
            byte[] data = mm.LoadWhiteboard(Board.RoomId);

            var ms = new MemoryStream(data);

            SKBitmap bitmap = SKBitmap.Decode(ms);
            var image = new SKCanvas(bitmap);

            return image;
        }
    }
}
