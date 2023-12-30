namespace ConsoleCube;

/// <summary>
/// Inspiration from the following example:
/// https://www.youtube.com/watch?v=p09i_hoFdd0
/// I've re-written this from the original GCC Linux into MS C#.
/// </summary>
internal class Program
{
    #region [Props]
    static float x, y, z, ooz, xp, yp;
    static int idx;
    static float incrementSpeed = 0.6f;
    static float cubeWidth = 22;
    static int width = 70;
    static int height = 30;
    static float[] zBuffer = new float[width * height];
    static int[] buffer = new int[width * height];
    static int backgroundASCIICode = ' ';
    static float A = 0.001f;
    static float B = 0.001f;
    static float C = 0.001f;
    static int distanceFromCam = 100;
    static float K1 = 30;
    static IntPtr conHwnd = IntPtr.Zero;
    #endregion

    static void Main(string[] args)
    {
        #region [Center Console Window]
        conHwnd = ConsoleHelper.GetForegroundWindow();
        var winSize = ConsoleHelper.GetWindowSize(conHwnd);
        var buffSize = ConsoleHelper.GetConsoleSize();
        var dims = ConsoleHelper.GetScreenDimensions();
        if (dims.width > 0 && dims.height > 0)
        {
            var x = (dims.width - (winSize.width + 10)) / 2;
            var y = (dims.height - (winSize.height + 10)) / 3;
            ConsoleHelper.SetWindowPosition(conHwnd, x, y, winSize.width, winSize.height + (int)((float)winSize.height * 0.2f));
        }
        else
            ConsoleHelper.SetWindowPosition(conHwnd, 1, 1, winSize.width, winSize.height);
        
        if (args.Length > 0)
            ConsoleHelper.ShowWindow(conHwnd, ConsoleHelper.SW_MAXIMIZE);
        #endregion

        // Blocking call.
        DrawCube();
    }

    /// <summary>
    /// TODO: Add back-face culling.
    /// </summary>
    static void DrawCube()
    {
        //printf("\x1b[2J");
        // In C, printf("\x1b[2J") is an escape sequence using ASCII control characters.
        // Specifically, \x1b represents the escape character (\e in octal notation),
        // [ indicates the start of an escape sequence, 2 is a parameter indicating a 
        // specific action, and J is the command to clear the entire terminal screen.
        Console.Clear();
        Console.CursorVisible = false;
        while (true)
        {
            // In C, "void* memset(void* ptr, int x, size_t n)" will be replaced by "Array.Fill(int[] array, int value)".

            // Blank the background.
            Array.Fill(buffer, backgroundASCIICode);
            // Reset depth buffer.
            Array.Fill(zBuffer, 0);
            // Determine the char for each face of the cube.
            for (float cubeX = -cubeWidth; cubeX < cubeWidth; cubeX += incrementSpeed)
            {
                for (float cubeY = -cubeWidth; cubeY < cubeWidth; cubeY += incrementSpeed)
                {
                    // 1st face
                    calculateForSurface(cubeX, cubeY, -cubeWidth, '.');
                    // 2nd face
                    calculateForSurface(cubeWidth, cubeY, cubeX, '~');
                    // 3rd face
                    calculateForSurface(-cubeWidth, cubeY, -cubeX, '"');
                    // 4th face
                    calculateForSurface(-cubeX, cubeY, cubeWidth, ':');
                    // 5th face
                    calculateForSurface(cubeX, -cubeWidth, -cubeY, '+');
                    // 6th face
                    calculateForSurface(cubeX, cubeWidth, cubeY, '-');
                }
            }
            //printf("\x1b[H");
            // In C, printf("\x1b[H") is an escape sequence using ASCII control characters.
            // Specifically, \x1b represents the escape character (\e in octal notation) and
            // [H is the command to position the cursor to the top-left corner of the terminal
            // screen (Row 1, Column 1).
            Console.SetCursorPosition(0, 0);
            for (int k = 0; k < width * height; k++)
            {
                if (k % width == 0) // Add CRLF on end of row.
                    Console.WriteLine();
                else
                    Console.Write((char)buffer[k]);
            }

            // Rotate matrix by some amount.
            A += 0.05f;
            B += 0.05f;
            C += 0.001f;

            //if (A >= 360) { A = 0; }
            //if (B >= 360) { B = 0; }
            //if (C >= 360) { C = 0; }

            Thread.Sleep(1);

            // The cursor may become visible again if the user has resized the console window.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Console.CursorVisible)
                Console.CursorVisible = false;
        }
    }

    #region [Math Functions]
    static float calculateX(int i, int j, int k)
    {
        return j * MathF.Sin(A) * MathF.Sin(B) * MathF.Cos(C) - k * MathF.Cos(A) * MathF.Sin(B) * MathF.Cos(C) +
               j * MathF.Cos(A) * MathF.Sin(C) + k * MathF.Sin(A) * MathF.Sin(C) + i * MathF.Cos(B) * MathF.Cos(C);
    }
    static float calculateY(int i, int j, int k)
    {
        return j * MathF.Cos(A) * MathF.Cos(C) + k * MathF.Sin(A) * MathF.Cos(C) -
               j * MathF.Sin(A) * MathF.Sin(B) * MathF.Sin(C) + k * MathF.Cos(A) * MathF.Sin(B) * MathF.Sin(C) -
               i * MathF.Cos(B) * MathF.Sin(C);
    }
    static float calculateZ(int i, int j, int k)
    {
        return k * MathF.Cos(A) * MathF.Cos(B) - j * MathF.Sin(A) * MathF.Cos(B) + i * MathF.Sin(B);
    }
    static void calculateForSurface(float cubeX, float cubeY, float cubeZ, int ch)
    {
        x = calculateX((int)cubeX, (int)cubeY, (int)cubeZ);
        y = calculateY((int)cubeX, (int)cubeY, (int)cubeZ);
        z = calculateZ((int)cubeX, (int)cubeY, (int)cubeZ) + distanceFromCam;
        ooz = 1 / z;
        xp = (int)(width / 2 + K1 * ooz * x * 2); //xp = (int)(width / 2 - 2 * cubeWidth + K1 * ooz * x * 2);
        yp = (int)(height / 2 + K1 * ooz * y);
        idx = (int)(xp + yp * width);
        if (idx >= 0 && idx < width * height)
        {
            if (ooz > zBuffer[idx])
            {
                zBuffer[idx] = ooz;
                buffer[idx] = ch;
            }
        }
    }
    #endregion
}
