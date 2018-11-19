using Glfw3;
using NanoVG;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using NVGColor = System.Numerics.Vector4;

namespace NetCoreNanoVG.Test
{
    class Program
    {
        [DllImport("opengl32")]
        static extern void glClear(int Mask = 16384 | 1024 | 256);

        static void Main(string[] args)
        {
            /*Generator.Generate();
			Console.WriteLine("Done!");
			Console.ReadLine();*/

            NVG.SetLibraryDirectory();
            Glfw.Init();

            Glfw.WindowHint(Glfw.Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Glfw.Hint.ContextVersionMinor, 2);
            Glfw.WindowHint(Glfw.Hint.OpenglForwardCompat, true);
            Glfw.WindowHint(Glfw.Hint.OpenglProfile, Glfw.OpenGLProfile.Core);

            Glfw.WindowHint(Glfw.Hint.Samples, 16);

            Glfw.Window Wnd = Glfw.CreateWindow(1024, 700, "NanoVG.NET");
            Glfw.MakeContextCurrent(Wnd);
            Glfw.GetFramebufferSize(Wnd, out int FbW, out int FbH);

            NanoVGContext Ctx = NVG.CreateGL3Glew(3);
            Glfw.SwapInterval(0);

            int Icons = Ctx.CreateFont("icons", "data/fonts/entypo.ttf");
            int Sans = Ctx.CreateFont("sans", "data/fonts/Roboto-Regular.ttf");
            int SansBold = Ctx.CreateFont("sans-bold", "data/fonts/Roboto-Bold.ttf");
            int Emoji = Ctx.CreateFont("emoji", "data/fonts/NotoEmoji-Regular.ttf");

            Ctx.AddFallbackFontId(Sans, Emoji);
            Ctx.AddFallbackFontId(SansBold, Emoji);

            while (!Glfw.WindowShouldClose(Wnd))
            {
                glClear();
                Ctx.BeginFrame(1024, 700, (float)FbW / FbH);

                Demo(Ctx);

                Ctx.EndFrame();
                Glfw.SwapBuffers(Wnd);
                Glfw.PollEvents();
            }
        }

        static void Demo(NanoVGContext vg)
        {
            int x = 0;
            int y = 0;

            x = 60;
            y = 95;

            int sWinX = 650;
            int sWinY = 50;

            // Widgets

            DrawColorWheel(vg, 400, 50, 200, 200, 1);
            DemoData data = new DemoData();
            LoadDemoData(vg, ref data);
            DrawThumbnails(vg, 400, 250, 200, 300, data.images, 12, 10);

            DrawWindow(vg, "Left Widgets Stuff", 50, 50, 300, 600);

            DrawSearchBox(vg, "Search ", x, y, 200, 25);

            DrawLabel(vg, "OMG Label", x, 120, 200, 50);
            DrawDropDown(vg, "My Dropdown", x, 170, 200, 25);
            DrawEditBox(vg, "Edit Text", x, 200, 200, 30);
            DrawEditBoxNum(vg, "0123455", new byte[] { 1 }, x, 240, 200, 30);
            DrawCheckBox(vg, "Check 1", x, 280, 200, 20);
            DrawButton(vg, 1, "Click me!", x, 300, 200, 30, NVG.RGB(10,10,10));
            DrawSlider(vg, 0.4f, x, 340, 200, 30);

            DrawWindow(vg, "Right Widgets", sWinX, sWinY, 300, 600);
            DrawEyes(vg, sWinX+10, sWinY+10, 100, 50, 10, 10, 20);
            DrawGraph(vg, 0, 360, 1024, 350, 500);
            DrawSpinner(vg, 800, 150, 60, 100);
            DrawLines(vg,sWinX+10, sWinY+100,200,10,10);
            //DrawParagraph(vg, sWinX + 10, sWinY + 200, 200, 100, 10f, 10f);

        }

        const int ICON_SEARCH = 0x1F50D;
        const int ICON_CIRCLED_CROSS = 0x2716;
        const int ICON_CHEVRON_RIGHT = 0xE75E;
        const int ICON_CHECK = 0x2713;
        const int ICON_LOGIN = 0xE740;
        const int ICON_TRASH = 0xE729;
        const float NVG_PI = 3.14f;

        struct DemoData
        {
            public int fontNormal, fontBold, fontIcons, fontEmoji;
            public int[] images;
        };

        static int LoadDemoData(NanoVGContext vg, ref DemoData data)
        {
            int i;

            data.images = new int[13];

            for (i = 0; i < 12; i++)
            {
                string file;
                file = $"data/images/image{i + 1}.jpg";                
                data.images[i] = vg.CreateImage(file, 0);
                if (data.images[i] == 0)
                {
                    Console.WriteLine("Could not load %s.\n", file);
                    return -1;
                }
            }

            data.fontIcons = NVG.CreateFont(vg, "icons", "data/fonts/entypo.ttf");
            if (data.fontIcons == -1)
            {
                Console.WriteLine("Could not add font icons.\n");
                return -1;
            }
            data.fontNormal = NVG.CreateFont(vg, "sans", "data/fonts/Roboto-Regular.ttf");
            if (data.fontNormal == -1)
            {
                Console.WriteLine("Could not add font italic.\n");
                return -1;
            }
            data.fontBold = NVG.CreateFont(vg, "sans-bold", "data/fonts/Roboto-Bold.ttf");
            if (data.fontBold == -1)
            {
                Console.WriteLine("Could not add font bold.\n");
                return -1;
            }
            data.fontEmoji = NVG.CreateFont(vg, "emoji", "data/fonts/NotoEmoji-Regular.ttf");
            if (data.fontEmoji == -1)
            {
                Console.WriteLine("Could not add font emoji.\n");
                return -1;
            }

            NVG.AddFallbackFontId(vg, data.fontNormal, data.fontEmoji);
            NVG.AddFallbackFontId(vg, data.fontBold, data.fontEmoji);

            return 0;
        }

        static bool isBlack(NVGColor col)
        {
            if (col.X == 0.0f && col.Y == 0.0f && col.Z == 0.0f && col.W == 0.0f)
            {
                return true;
            }
            return false;
        }

        static byte[] cpToUTF8(int cp)
        {
            byte[] str = new byte[8];
            int n = 0;

            if (cp < 0x80)
                n = 1;
            else if (cp < 0x800)
                n = 2;
            else if (cp < 0x10000)
                n = 3;
            else if (cp < 0x200000)
                n = 4;
            else if (cp < 0x4000000)
                n = 5;
            else if (cp <= 0x7fffffff)
                n = 6;

            str[n] = 0;

            switch (n)
            {
                case 6:
                    str[5] = (byte)(0x80 | (cp & 0x3f));
                    cp = cp >> 6;
                    cp |= 0x4000000;
                    goto case 5;

                case 5:
                    str[4] = (byte)(0x80 | (cp & 0x3f));
                    cp = cp >> 6;
                    cp |= 0x200000;
                    goto case 4;

                case 4:
                    str[3] = (byte)(0x80 | (cp & 0x3f));
                    cp = cp >> 6;
                    cp |= 0x10000;
                    goto case 3;

                case 3:
                    str[2] = (byte)(0x80 | (cp & 0x3f));
                    cp = cp >> 6;
                    cp |= 0x800;
                    goto case 2;

                case 2:
                    str[1] = (byte)(0x80 | (cp & 0x3f));
                    cp = cp >> 6;
                    cp |= 0xc0;
                    goto case 1;

                case 1:
                    str[0] = (byte)cp;
                    break;
            }
            return str;
        }

        static void DrawWindow(NanoVGContext Ctx, string Title, float X, float Y, float W, float H)
        {
            float CornerRadius = 3;
            NVGPaint ShadowPaint;
            NVGPaint HeaderPaint;
            Ctx.Save();

            // Window
            Ctx.BeginPath();
            Ctx.RoundedRect(X, Y, W, H, CornerRadius);
            Ctx.FillColor(NVG.RGBA(28, 30, 34, 192));
            Ctx.Fill();

            // Drop shadow
            ShadowPaint = Ctx.BoxGradient(X, Y + 2, W, H, CornerRadius * 2, 10, NVG.RGBA(0, 0, 0, 128), NVG.RGBA(0, 0, 0, 0));
            Ctx.BeginPath();
            Ctx.Rect(X - 10, Y - 10, W + 20, H + 30);
            Ctx.RoundedRect(X, Y, W, H, CornerRadius);
            Ctx.PathWinding(NVGSolidity.NVG_HOLE);
            Ctx.FillPaint(ShadowPaint);
            Ctx.Fill();

            // Header
            HeaderPaint = Ctx.LinearGradient(X, Y, X, Y + 15, NVG.RGBA(255, 255, 255, 8), NVG.RGBA(0, 0, 0, 16));
            Ctx.BeginPath();
            Ctx.RoundedRect(X + 1, Y + 1, W - 2, 30, CornerRadius - 1);
            Ctx.FillPaint(HeaderPaint);
            Ctx.Fill();
            Ctx.BeginPath();
            Ctx.MoveTo(X + 0.5f, Y + 0.5f + 30);
            Ctx.LineTo(X + 0.5f + W - 1, Y + 0.5f + 30);
            Ctx.StrokeColor(NVG.RGBA(0, 0, 0, 32));
            Ctx.Stroke();

            Ctx.FontSize(18.0f);
            Ctx.FontFace("sans-bold");
            Ctx.TextAlign(NVGAlign.NVG_ALIGN_CENTER | NVGAlign.NVG_ALIGN_MIDDLE);

            Ctx.FontBlur(2);
            Ctx.FillColor(NVG.RGBA(0, 0, 0, 128));
            Ctx.Text(X + W / 2, Y + 16 + 1, Title, null);

            Ctx.FontBlur(0);
            Ctx.FillColor(NVG.RGBA(220, 220, 220, 160));
            Ctx.Text(X + W / 2, Y + 16, Title, null);

            Ctx.Restore();
        }

        static void DrawSearchBox(NanoVGContext vg, string text, float x, float y, float w, float h)
        {
            NVGPaint bg;
            float cornerRadius = h / 2 - 1;

            // Edit
            bg = NVG.BoxGradient(vg, x, y + 1.5f, w, h, h / 2, 5, NVG.RGBA(0, 0, 0, 16), NVG.RGBA(0, 0, 0, 92));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x, y, w, h, cornerRadius);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            NVG.FontSize(vg, h * 1.3f);
            NVG.FontFace(vg, "icons");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 64));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_CENTER | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + h * 0.55f, y + h * 0.55f, cpToUTF8(ICON_SEARCH), null);

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 32));

            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + h * 1.05f, y + h * 0.5f, text, null);

            NVG.FontSize(vg, h * 1.3f);
            NVG.FontFace(vg, "icons");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 32));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_CENTER | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + w - h * 0.55f, y + h * 0.55f, cpToUTF8(ICON_CIRCLED_CROSS), null);
        }

        static void DrawColorWheel(NanoVGContext vg, float x, float y, float w, float h, float t)
        {
            int i;
            float r0, r1, ax, ay, bx, by, cx, cy, aeps, r;
            float hue = (float)Math.Sin(t * 0.12f);
            NVGPaint paint;

            NVG.Save(vg);

            cx = x + w * 0.5f;
            cy = y + h * 0.5f;
            r1 = (w < h ? w : h) * 0.5f - 5.0f;
            r0 = r1 - 20.0f;
            aeps = 0.5f / r1;   // half a pixel arc length in radians (2pi cancels out).

            for (i = 0; i < 6; i++)
            {
                float a0 = (float)i / 6.0f * (float)Math.PI * 2.0f - aeps;
                float a1 = (float)(i + 1.0f) / 6.0f * (float)Math.PI * 2.0f + aeps;
                NVG.BeginPath(vg);
                NVG.Arc(vg, cx, cy, r0, a0, a1, NVGWinding.NVG_CW);
                NVG.Arc(vg, cx, cy, r1, a1, a0, NVGWinding.NVG_CCW);
                NVG.ClosePath(vg);
                ax = cx + (float)Math.Cos(a0) * (r0 + r1) * 0.5f;
                ay = cy + (float)Math.Sin(a0) * (r0 + r1) * 0.5f;
                bx = cx + (float)Math.Cos(a1) * (r0 + r1) * 0.5f;
                by = cy + (float)Math.Sin(a1) * (r0 + r1) * 0.5f;
                paint = NVG.LinearGradient(vg, ax, ay, bx, by, NVG.HSLA(a0 / ((float)Math.PI * 2), 1.0f, 0.55f, 255), NVG.HSLA(a1 / ((float)Math.PI * 2), 1.0f, 0.55f, 255));
                NVG.FillPaint(vg, paint);
                NVG.Fill(vg);
            }

            NVG.BeginPath(vg);
            NVG.Circle(vg, cx, cy, r0 - 0.5f);
            NVG.Circle(vg, cx, cy, r1 + 0.5f);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 64));
            NVG.StrokeWidth(vg, 1.0f);
            NVG.Stroke(vg);

            // Selector
            NVG.Save(vg);
            NVG.Translate(vg, cx, cy);
            NVG.Rotate(vg, hue * (float)Math.PI * 2);

            // Marker on
            NVG.StrokeWidth(vg, 2.0f);
            NVG.BeginPath(vg);
            NVG.Rect(vg, r0 - 1, -3, r1 - r0 + 2, 6);
            NVG.StrokeColor(vg, NVG.RGBA(255, 255, 255, 192));
            NVG.Stroke(vg);

            paint = NVG.BoxGradient(vg, r0 - 3, -5, r1 - r0 + 6, 10, 2, 4, NVG.RGBA(0, 0, 0, 128), NVG.RGBA(0, 0, 0, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, r0 - 2 - 10, -4 - 10, r1 - r0 + 4 + 20, 8 + 20);
            NVG.Rect(vg, r0 - 2, -4, r1 - r0 + 4, 8);
            NVG.PathWinding(vg, NVGSolidity.NVG_HOLE);
            NVG.FillPaint(vg, paint);
            NVG.Fill(vg);

            // Center triangle
            r = r0 - 6;
            ax = (float)Math.Cos(120.0f / 180.0f * (float)Math.PI) * r;
            ay = (float)Math.Sin(120.0f / 180.0f * (float)Math.PI) * r;
            bx = (float)Math.Cos(-120.0f / 180.0f * (float)Math.PI) * r;
            by = (float)Math.Sin(-120.0f / 180.0f * (float)Math.PI) * r;
            NVG.BeginPath(vg);
            NVG.MoveTo(vg, r, 0);
            NVG.LineTo(vg, ax, ay);
            NVG.LineTo(vg, bx, by);
            NVG.ClosePath(vg);
            paint = NVG.LinearGradient(vg, r, 0, ax, ay, NVG.HSLA(hue, 1.0f, 0.5f, 255), NVG.RGBA(255, 255, 255, 255));
            NVG.FillPaint(vg, paint);
            NVG.Fill(vg);
            paint = NVG.LinearGradient(vg, (r + ax) * 0.5f, (0 + ay) * 0.5f, bx, by, NVG.RGBA(0, 0, 0, 0), NVG.RGBA(0, 0, 0, 255));
            NVG.FillPaint(vg, paint);
            NVG.Fill(vg);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 64));
            NVG.Stroke(vg);

            // Select circle on triangle
            ax = (float)Math.Cos(120.0f / 180.0f * (float)Math.PI) * r * 0.3f;
            ay = (float)Math.Sin(120.0f / 180.0f * (float)Math.PI) * r * 0.4f;
            NVG.StrokeWidth(vg, 2.0f);
            NVG.BeginPath(vg);
            NVG.Circle(vg, ax, ay, 5);
            NVG.StrokeColor(vg, NVG.RGBA(255, 255, 255, 192));
            NVG.Stroke(vg);

            paint = NVG.RadialGradient(vg, ax, ay, 7, 9, NVG.RGBA(0, 0, 0, 64), NVG.RGBA(0, 0, 0, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, ax - 20, ay - 20, 40, 40);
            NVG.Circle(vg, ax, ay, 7);
            NVG.PathWinding(vg, NVGSolidity.NVG_HOLE);
            NVG.FillPaint(vg, paint);
            NVG.Fill(vg);

            NVG.Restore(vg);

            NVG.Restore(vg);
        }

        static void DrawLabel(NanoVGContext vg, string text, float x, float y, float w, float h)
        {

            //NVG_NOTUSED(w);

            NVG.FontSize(vg, 18.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 128));

            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x, y + h * 0.5f, text, null);
        }

        static void DrawDropDown(NanoVGContext vg, string text, float x, float y, float w, float h)
        {
            NVGPaint bg;
            const int icon = 0x2713;
            float cornerRadius = 4.0f;

            bg = NVG.LinearGradient(vg, x, y, x, y + h, NVG.RGBA(255, 255, 255, 16), NVG.RGBA(0, 0, 0, 16));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 1, y + 1, w - 2, h - 2, cornerRadius - 1);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 0.5f, y + 0.5f, w - 1, h - 1, cornerRadius - 0.5f);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 48));
            NVG.Stroke(vg);

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 160));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + h * 0.3f, y + h * 0.5f, text, null);

            NVG.FontSize(vg, h * 1.3f);
            NVG.FontFace(vg, "icons");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 64));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_CENTER | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + w - h * 0.5f, y + h * 0.5f, cpToUTF8(icon), null);
        }

        static void DrawEditBoxBase(NanoVGContext vg, float x, float y, float w, float h)
        {
            NVGPaint bg;
            // Edit
            bg = NVG.BoxGradient(vg, x + 1, y + 1 + 1.5f, w - 2, h - 2, 3, 4, NVG.RGBA(255, 255, 255, 32), NVG.RGBA(32, 32, 32, 32));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 1, y + 1, w - 2, h - 2, 4 - 1);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 0.5f, y + 0.5f, w - 1, h - 1, 4 - 0.5f);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 48));
            NVG.Stroke(vg);
        }

        static void DrawEditBox(NanoVGContext vg, string text, float x, float y, float w, float h)
        {

            DrawEditBoxBase(vg, x, y, w, h);

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 64));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + h * 0.3f, y + h * 0.5f, text, null);
        }

        static unsafe void DrawEditBoxNum(NanoVGContext vg, string text, byte[] units, float x, float y, float w, float h)
        {
            float uw;

            DrawEditBoxBase(vg, x, y, w, h);

            uw = NVG.TextBounds(vg, 0, 0, units, null, null);

            NVG.FontSize(vg, 18.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 64));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_RIGHT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + w - h * 0.3f, y + h * 0.5f, units, null);

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 128));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_RIGHT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + w - uw - h * 0.5f, y + h * 0.5f, text, null);
        }

        static void DrawCheckBox(NanoVGContext vg, string text, float x, float y, float w, float h)
        {
            NVGPaint bg;
            int icon = 0x2713;
            //NVG_NOTUSED(w);

            NVG.FontSize(vg, 18.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 160));

            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + 28, y + h * 0.5f, text, null);

            bg = NVG.BoxGradient(vg, x + 1, y + (int)(h * 0.5f) - 9 + 1, 18, 18, 3, 3, NVG.RGBA(0, 0, 0, 32), NVG.RGBA(0, 0, 0, 92));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 1, y + (int)(h * 0.5f) - 9, 18, 18, 3);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            NVG.FontSize(vg, 40);
            NVG.FontFace(vg, "icons");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 128));
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_CENTER | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x + 9 + 2, y + h * 0.5f, cpToUTF8(icon), null);
        }

        static unsafe void DrawButton(NanoVGContext vg, int preicon, string text, float x, float y, float w, float h, NVGColor col)
        {
            
            NVGPaint bg;
            int icon = 0xE740;
            float cornerRadius = 4.0f;
            float tw = 0, iw = 0;

            bg = NVG.LinearGradient(vg, x, y, x, y + h, NVG.RGBA(255, 255, 255, isBlack(col) ? (byte)16 : (byte)32), NVG.RGBA(0, 0, 0, isBlack(col) ? (byte)16 : (byte)32));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 1, y + 1, w - 2, h - 2, cornerRadius - 1);
            if (!isBlack(col))
            {
                NVG.FillColor(vg, col);
                NVG.Fill(vg);
            }
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + 0.5f, y + 0.5f, w - 1, h - 1, cornerRadius - 0.5f);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 48));
            NVG.Stroke(vg);

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans-bold");
            tw = NVG.TextBounds(vg, 0, 0, Encoding.UTF8.GetBytes(text), null, null);
            if (preicon != 0)
            {
                NVG.FontSize(vg, h * 1.3f);
                NVG.FontFace(vg, "icons");
                iw = NVG.TextBounds(vg, 0, 0, cpToUTF8(icon), null, null);
                iw += h * 0.15f;
            }

            if (preicon != 0)
            {
                NVG.FontSize(vg, h * 1.3f);
                NVG.FontFace(vg, "icons");
                NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 96));
                NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
                NVG.Text(vg, x + w * 0.5f - tw * 0.5f - iw * 0.75f, y + h * 0.5f, cpToUTF8(icon), null);
            }

            NVG.FontSize(vg, 20.0f);
            NVG.FontFace(vg, "sans-bold");
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.FillColor(vg, NVG.RGBA(0, 0, 0, 160));
            NVG.Text(vg, x + w * 0.5f - tw * 0.5f + iw * 0.25f, y + h * 0.5f - 1, text, null);
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 160));
            NVG.Text(vg, x + w * 0.5f - tw * 0.5f + iw * 0.25f, y + h * 0.5f, text, null);
        }

        static void DrawSlider(NanoVGContext vg, float pos, float x, float y, float w, float h)
        {
            NVGPaint bg, knob;
            float cy = y + (int)(h * 0.5f);
            float kr = (int)(h * 0.25f);

            NVG.Save(vg);
            //	NVG.ClearState(vg);

            // Slot
            bg = NVG.BoxGradient(vg, x, cy - 2 + 1, w, 4, 2, 2, NVG.RGBA(0, 0, 0, 32), NVG.RGBA(0, 0, 0, 128));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x, cy - 2, w, 4, 2);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            // Knob Shadow
            bg = NVG.RadialGradient(vg, x + (int)(pos * w), cy + 1, kr - 3, kr + 3, NVG.RGBA(0, 0, 0, 64), NVG.RGBA(0, 0, 0, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, x + (int)(pos * w) - kr - 5, cy - kr - 5, kr * 2 + 5 + 5, kr * 2 + 5 + 5 + 3);
            NVG.Circle(vg, x + (int)(pos * w), cy, kr);
            NVG.PathWinding(vg, NVGSolidity.NVG_HOLE);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            // Knob
            knob = NVG.LinearGradient(vg, x, cy - kr, x, cy + kr, NVG.RGBA(255, 255, 255, 16), NVG.RGBA(0, 0, 0, 16));
            NVG.BeginPath(vg);
            NVG.Circle(vg, x + (int)(pos * w), cy, kr - 1);
            NVG.FillColor(vg, NVG.RGBA(40, 43, 48, 255));
            NVG.Fill(vg);
            NVG.FillPaint(vg, knob);
            NVG.Fill(vg);

            NVG.BeginPath(vg);
            NVG.Circle(vg, x + (int)(pos * w), cy, kr - 0.5f);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 92));
            NVG.Stroke(vg);

            NVG.Restore(vg);
        }

        static void DrawEyes(NanoVGContext vg, float x, float y, float w, float h, float mx, float my, float t)
        {
            NVGPaint gloss, bg;
            float ex = w * 0.23f;
            float ey = h * 0.5f;
            float lx = x + ex;
            float ly = y + ey;
            float rx = x + w - ex;
            float ry = y + ey;
            float dx, dy, d;
            float br = (ex < ey ? ex : ey) * 0.5f;
            float blink = 1 - (float) (Math.Pow(Math.Sin(t * 0.5f), 200) * 0.8f);

            bg = NVG.LinearGradient(vg, x, y + h * 0.5f, x + w * 0.1f, y + h, NVG.RGBA(0, 0, 0, 32), NVG.RGBA(0, 0, 0, 16));
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, lx + 3.0f, ly + 16.0f, ex, ey);
            NVG.Ellipse(vg, rx + 3.0f, ry + 16.0f, ex, ey);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            bg = NVG.LinearGradient(vg, x, y + h * 0.25f, x + w * 0.1f, y + h, NVG.RGBA(220, 220, 220, 255), NVG.RGBA(128, 128, 128, 255));
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, lx, ly, ex, ey);
            NVG.Ellipse(vg, rx, ry, ex, ey);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            dx = (mx - rx) / (ex * 10);
            dy = (my - ry) / (ey * 10);
            d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d > 1.0f)
            {
                dx /= d; dy /= d;
            }
            dx *= ex * 0.4f;
            dy *= ey * 0.5f;
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, lx + dx, ly + dy + ey * 0.25f * (1 - blink), br, br * blink);
            NVG.FillColor(vg, NVG.RGBA(32, 32, 32, 255));
            NVG.Fill(vg);

            dx = (mx - rx) / (ex * 10);
            dy = (my - ry) / (ey * 10);
            d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d > 1.0f)
            {
                dx /= d; dy /= d;
            }
            dx *= ex * 0.4f;
            dy *= ey * 0.5f;
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, rx + dx, ry + dy + ey * 0.25f * (1 - blink), br, br * blink);
            NVG.FillColor(vg, NVG.RGBA(32, 32, 32, 255));
            NVG.Fill(vg);

            gloss = NVG.RadialGradient(vg, lx - ex * 0.25f, ly - ey * 0.5f, ex * 0.1f, ex * 0.75f, NVG.RGBA(255, 255, 255, 128), NVG.RGBA(255, 255, 255, 0));
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, lx, ly, ex, ey);
            NVG.FillPaint(vg, gloss);
            NVG.Fill(vg);

            gloss = NVG.RadialGradient(vg, rx - ex * 0.25f, ry - ey * 0.5f, ex * 0.1f, ex * 0.75f, NVG.RGBA(255, 255, 255, 128), NVG.RGBA(255, 255, 255, 0));
            NVG.BeginPath(vg);
            NVG.Ellipse(vg, rx, ry, ex, ey);
            NVG.FillPaint(vg, gloss);
            NVG.Fill(vg);
        }

        static void DrawGraph(NanoVGContext vg, float x, float y, float w, float h, float t)
        {
            NVGPaint bg;
            float[] samples = new float[6];
            float[] sx = new float[6], sy = new float[6];
            float dx = w / 5.0f;
            int i;

            samples[0] = (1 + (float)Math.Sin(t * 1.2345f + Math.Cos(t * 0.33457f) * 0.44f)) * 0.5f;
            samples[1] = (1 + (float)Math.Sin(t * 0.68363f + Math.Cos(t * 1.3f) * 1.55f)) * 0.5f;
            samples[2] = (1 + (float)Math.Sin(t * 1.1642f + Math.Cos(t * 0.33457) * 1.24f)) * 0.5f;
            samples[3] = (1 + (float)Math.Sin(t * 0.56345f + Math.Cos(t * 1.63f) * 0.14f)) * 0.5f;
            samples[4] = (1 + (float)Math.Sin(t * 1.6245f + Math.Cos(t * 0.254f) * 0.3f)) * 0.5f;
            samples[5] = (1 + (float)Math.Sin(t * 0.345f + Math.Cos(t * 0.03f) * 0.6f)) * 0.5f;

            for (i = 0; i < 6; i++)
            {
                sx[i] = x + i * dx;
                sy[i] = y + h * samples[i] * 0.8f;
            }

            // Graph background
            bg = NVG.LinearGradient(vg, x, y, x, y + h, NVG.RGBA(0, 160, 192, 0), NVG.RGBA(0, 160, 192, 64));
            NVG.BeginPath(vg);
            NVG.MoveTo(vg, sx[0], sy[0]);
            for (i = 1; i < 6; i++)
                NVG.BezierTo(vg, sx[i - 1] + dx * 0.5f, sy[i - 1], sx[i] - dx * 0.5f, sy[i], sx[i], sy[i]);
            NVG.LineTo(vg, x + w, y + h);
            NVG.LineTo(vg, x, y + h);
            NVG.FillPaint(vg, bg);
            NVG.Fill(vg);

            // Graph line
            NVG.BeginPath(vg);
            NVG.MoveTo(vg, sx[0], sy[0] + 2);
            for (i = 1; i < 6; i++)
                NVG.BezierTo(vg, sx[i - 1] + dx * 0.5f, sy[i - 1] + 2, sx[i] - dx * 0.5f, sy[i] + 2, sx[i], sy[i] + 2);
            NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 32));
            NVG.StrokeWidth(vg, 3.0f);
            NVG.Stroke(vg);

            NVG.BeginPath(vg);
            NVG.MoveTo(vg, sx[0], sy[0]);
            for (i = 1; i < 6; i++)
                NVG.BezierTo(vg, sx[i - 1] + dx * 0.5f, sy[i - 1], sx[i] - dx * 0.5f, sy[i], sx[i], sy[i]);
            NVG.StrokeColor(vg, NVG.RGBA(0, 160, 192, 255));
            NVG.StrokeWidth(vg, 3.0f);
            NVG.Stroke(vg);

            // Graph sample pos
            for (i = 0; i < 6; i++)
            {
                bg = NVG.RadialGradient(vg, sx[i], sy[i] + 2, 3.0f, 8.0f, NVG.RGBA(0, 0, 0, 32), NVG.RGBA(0, 0, 0, 0));
                NVG.BeginPath(vg);
                NVG.Rect(vg, sx[i] - 10, sy[i] - 10 + 2, 20, 20);
                NVG.FillPaint(vg, bg);
                NVG.Fill(vg);
            }

            NVG.BeginPath(vg);
            for (i = 0; i < 6; i++)
                NVG.Circle(vg, sx[i], sy[i], 4.0f);
            NVG.FillColor(vg, NVG.RGBA(0, 160, 192, 255));
            NVG.Fill(vg);
            NVG.BeginPath(vg);
            for (i = 0; i < 6; i++)
                NVG.Circle(vg, sx[i], sy[i], 2.0f);
            NVG.FillColor(vg, NVG.RGBA(220, 220, 220, 255));
            NVG.Fill(vg);

            NVG.StrokeWidth(vg, 1.0f);
        }

        static void DrawSpinner(NanoVGContext vg, float cx, float cy, float r, float t)
        {
            float a0 = 0.0f + t * 6;
            float a1 =  3.14f + t * 6f;
            float r0 = r;
            float r1 = r * 0.75f;
            float ax, ay, bx, by;
            NVGPaint paint;

            NVG.Save(vg);

            NVG.BeginPath(vg);
            NVG.Arc(vg, cx, cy, r0, a0, a1, NVGWinding.NVG_CW);
            NVG.Arc(vg, cx, cy, r1, a1, a0, NVGWinding.NVG_CCW);
            NVG.ClosePath(vg);
            ax = cx + (float)Math.Cos(a0) * (r0 + r1) * 0.5f;
            ay = cy + (float)Math.Sin(a0) * (r0 + r1) * 0.5f;
            bx = cx + (float)Math.Cos(a1) * (r0 + r1) * 0.5f;
            by = cy + (float)Math.Sign(a1) * (r0 + r1) * 0.5f;
            paint = NVG.LinearGradient(vg, ax, ay, bx, by, NVG.RGBA(0, 0, 0, 0), NVG.RGBA(0, 0, 0, 128));
            NVG.FillPaint(vg, paint);
            NVG.Fill(vg);

            NVG.Restore(vg);
        }

        static unsafe void DrawThumbnails(NanoVGContext vg, float x, float y, float w, float h, int[] images, int nimages, float t)
        {
            float cornerRadius = 3.0f;
            NVGPaint shadowPaint, imgPaint, fadePaint;
            float ix, iy, iw, ih;
            float thumb = 60.0f;
            float arry = 30.5f;
            int imgw, imgh;
            float stackh = (nimages / 2) * (thumb + 10) + 10;
            int i;
            float u = (1 + (float)Math.Cos(t * 0.5f)) * 0.5f;
            float u2 = (1 - (float)Math.Cos(t * 0.2f)) * 0.5f;
            float scrollh, dv;

            NVG.Save(vg);
            //	nvgClearState(vg);

            // Drop shadow
            shadowPaint = NVG.BoxGradient(vg, x, y + 4, w, h, cornerRadius * 2, 20, NVG.RGBA(0, 0, 0, 128), NVG.RGBA(0, 0, 0, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, x - 10, y - 10, w + 20, h + 30);
            NVG.RoundedRect(vg, x, y, w, h, cornerRadius);
            NVG.PathWinding(vg,NVGSolidity.NVG_HOLE);
            NVG.FillPaint(vg, shadowPaint);
            NVG.Fill(vg);

            // Window
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x, y, w, h, cornerRadius);
            NVG.MoveTo(vg, x - 10, y + arry);
            NVG.LineTo(vg, x + 1, y + arry - 11);
            NVG.LineTo(vg, x + 1, y + arry + 11);
            NVG.FillColor(vg, NVG.RGBA(200, 200, 200, 255));
            NVG.Fill(vg);

            NVG.Save(vg);
            NVG.Scissor(vg, x, y, w, h);
            NVG.Translate(vg, 0, -(stackh - h) * u);

            dv = 1.0f / (float)(nimages - 1);

            for (i = 0; i < nimages; i++)
            {
                float tx, ty, v, a;
                tx = x + 10;
                ty = y + 10;
                tx += (i % 2) * (thumb + 10);
                ty += (i / 2) * (thumb + 10);
                NVG.ImageSize(vg, images[i], &imgw, &imgh);
                if (imgw < imgh)
                {
                    iw = thumb;
                    ih = iw * (float)imgh / (float)imgw;
                    ix = 0;
                    iy = -(ih - thumb) * 0.5f;
                }
                else
                {
                    ih = thumb;
                    iw = ih * (float)imgw / (float)imgh;
                    ix = -(iw - thumb) * 0.5f;
                    iy = 0;
                }

                v = i * dv;
                a = Math.Clamp((u2 - v) / dv, 0, 1);

                if (a < 1.0f)
                    DrawSpinner(vg, tx + thumb / 2, ty + thumb / 2, thumb * 0.25f, t);

                imgPaint = NVG.ImagePattern(vg, tx + ix, ty + iy, iw, ih, 0.0f / 180.0f * NVG_PI, images[i], a);
                NVG.BeginPath(vg);
                NVG.RoundedRect(vg, tx, ty, thumb, thumb, 5);
                NVG.FillPaint(vg, imgPaint);
                NVG.Fill(vg);

                shadowPaint = NVG.BoxGradient(vg, tx - 1, ty, thumb + 2, thumb + 2, 5, 3, NVG.RGBA(0, 0, 0, 128), NVG.RGBA(0, 0, 0, 0));
                NVG.BeginPath(vg);
                NVG.Rect(vg, tx - 5, ty - 5, thumb + 10, thumb + 10);
                NVG.RoundedRect(vg, tx, ty, thumb, thumb, 6);
                NVG.PathWinding(vg, NVGSolidity.NVG_HOLE);
                NVG.FillPaint(vg, shadowPaint);
                NVG.Fill(vg);

                NVG.BeginPath(vg);
                NVG.RoundedRect(vg, tx + 0.5f, ty + 0.5f, thumb - 1, thumb - 1, 4 - 0.5f);
                NVG.StrokeWidth(vg, 1.0f);
                NVG.StrokeColor(vg, NVG.RGBA(255, 255, 255, 192));
                NVG.Stroke(vg);
            }
            NVG.Restore(vg);

            // Hide fades
            fadePaint = NVG.LinearGradient(vg, x, y, x, y + 6, NVG.RGBA(200, 200, 200, 255), NVG.RGBA(200, 200, 200, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, x + 4, y, w - 8, 6);
            NVG.FillPaint(vg, fadePaint);
            NVG.Fill(vg);

            fadePaint = NVG.LinearGradient(vg, x, y + h, x, y + h - 6, NVG.RGBA(200, 200, 200, 255), NVG.RGBA(200, 200, 200, 0));
            NVG.BeginPath(vg);
            NVG.Rect(vg, x + 4, y + h - 6, w - 8, 6);
            NVG.FillPaint(vg, fadePaint);
            NVG.Fill(vg);

            // Scroll bar
            shadowPaint = NVG.BoxGradient(vg, x + w - 12 + 1, y + 4 + 1, 8, h - 8, 3, 4, NVG.RGBA(0, 0, 0, 32), NVG.RGBA(0, 0, 0, 92));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + w - 12, y + 4, 8, h - 8, 3);
            NVG.FillPaint(vg, shadowPaint);
            //	NVG.FillColor(vg, NVG.RGBA(255,0,0,128));
            NVG.Fill(vg);

            scrollh = (h / stackh) * (h - 8);
            shadowPaint = NVG.BoxGradient(vg, x + w - 12 - 1, y + 4 + (h - 8 - scrollh) * u - 1, 8, scrollh, 3, 4, NVG.RGBA(220, 220, 220, 255), NVG.RGBA(128, 128, 128, 255));
            NVG.BeginPath(vg);
            NVG.RoundedRect(vg, x + w - 12 + 1, y + 4 + 1 + (h - 8 - scrollh) * u, 8 - 2, scrollh - 2, 2);
            NVG.FillPaint(vg, shadowPaint);
            //	NVG.FillColor(vg, NVG.RGBA(0,0,0,128));
            NVG.Fill(vg);

            NVG.Restore(vg);
        }

        static void DrawLines(NanoVGContext vg, float x, float y, float w, float h, float t)
        {
            int i, j;
            float pad = 5.0f, s = w / 9.0f - pad * 2;
            float fx, fy;
            float[] pts = new float[8];

            int[] joins = new int[3]{ (int)NVGlineCap.NVG_MITER, (int)NVGlineCap.NVG_ROUND, (int)NVGlineCap.NVG_BEVEL };
            int[] caps = new int[3]{ (int)NVGlineCap.NVG_BUTT, (int)NVGlineCap.NVG_ROUND, (int)NVGlineCap.NVG_SQUARE };
            //NVG_NOTUSED(h);

            NVG.Save(vg);
            pts[0] = -s * 0.25f + (int)Math.Cos(t * 0.3f) * s * 0.5f;
            pts[1] = (int)Math.Sin(t * 0.3f) * s * 0.5f;
            pts[2] = -s * 0.25f;
            pts[3] = 0;
            pts[4] = s * 0.25f;
            pts[5] = 0;
            pts[6] = s * 0.25f + (int)Math.Cos(-t * 0.3f) * s * 0.5f;
            pts[7] = (int)Math.Sin(-t * 0.3f) * s * 0.5f;

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    fx = x + s * 0.5f + (i * 3 + j) / 9.0f * w + pad;
                    fy = y - s * 0.5f + pad;

                    NVG.LineCap(vg, caps[i]);
                    NVG.LineJoin(vg, joins[j]);

                    NVG.StrokeWidth(vg, s * 0.3f);
                    NVG.StrokeColor(vg, NVG.RGBA(0, 0, 0, 160));
                    NVG.BeginPath(vg);
                    NVG.MoveTo(vg, fx + pts[0], fy + pts[1]);
                    NVG.LineTo(vg, fx + pts[2], fy + pts[3]);
                    NVG.LineTo(vg, fx + pts[4], fy + pts[5]);
                    NVG.LineTo(vg, fx + pts[6], fy + pts[7]);
                    NVG.Stroke(vg);

                    NVG.LineCap(vg, (int) NVGlineCap.NVG_BUTT);
                    NVG.LineJoin(vg, (int) NVGlineCap.NVG_BEVEL);

                    NVG.StrokeWidth(vg, 1.0f);
                    NVG.StrokeColor(vg, NVG.RGBA(0, 192, 255, 255));
                    NVG.BeginPath(vg);
                    NVG.MoveTo(vg, fx + pts[0], fy + pts[1]);
                    NVG.LineTo(vg, fx + pts[2], fy + pts[3]);
                    NVG.LineTo(vg, fx + pts[4], fy + pts[5]);
                    NVG.LineTo(vg, fx + pts[6], fy + pts[7]);
                    NVG.Stroke(vg);
                }
            }


            NVG.Restore(vg);
        }

        static unsafe void DrawParagraph(NanoVGContext vg, float x, float y, float width, float height, float mx, float my)
        {
            NVGtextRow[] rows = new NVGtextRow[3];
            NVGglyphPosition[] glyphs = new NVGglyphPosition[100];
            string text = "This is longer chunk of text.\n  \n  Would have used lorem ipsum but she    was busy jumping over the lazy dog with the fox and all the men who came to the aid of the party.🎉";
            string start;
            string end;
            int nrows, i, nglyphs, j, lnum = 0;
            float lineh;
            float caretx, px;
            float[] bounds = new float[4];
            float a;
            float gx = 0, gy = 0;
            int gutter = 0;

            //NVG_NOTUSED(height);

            NVG.Save(vg);

            NVG.FontSize(vg, 18.0f);
            NVG.FontFace(vg, "sans");
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_TOP);
            NVG.TextMetrics(vg, null, null, &lineh);

            // The text break API can be used to fill a large buffer of rows,
            // or to iterate over the text just few lines (or just one) at a time.
            // The "next" variable of the last returned item tells where to continue.
            start = text;
            end = text + text.Length;
            IntPtr rowPtr = NVG.GetIntPtrFromStructArray(rows);
            //GCHandle gch = GCHandle.Alloc(rows, GCHandleType.Normal);
            //IntPtr rowPtr = GCHandle.ToIntPtr(gch);
            nrows = NVG.TextBreakLines(vg, Encoding.UTF8.GetBytes(start), Encoding.UTF8.GetBytes(end), width, ref rowPtr, 3);

            var cRows = NVG.GetStructArrayFromIntPtr<NVGtextRow>(rowPtr, nrows); 

            while (nrows > 0)
            {
                for (i = 0; i < nrows; i++)
                {
                    NVGtextRow row =  rows[i];
                    bool hit = mx > x && mx < (x + width) && my >= y && my < (y + lineh);

                    NVG.BeginPath(vg);
                    NVG.FillColor(vg, NVG.RGBA(255, 255, 255, hit ? (byte)64 : (byte)16));
                    NVG.Rect(vg, x, y, row.width, lineh);
                    NVG.Fill(vg);

                    NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 255));
                    NVG.Text(vg, x, y, row.start, row.end);

                    if (hit)
                    {
                        caretx = (mx < x + row.width / 2) ? x : x + row.width;
                        px = x;
                        IntPtr positions = NVG.MarshalArrayToPointer(glyphs);
                        nglyphs = NVG.TextGlyphPositions(vg, x, y, NVG.StrToByte(row.start), NVG.StrToByte(row.end), positions, 100);
                        for (j = 0; j < nglyphs; j++)
                        {
                            float x0 = glyphs[j].x;
                            float x1 = (j + 1 < nglyphs) ? glyphs[j + 1].x : x + row.width;
                            float gx2 = x0 * 0.3f + x1 * 0.7f;
                            if (mx >= px && mx < gx2)
                                caretx = glyphs[j].x;
                            px = gx2;
                        }
                        NVG.BeginPath(vg);
                        NVG.FillColor(vg, NVG.RGBA(255, 192, 0, 255));
                        NVG.Rect(vg, caretx, y, 1, lineh);
                        NVG.Fill(vg);

                        gutter = lnum + 1;
                        gx = x - 10;
                        gy = y + lineh / 2;
                    }
                    lnum++;
                    y += lineh;
                }
                // Keep going...
                start = rows[nrows - 1].next;
                //nrows = NVG.TextBreakLines(vg, Encoding.UTF8.GetBytes(start), Encoding.UTF8.GetBytes(end), width, rowPtr, 3);
            }

            if (gutter>0)
            {
                string txt;
                txt = "16"+ gutter;
                NVG.FontSize(vg, 13.0f);
                NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_RIGHT | NVGAlign.NVG_ALIGN_MIDDLE);

                NVG.TextBounds(vg, gx, gy, Encoding.UTF8.GetBytes(txt), null, NVG.MarshalToPointer(bounds));

                NVG.BeginPath(vg);
                NVG.FillColor(vg, NVG.RGBA(255, 192, 0, 255));
                NVG.RoundedRect(vg, (int)bounds[0] - 4, (int)bounds[1] - 2, (int)(bounds[2] - bounds[0]) + 8, (int)(bounds[3] - bounds[1]) + 4, ((int)(bounds[3] - bounds[1]) + 4) / 2 - 1);
                NVG.Fill(vg);

                NVG.FillColor(vg, NVG.RGBA(32, 32, 32, 255));
                NVG.Text(vg, gx, gy, txt, null);
            }

            y += 20.0f;

            NVG.FontSize(vg, 13.0f);
            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_TOP);
            NVG.TextLineHeight(vg, 1.2f);

            IntPtr boundPtr = NVG.MarshalArrayToPointer(glyphs);
            NVG.TextBoxBounds(vg, x, y, 150, Encoding.UTF8.GetBytes("Hover your mouse over the text to see calculated caret position."), null,  boundPtr);

            // Fade the tooltip out when close to it.
            gx = Math.Abs((mx - (bounds[0] + bounds[2]) * 0.5f) / (bounds[0] - bounds[2]));
            gy = Math.Abs((my - (bounds[1] + bounds[3]) * 0.5f) / (bounds[1] - bounds[3]));
            a = Math.Max(gx, gy) - 0.5f;
            a = Math.Clamp(a, 0, 1);
            NVG.GlobalAlpha(vg, a);

            NVG.BeginPath(vg);
            NVG.FillColor(vg, NVG.RGBA(220, 220, 220, 255));
            NVG.RoundedRect(vg, bounds[0] - 2, bounds[1] - 2, (int)(bounds[2] - bounds[0]) + 4, (int)(bounds[3] - bounds[1]) + 4, 3);
            px = (int)((bounds[2] + bounds[0]) / 2);
            NVG.MoveTo(vg, px, bounds[1] - 10);
            NVG.LineTo(vg, px + 7, bounds[1] + 1);
            NVG.LineTo(vg, px - 7, bounds[1] + 1);
            NVG.Fill(vg);

            NVG.FillColor(vg, NVG.RGBA(0, 0, 0, 220));
            NVG.TextBox(vg, x, y, 150, Encoding.UTF8.GetBytes("Hover your mouse over the text to see calculated caret position."), null);

            NVG.Restore(vg);
        }

        public static void MarshalUnmananagedArray2Struct<T>(IntPtr unmanagedArray, int length, out T[] mangagedArray)
        {
            var size = Marshal.SizeOf(typeof(T));
            mangagedArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = Marshal.PtrToStructure<T>(ins);
            }
        }

    }
}
