using System;

class ElGamalEC
{
    static void Main()
    {
        int p = 23;
        int a = 1;
        int b = 1;
        Point basePoint = new Point(17, 20);

        Console.WriteLine($"Елiптична крива: y^2 = (x^3 + x^2 + 1) mod {p}");
        Console.WriteLine($"Базова точка G = ({basePoint.X}, {basePoint.Y})");

        // Генерація приватного ключа
        int privateKey = GeneratePrivateKey(p);

        Console.WriteLine($"Приватний ключ: {privateKey}");

        // Генерація відкритого ключа
        Point publicKey = GeneratePublicKey(basePoint, privateKey, p);
        Console.WriteLine($"Вiдкритий ключ (Q): ({publicKey.X}, {publicKey.Y})");

        // Шифрування повідомлення
        int message = 10;
        Console.WriteLine($"Повiдомлення для шифрування: {message}");

        // Генерація випадкового числа k
        int k = GenerateRandomK(p);

        // Генерація точки (C1, C2)
        Point[] cipherText = Encrypt(message, basePoint, publicKey, k, p);
        Console.WriteLine($"Шифрований текст: C1 = ({cipherText[0].X}, {cipherText[0].Y}), C2 = ({cipherText[1].X}, {cipherText[1].Y})");

        // Дешифрування повідомлення
        int decryptedMessage = Decrypt(cipherText, basePoint, privateKey, p);
        Console.WriteLine($"Розшифроване повiдомлення: {decryptedMessage}");
    }

    static int GeneratePrivateKey(int p)
    {
        // Генерація випадкового приватного ключа
        Random random = new Random();
        return random.Next(1, p - 1);
    }

    static Point GeneratePublicKey(Point basePoint, int privateKey, int p)
    {
        // Генерація відкритого ключа (Q = privateKey * G)
        return Multiply(basePoint, privateKey, p);
    }

    static int GenerateRandomK(int p)
    {
        // Генерація випадкового числа k
        Random random = new Random();
        return random.Next(1, p - 1);
    }

    static Point[] Encrypt(int message, Point basePoint, Point publicKey, int k, int p)
    {
        // Шифрування повідомлення
        Point[] cipherText = new Point[2];

        // C1 = k * G
        cipherText[0] = Multiply(basePoint, k, p);

        // C2 = k * publicKey + message * G
        Point temp = Multiply(publicKey, k, p);
        cipherText[1] = AddPoints(temp, Multiply(basePoint, message, p), p);

        return cipherText;
    }

    static int Decrypt(Point[] cipherText, Point basePoint, int privateKey, int p)
    {
        // Дешифрування повідомлення
        // M = C2 - privateKey * C1
        Point temp = Multiply(cipherText[0], privateKey, p);
        Point decryptedMessage = SubtractPoints(cipherText[1], temp, p);

        return decryptedMessage.X;
    }

    // Множення точки на скаляр
    static Point Multiply(Point point, int scalar, int p)
    {
        Point result = new Point(0, 0);
        Point temp = new Point(point.X, point.Y);

        for (int i = 0; i < scalar; i++)
        {
            result = AddPoints(result, temp, p);
        }

        return result;
    }

    // Додавання точок
    static Point AddPoints(Point point1, Point point2, int p)
    {
        if (point1.IsNeutralElement())
        {
            return point2;
        }
        if (point2.IsNeutralElement())
        {
            return point1;
        }

        int slope;
        if (point1.Equals(point2))
        {
            slope = ((3 * point1.X * point1.X + 2 * 1) % p) * ModInverse(2 * point1.Y, p) % p;
        }
        else
        {
            slope = ((point2.Y - point1.Y) % p + p) * ModInverse((point2.X - point1.X + p) % p, p) % p;
        }

        int x3 = (slope * slope - point1.X - point2.X + p + p) % p;
        int y3 = (slope * (point1.X - x3) - point1.Y + p) % p;

        return new Point(x3, y3);
    }

    // Віднімання точок
    static Point SubtractPoints(Point point1, Point point2, int p)
    {
        Point negativePoint2 = new Point(point2.X, (p - point2.Y) % p);
        return AddPoints(point1, negativePoint2, p);
    }

    // Обернений елемент за модулем
    static int ModInverse(int a, int m)
    {
        for (int i = 1; i < m; i++)
        {
            if ((a * i) % m == 1)
            {
                return i;
            }
        }

        return -1;
    }
}

class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsNeutralElement()
    {
        return X == 0 && Y == 0;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Point other = (Point)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}