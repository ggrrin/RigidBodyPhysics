using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.MathAddition
{
    /// <summary>
    /// Statická třída poskytujcí některé funkce, které se hodili nebo často používaly.
    /// </summary>
    [Serializable]
    static class Helper
    {
        /// <summary>
        /// Determine if value is in specific interval. Interval don't have to be sorted.
        /// </summary>
        /// <param name="interval">Interval to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>Returns true wheather value lies int interval.</returns>
        public static bool IsInInterval(Vector2 interval, float value)
        {
            interval = new Vector2(MathHelper.Min(interval.X, interval.Y), MathHelper.Max(interval.X, interval.Y));

            if ((value >= interval.X || AproximatellyEqual(value, interval.X)) && (value <= interval.Y || AproximatellyEqual(value, interval.Y)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Vypocte zda je hodnota v obdelniku urcenymi dvema vektory
        /// </summary>
        /// <param name="intervalPoint1">prvni vektor</param>
        /// <param name="intervalPoint2">druhy vektor</param>
        /// <param name="valueTest">testovaci vektor</param>
        /// <returns></returns>
        public static bool IsInSquareInterval(Vector2 intervalPoint1, Vector2 intervalPoint2, Vector2 valueTest)
        {
            return IsInInterval(new Vector2(intervalPoint1.X, intervalPoint2.X), valueTest.X) && IsInInterval(new Vector2(intervalPoint1.Y, intervalPoint2.Y), valueTest.Y);
        }


        /// <summary>
        /// vypocte mocninu daneho cisla
        /// </summary>
        /// <param name="value">hodnota</param>
        /// <param name="power">mocnina</param>
        /// <returns>vysledek</returns>
        public static float Pow(float value, int power)
        {
            bool isInvers = false;
            if (power < 0) isInvers = true;
            float result = 1;
            for (int i = 1; i <= Math.Abs(power); i++)
            {
                result *= value;
            }

            if (isInvers)
                return 1 / result;
            else
                return result;
        }

        /// <summary>
        /// Vypocte druhou mocninu hodnoty
        /// </summary>
        /// <param name="value"></param>
        /// <returns>vysledek</returns>
        public static float Pow(float value)
        {
            return Pow(value, 2);
        }

        /// <summary>
        /// Vrati zda jsou hodnoty priblizne na 4 desetina mista si rovny
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool AproximatellyEqual(float x, float y)
        {
            if (Math.Round((double)x, 4) == Math.Round((double)y, 4))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Vypocte zda jsou vektory priblizne stejen velke
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool AproximatellyEqual(Vector2 u, Vector2 v)
        {
            if (AproximatellyEqual(u.X, v.X) && AproximatellyEqual(u.Y, v.Y))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Udela z vestaveneho vektoru matici urcenou vektorem
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static MatrixND ToMatrix(Vector2 vector)
        {
            return new MatrixND(new float[,] { { vector.X }, { vector.Y } });
        }


        /// <summary>
        /// Udela z vestaveneho vektoru matici urcenou vektorem
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static MatrixND ToMatrix(Vector3 vector)
        {
            return new MatrixND(new float[,] { { vector.X }, { vector.Y }, { vector.Z } });
        }

        /// <summary>
        /// udela z matice urcujici vektor vestaveni vektor
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(MatrixND matrix)
        {
            if (matrix.Columns != 1 || matrix.Rows != 2) throw new Exception();

            return new Vector2(matrix[0, 0], matrix[1, 0]);
        }



        /// <summary>
        /// udela z matice urcujici vektor vestaveni vektor
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(MatrixND matrix)
        {
            if (matrix.Columns != 1 || matrix.Rows != 3) throw new Exception();

            return new Vector3(matrix[0, 0], matrix[1, 0], matrix[2, 0]);
        }


        /// <summary>
        /// aplikuje danou transformacni matici na vektor
        /// </summary>
        /// <param name="matrix">transformacni matice</param>
        /// <param name="v">vektor</param>
        /// <returns>transformovany fektor</returns>
        public static Vector2 Transform(MatrixND matrix, Vector2 v)
        {
            return ToVector2(matrix * ToMatrix(v));
        }

        /// <summary>
        /// aplikuje danou transformacni matici na vektor
        /// </summary>
        /// <param name="matrix">transformacni matice</param>
        /// <param name="v">vektor</param>
        /// <returns>transformovany fektor</returns>
        public static Vector3 Transform(MatrixND matrix, Vector3 v)
        {
            return ToVector3(matrix * ToMatrix(v));
        }

        /// <summary>
        /// Vytvori matici rotace
        /// </summary>
        /// <param name="angle">dany uhel</param>
        /// <returns>vysledna matice</returns>
        public static MatrixND CreateRotationMatrix(float angle)
        {
            MatrixND rotationMatrix = new MatrixND(2, 2, true);
            rotationMatrix[0, 0] = (float)Math.Cos((double)angle);
            rotationMatrix[1, 0] = (float)Math.Sin((double)angle);
            rotationMatrix[0, 1] = -(float)Math.Sin((double)angle);
            rotationMatrix[1, 1] = (float)Math.Cos((double)angle);

            return rotationMatrix;
        }
    }
}
