using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace PhysicsSimulator.Engine.MathAddition
{
    /// <summary>
    /// Třída reprezentujcí obecně m x n matici, a realizující základní operace s nimi. (Gausova eliminace, invers, násobení, scitani, odecitani, nasobeni scalarem, transponování, elementární transformace, rank, regularita)
    /// </summary>
    [Serializable]
    class MatrixND
    {
        /// <summary>
        /// data matice
        /// </summary>
        private float[,] fields;

        /// <summary>
        /// zda je transponovana, pri transponovani se neprehazuje cele pole jen se zmeni indexer podle teto hodnoty
        /// </summary>
        private bool isTransposed;

        #region "Constructors"

        /// <summary>
        /// vytvori matici o danem poctu radku a sloupcu s volbou zda je jednotkova
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="isIdentity"></param>
        public MatrixND(int rows, int columns, bool isIdentity)
        {
            isTransposed = false;
            fields = new float[rows, columns];

            if (isIdentity)
                for (int i = 0; i < MathHelper.Min(rows, columns); i++)
                    fields[i, i] = 1;

        }

        /// <summary>
        /// vytvori matici z daneho pole
        /// </summary>
        /// <param name="fields"></param>
        public MatrixND(float[,] fields)
        {
            isTransposed = false;

            this.fields = fields;
        }


        /// <summary>
        /// vytvori matici z danych dvou rozmernych vektoru jako sloupcu
        /// </summary>
        /// <param name="columns"></param>
        public MatrixND(params Vector2[] columns)
        {
            isTransposed = false;

            this.fields = new float[2, columns.Length];
            for (int i = 0; i < this.Columns; i++)
            {
                this.fields[0, i] = columns[i].X;
                this.fields[1, i] = columns[i].Y;
            }
        }


        /// <summary>
        /// vytvori matici z danych trojrozmernych vektoru jako sloupcu
        /// </summary>
        /// <param name="columns"></param>
        public MatrixND(params Vector3[] columns)
        {
            isTransposed = false;

            this.fields = new float[3, columns.Length];
            for (int i = 0; i < this.Columns; i++)
            {
                this.fields[i, 0] = columns[i].X;
                this.fields[i, 1] = columns[i].Y;
                this.fields[i, 2] = columns[i].Z;
            }
        }

        #endregion

        #region "Functions"

        /// <summary>
        /// Vypise prvky matice ve formatu, ktery zchrousta volfram alfa
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < this.Rows; i++)
            {
                sb.Append("{");
                for (int j = 0; j < this.Columns; j++)
                {
                    sb.AppendFormat(" {0} ", this[i, j]);
                    if (j != this.Columns - 1) sb.Append(",");
                }
                sb.Append("}");
                if (i != this.Rows - 1) sb.Append(",");
            }
            sb.Append("}");

            return sb.ToString();
        }


        /// <summary>
        /// vypocte rank matice
        /// </summary>
        /// <param name="lastColumn"></param>
        /// <returns></returns>
        public int GetRank(int lastColumn)
        {
            int rank = 0;

            for (int row = 0; row < this.Rows; row++)
                for (int column = 0; column <= lastColumn; column++)
                    if (!Helper.AproximatellyEqual(this[row, column], 0))//if (fields[row, column] != 0)
                    {
                        rank++;
                        break;
                    }

            return rank;
        }

        /// <summary>
        /// Invertuje matici
        /// </summary>
        /// <returns></returns>
        public bool Invert()
        {
            if (Columns != Rows)
                return false;

            MatrixND result = new MatrixND(Rows, Columns, true);

            int height = 0;

            for (int column = 0; column < this.Columns; column++)
            {
                if (Helper.AproximatellyEqual(this[height, column], 0))//if (fields[height, column] == 0)
                    if (!FindMaxPivot(column, height, result)) return false;//pokud neni v sloupci neuulová hodnota nema invers  //hledam jendom kdyz je nula to je divny ale tak co ....

                for (int row = 0; row < this.Rows; row++)
                    if (row != height)
                        if (!Helper.AproximatellyEqual(this[row, column], 0))//if (fields[row, column] != 0)        
                        {
                            float scalar = -this[row, column] / this[height, column];

                            this.MultiplyAndAddRowToAnother(height, row, scalar);
                            result.MultiplyAndAddRowToAnother(height, row, scalar);
                        }

                float scalar1 = 1 / this[height, column];

                this.MultiplyRow(height, scalar1);
                result.MultiplyRow(height, scalar1);

                height++;
                if (height >= this.Rows) break;
            }

            this.fields = result.fields;
            this.isTransposed = false;

            return true;
        }


        /// <summary>
        /// transponuje matici
        /// </summary>
        public void Transpose()
        {
            isTransposed ^= true;
        }



        /// <summary>
        /// Elimine and return true when matrix has one solution.
        /// </summary>
        /// <returns>Return true, wheather matrix has one solution.</returns>
        public bool Elimine()
        {
            bool result = true;

            int height = 0;

            for (int column = 0; column < this.Columns; column++)
            {
                if (Helper.AproximatellyEqual(this[height, column], 0))//if (fields[height, column] == 0)
                    if (!FindMaxPivot(column, height))
                    {
                        if (column < this.Columns)
                            result = false;
                        continue;//pokud neni v sloupci neuulová hodnota přeskoč ho      
                    }


                for (int row = 0; row < this.Rows; row++)
                    if (row != height)
                        if (!Helper.AproximatellyEqual(this[row, column], 0))//if (fields[row, column] != 0)                            
                            MultiplyAndAddRowToAnother(height, row, -this[row, column] / this[height, column]);

                MultiplyRow(height, 1 / this[height, column]);

                height++;
                if (height >= this.Rows) break;

            }

            return result;

        }

        #region "Elementary trnsformations"

        /// <summary>
        /// Multiple specific row by scalar.
        /// </summary>
        /// <param name="row">Specific row.</param>
        /// <param name="scalar">Specific scalar.</param>
        private void MultiplyRow(int row, float scalar)
        {
            for (int i = 0; i < this.Columns; i++)
            {
                this[row, i] *= scalar;
            }
        }

        private void MultiplyAndAddRowToAnother(int row1, int row2, float scalar)
        {
            for (int i = 0; i < this.Columns; i++)
                this[row2, i] += scalar * this[row1, i];
        }

        private void SwitchRows(int row1, int row2)
        {
            if (row1 == row2)
                return;

            for (int i = 0; i < this.Columns; i++)
            {
                float temp = this[row2, i];
                this[row2, i] = this[row1, i];
                this[row1, i] = temp;
            }
        }

        #endregion

        /// <summary>
        /// Methods find max value pivot and switch rows in order to make RREF.
        /// </summary>
        /// <param name="column">Search in this specific column.</param>
        /// <param name="height">Searcho from this row to end.</param>
        /// <returns>Returns false wheather nonzero does not exist.</returns>
        private bool FindMaxPivot(int column, int height)
        {
            return FindMaxPivot(column, height, new MatrixND(Rows, Columns, false));
        }


        /// <summary>
        /// Najde nejvetsi poivota, aby se alespon castecne eliminovaly chyby foating point aritmentky
        /// </summary>
        /// <param name="column"></param>
        /// <param name="height"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private bool FindMaxPivot(int column, int height, MatrixND matrix)
        {
            int max = height;

            for (int row = height; row < this.Rows; row++)
                if (!Helper.AproximatellyEqual(this[row, column], 0))//if (fields[row, column] != 0)                
                    if (Math.Abs(this[max, column]) < Math.Abs(this[row, column]))
                        max = Math.Abs(row);

            if (Helper.AproximatellyEqual(this[max, column], 0))
                return false;
            else
            {
                this.SwitchRows(height, max);
                matrix.SwitchRows(height, max);
                return true;
            }
        }


        #endregion

        #region "Properties"

        /// <summary>
        /// urcuje zda je matice regularni
        /// </summary>
        public bool Regular
        {
            get
            {
                if (Columns == Rows && Rank == Columns)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// urcuje pocet radku matice
        /// </summary>
        public int Rows
        {
            get
            {
                if (isTransposed)
                    return fields.GetLength(1);
                else
                    return fields.GetLength(0);
            }
        }


        /// <summary>
        /// urcuje pocet sloupcu matice
        /// </summary>
        public int Columns
        {
            get
            {
                if (isTransposed)
                    return fields.GetLength(0);
                else
                    return fields.GetLength(1);
            }
        }


        /// <summary>
        /// urcuje rank matice
        /// </summary>
        public int Rank
        {
            get
            {
                MatrixND elim = MatrixND.Elimine(this);

                return elim.GetRank(Columns - 1);
            }
        }

        /// <summary>
        /// definuje idexer matice tak aby sedel dle trsponace
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public float this[int i, int j]
        {
            get
            {
                if (isTransposed)
                    return this.fields[j, i];
                else
                    return this.fields[i, j];
            }
            set
            {
                if (isTransposed)
                    this.fields[j, i] = value;
                else
                    this.fields[i, j] = value;
            }
        }

        #endregion

        #region "Static fields"


        /// <summary>
        /// Z invertuje danou matici
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND Inverse(MatrixND m)
        {
            MatrixND result = MatrixND.Clone(m);

            if (!result.Invert())
                throw new Exception();

            return result;
        }

        /// <summary>
        /// Naklonuje danou matici
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND Clone(MatrixND m)
        {
            MatrixND result = new MatrixND((float[,])m.fields.Clone());
            result.isTransposed = m.isTransposed;

            return result;
        }


        /// <summary>
        /// Eliminuje danou matici Gausovou eliminaci
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND Elimine(MatrixND m)
        {
            MatrixND result = MatrixND.Clone(m);
            result.Elimine();

            return result;
        }


        #region "Operators"


        /// <summary>
        /// Trasponuje matici
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND Transpose(MatrixND m)
        {
            m.Transpose();
            return m;
        }


        /// <summary>
        /// Vynasobi dve matice
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND operator *(float scalar, MatrixND m)
        {
            MatrixND result = MatrixND.Clone(m);

            for (int r = 0; r < result.Rows; r++)
                result.MultiplyRow(r, scalar);

            return result;
        }


        /// <summary>
        /// vynasobi matici scalarem -1
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MatrixND operator -(MatrixND m)
        {
            return -1 * m;
        }


        /// <summary>
        /// secte dve matice
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static MatrixND operator +(MatrixND m1, MatrixND m2)
        {
            if (m1.Columns != m1.Columns || m1.Rows != m1.Rows) throw new Exception();

            MatrixND result = new MatrixND(m1.Rows, m2.Columns, false);

            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result.fields[r, c] = m1[r, c] + m2[r, c];

            return result;
        }


        /// <summary>
        /// odecte dve matice
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static MatrixND operator -(MatrixND m1, MatrixND m2)
        {
            return m1 + -1 * m2;
        }


        /// <summary>
        /// vynasobi dve matice
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static MatrixND operator *(MatrixND m1, MatrixND m2)
        {
            if (m1.Columns != m2.Rows)
                throw new Exception();

            MatrixND result = new MatrixND(m1.Rows, m2.Columns, false);

            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    for (int m = 0; m < m1.Columns; m++)
                        result.fields[r, c] += m1[r, m] * m2[m, c];

            return result;
        }

        #endregion

        #endregion


    }
}
