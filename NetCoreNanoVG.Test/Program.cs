using Glfw3;
using NanoVG;
using System;
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

            // Widgets

            DrawColorWheel(vg, 400, 150, 200, 200, 1);

            DrawWindow(vg, "Left Widgets Stuff", 50, 50, 300, 600);

            DrawSearchBox(vg, "Search ", x, y, 200, 25);

            drawLabel(vg, "OMG Label", x, 120, 200, 50);
            drawDropDown(vg, "My Dropdown", x, 170, 200, 25);
            DrawEditBox(vg, "Edit Text", x, 200, 200, 30);
            DrawEditBoxNum(vg, "0123455", new byte[] { 1 }, x, 240, 200, 30);
            DrawCheckBox(vg, "Check 1", x, 280, 200, 20);
            DrawButton(vg, 1, "Click me!", x, 300, 200, 30, NVG.RGB(10,10,10));

            DrawWindow(vg, "Right Widgets", 650, 50, 300, 600);
        }

        const int ICON_SEARCH = 0x1F50D;
        const int ICON_CIRCLED_CROSS = 0x2716;
        const int ICON_CHEVRON_RIGHT = 0xE75E;
        const int ICON_CHECK = 0x2713;
        const int ICON_LOGIN = 0xE740;
        const int ICON_TRASH = 0xE729;

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

        static void drawLabel(NanoVGContext vg, string text, float x, float y, float w, float h)
        {

            //NVG_NOTUSED(w);

            NVG.FontSize(vg, 18.0f);
            NVG.FontFace(vg, "sans");
            NVG.FillColor(vg, NVG.RGBA(255, 255, 255, 128));

            NVG.TextAlign(vg, NVGAlign.NVG_ALIGN_LEFT | NVGAlign.NVG_ALIGN_MIDDLE);
            NVG.Text(vg, x, y + h * 0.5f, text, null);
        }

        static void drawDropDown(NanoVGContext vg, string text, float x, float y, float w, float h)
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
    }
}
