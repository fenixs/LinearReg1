using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using System.IO;

namespace LinearReg1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("请输入csv文件目录(输入q退出):");
            var inputstr = Console.ReadLine();
            while (!File.Exists(inputstr))
            {
                if (inputstr == "q" || inputstr == "quit")
                {
                    return;
                }
                Console.WriteLine("请输入csv文件目录(输入q退出):");
                inputstr = Console.ReadLine();
            }
            Console.WriteLine("csv文件读取成功");
            
            
            var lines = File.ReadAllLines(inputstr);
            
            
            if (lines==null || lines.Length == 0)
            {
                Console.WriteLine("文件错误");
                Console.Read();
                return;
            }
            

            //var datas = new double[row, col];        //声明源数据矩阵 x1,x2,...,xn
            //var ydata = new double[row];           //声明结果矩阵,y
            //var headers = new string[];

            double[,] xdata;
            double[] ydata;
            string[] headers;


            ReadData(lines, out headers, out xdata, out ydata);     //读取数据

            LinearRegRange(3, 4, xdata, ydata);

            var p = LinearReg(xdata, ydata);            //线性回归


            PrintResult(p, headers);
       

            Console.ReadKey();
        }

        #region "方法"

        /// <summary>
        /// 多次计算回归结果
        /// 遍历参数表的各个参数
        /// </summary>
        /// <param name="minParamCount">最小参数个数</param>
        /// <param name="maxParamCount">最大参数个数</param>
        /// <param name="xdata">全部参数的x集合</param>
        /// <param name="ydata">结果集</param>
        private static void LinearRegRange(int minParamCount, int maxParamCount,double[,] xdata,double[] ydata)
        {
            //获取矩阵的row 和 col
            var row = xdata.GetLength(0);
            var col = xdata.GetLength(1);
            

            //获取列下标，如果有20列，则下标数组为0-19
            var colnumbers = new int[col - 1];
            for (int i = 1; i < col; i++)
			{
			    colnumbers[i - 1] = i;
			}

            for (int i = minParamCount; i <= maxParamCount; i++)
            {
                var combs = PermutationAndCombination<int>.GetCombination(colnumbers,i);
                double[,] xdata1 = new double[row, i];
                

            }
        }
        

        /// <summary>
        /// 从cvs文件中读取数据
        /// </summary>
        /// <param name="fileLines"></param>
        /// <param name="headers"></param>
        /// <param name="xdata"></param>
        /// <param name="ydata"></param>
        private static void ReadData(string[] fileLines, out string[] headers, out double[,] xdata, out double[] ydata)
        {
            var row = fileLines.Length;     //当前数据的行数，矩阵的行数
            headers = fileLines[0].Split(',');     //获取到抬头的行数 

            row = row - 1;          //默认第一行为header，不计算
            var col = fileLines[0].Count(c => c == ',') + 1;        //当前数据的列数
            //当前矩阵的列数，第一列要添加数字1,因为多一列结果列，不需要添加1
            xdata = new double[row, col];
            ydata = new double[row];
            //读取数据源赋值到矩阵
            for (int r = 0; r < row; r++)
            {
                var curLine = fileLines[r + 1].Split(',');      //第一行默认为header
                xdata[r, 0] = 1d;       //第一列赋值为1
                for (int c = 1; c < col; c++)
                {
                    xdata[r, c] = Convert.ToDouble(curLine[c - 1]);
                }
                ydata[r] = Convert.ToDouble(curLine[col - 1]);     //获取到实际结果,y
            }
        }

        /// <summary>
        /// 打印数据结果
        /// </summary>
        /// <param name="p"></param>
        /// <param name="headers"></param>
        private static void PrintResult(Vector<double> p,string[] headers)
        {
            for (int i = 0; i < p.Count; i++)
            {
                if (i > 0)
                {
                    if (p[i] > 0)
                        Console.Write(" + ");
                    else
                    {
                        Console.Write(" - ");
                    }
                    Console.Write("{0:0.##} * {1}", Math.Abs(p[i]), headers[i - 1]);        //header从第二个数值开始
                }
                else
                {
                    Console.Write(p[i].ToString("0.##"));       //第一个数值是常量
                }

            }
            Console.WriteLine();
        }

        /// <summary>
        /// 回归计算
        /// </summary>
        /// <param name="datas">因变量矩阵,x1,x2..,xn</param>
        /// <param name="ydata">自变量结果数组,y1,y2,...,yn</param>
        static Vector<double> LinearReg(double[,] datas, double[] ydata)
        {
            var X = DenseMatrix.OfArray(datas);
            var y = new DenseVector(ydata);
            var p = X.QR().Solve(y);
            return p;
            
        }

        #endregion

        static void CalHotel()
        {
            var M3 = new double[] {  1, 283, 80089,184,280 };
            var M4 = new double[] {  1, 291, 84681,209,287 };
            var M5 = new double[] {  1, 294, 86436,198,290 };
            var M6 = new double[] {  1, 298, 88804,200,295 };
            var M7 = new double[] {  1, 303, 91809,198,299 };
            var M8 = new double[] {  1, 302, 91204,184,299 };
            var M9 = new double[] {  1, 296, 87616,189,292 };
            var M10 = new double[] { 1, 294, 86436,234,290 };
            var M11 = new double[] { 1, 285, 81225,176,283 };
            var M12 = new double[] { 1, 279, 77841,210,276 };

            var X = new DenseMatrix(10, 5);
            X.SetRow(0, M3);
            X.SetRow(1, M4);
            X.SetRow(2, M5);
            X.SetRow(3, M6);
            X.SetRow(4, M7);
            X.SetRow(5, M8);
            X.SetRow(6, M9);
            X.SetRow(7, M10);
            X.SetRow(8, M11);
            X.SetRow(9, M12);

            var ydata = new double[] { 539442, 340016, 48100, 574992, 748957, 687898, 648278, 519887, 384086, 539514 };
            var y = new DenseVector(ydata);
            var p = X.QR().Solve(y);
            //Console.WriteLine("a={0},b={1},c={2},d={3},e={4},f={5}", p[0], p[1], p[2], p[3], p[4],p[5]);
            Console.WriteLine("a={0},b={1},c={2},d={3},e={4}", p[0], p[1], p[2], p[3], p[4]);
            Console.WriteLine("1");
        }

        static void ExamFunction()
        {
            
            var datas = new double[,]{
                {  1, 283, 80089,184,280 },
                {  1, 283, 80089,184,280 },
                {  1, 294, 86436,198,290 },
                {  1, 298, 88804,200,295 },
                {  1, 303, 91809,198,299 },
                {  1, 302, 91204,184,299 },
                {  1, 296, 87616,189,292 },
                { 1, 294, 86436,234,290 },
                { 1, 285, 81225,176,283 },
                { 1, 279, 77841,210,276 }
            };

           
            
            var funcs = new double[] {1480000,-10400,1816,764.5,4985 };

            var row = datas.GetLength(0);
            var col = datas.GetLength(1);
            for (int i = 0; i < row; i++)
            {
                var me = 0d;
                for (int c = 0; c < col; c++)
                {
                    me += datas[i, c] * funcs[c];                    
                }
                Console.WriteLine(me);
            }

        }

        static void CalHotel2()
        {
            var M3 = new double[] { 1, 23, 8, 283, 280, 81.2 };
            var M4 = new double[] { 1, 19, 11, 291, 287, 84.9 };
            var M5 = new double[] { 1, 22, 9, 294, 290, 75.1 };
            var M6 = new double[] { 1, 20, 10, 298, 295, 75.7 };
            var M7 = new double[] { 1, 22, 9, 303, 299, 66.1 };
            var M8 = new double[] { 1, 23, 8, 302, 299, 65.6 };
            var M9 = new double[] { 1, 21, 9, 296, 292, 77.6 };
            var M10 = new double[] { 1, 18, 13, 294, 290, 80.2 };
            var M11 = new double[] { 1, 22, 8, 285, 283, 82.2 };
            var M12 = new double[] { 1, 21, 10, 279, 276, 62.3 };

            var X = new DenseMatrix(10, 6);
            X.SetRow(0, M3);
            X.SetRow(1, M4);
            X.SetRow(2, M5);
            X.SetRow(3, M6);
            X.SetRow(4, M7);
            X.SetRow(5, M8);
            X.SetRow(6, M9);
            X.SetRow(7, M10);
            X.SetRow(8, M11);
            X.SetRow(9, M12);

            var ydata = new double[] { 539442, 340016, 48100, 574992, 748957, 687898, 648278, 519887, 384086, 539514 };
            var y = new DenseVector(ydata);
            var p = X.QR().Solve(y);
            Console.WriteLine("a={0},b={1},c={2},d={3},e={4},f={5}", p[0], p[1], p[2], p[3], p[4], p[5]);
            Console.WriteLine("1");
        }
    }
}
