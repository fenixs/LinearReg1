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
            Console.WriteLine("csv文件读取成功...");
            
            
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
            Console.WriteLine("读取数据成功...");
            int minParamCount = 6;
            int maxParamCount = 6;
            var resLines = LinearRegRange(minParamCount, maxParamCount, xdata, ydata, inputstr, headers);
            Console.WriteLine("回归分析{0}个参数到{1}个参数的各种组合...",minParamCount,maxParamCount);
            WriteResFile(inputstr, resLines,string.Format("{0}-{1}",minParamCount,maxParamCount));
            Console.WriteLine("写入结果文件");
            //ReadDataOld(lines, out headers, out xdata, out ydata);     //读取数据
            //var p = LinearReg(xdata, ydata);            //线性回归
            //PrintResult(p, headers);
       
            

            Console.ReadKey();
        }

        #region "方法"


        /// <summary>
        /// 计算每个公式的R2
        /// </summary>
        /// <param name="xdata">当前使用的x数据源，第一列为1</param>
        /// <param name="ydata"></param>
        /// <param name="p">回归公式，p[0]为常量</param>
        /// <returns></returns>
        private static void CalR2CVFsig(double[,] xdata,double[] ydata,Vector<double> p,out double R2,out double CV,out double F,out double sig)
        {
            /*
             * R^2 = 1 - SEline/SEy
             * SEline = (y1-f(x1))^2 + (y2-f(x2))^2 + ... + (yn-f(xn))^2        残差平方和
             * SEy = (y1-yaver)^2 + (y2-yaver)^2 + ... + (yn-yaver)^2
             * */
            int row = xdata.GetLength(0);
            int col = xdata.GetLength(1);
            R2 = 0d;
            CV = 1d;
            F = 0d;
            sig = 1d;

            if(ydata.Length!=row) return;
            if(p.Count != col) return;

            double yAverage = ydata.Average();      //获取yAverage
            double[] y1 = new double[row];     //建立回归公式计算的y值数组
            //对矩阵每一行计算y1
            for (int r = 0; r < row; r++)
            {
                y1[r] = p[0];       //先计算常量
                for (int c = 1; c < col; c++)
                {
                    y1[r] += p[c] * xdata[r, c];        //累积计算y1[r]
                }
            }

            double[] SEy = new double[row];
            double[] SEline = new double[row];
            for (int i = 0; i < row; i++)
            {
                SEy[i] = Math.Pow((ydata[i] - yAverage), 2);
                SEline[i] = Math.Pow((ydata[i] - y1[i]), 2);
            }
            double sumSEline = SEline.Sum();
            double sumSEy = SEy.Sum();

            R2 = 1d;
            if (sumSEy != 0d)
            {
                R2 = 1 - sumSEline / sumSEy;
            }
            var RMSE = Math.Sqrt(sumSEline / row);
            CV = RMSE / yAverage;
            //return R2;
            //return y1;
        }

        private static void WriteResFile(string srcFilePath,List<string> resLines,string partailResFileName = "")
        {
            //获取源文件名            
            var fileName = Path.GetFileName(srcFilePath);
            var resFileName = fileName.Replace(".csv", string.Format("_结果{0}.csv", partailResFileName));
            var newPath = srcFilePath.Replace(fileName, resFileName);      //获取结果的文件名

            var linesWithHeader = new List<string>();
            linesWithHeader.Add("Linear Regression Result,R2,CV(RMSE)");
            linesWithHeader.AddRange(resLines);

            File.WriteAllLines(newPath, linesWithHeader);

        }

        /// <summary>
        /// 按照给定的参数范围计算各个组合，然后对每个组合计算回归结果
        /// 遍历参数表的各个参数
        /// </summary>
        /// <param name="minParamCount">最小参数个数</param>
        /// <param name="maxParamCount">最大参数个数</param>
        /// <param name="xdata">全部参数的x集合</param>
        /// <param name="ydata">结果集</param>
        private static List<string> LinearRegRange(int minParamCount, int maxParamCount,double[,] xdata,double[] ydata,string path,string[] headers)
        {
            //获取矩阵的row 和 col
            var row = xdata.GetLength(0);
            var col = xdata.GetLength(1);
            

            //获取列下标，如果有20列，则下标数组为0-19            
            var colnumbers = new int[col];
            for (int i = 0; i < col; i++)
			{
			    colnumbers[i] = i;
			}            

            List<string> resLines = new List<string>(); //结果数据行

            for (int i = minParamCount; i <= maxParamCount; i++)
            {
                var combs = PermutationAndCombination<int>.GetCombination(colnumbers,i);
                double[,] xdata1 = new double[row, i + 1];
                foreach (var cb in combs)
                {
                    //从完整源数据正提取当前组合的新数据                    
                    for (int r = 0; r < row; r++)
                    {
                        xdata1[r, 0] = 1;            //第一列置为1
                        for (int c = 1; c <= i; c++)
                        {
                            xdata1[r, c] = xdata[r, cb[c - 1]];
                        }
                    }
                    var p = LinearReg(xdata1, ydata);
                    var line = GeneResultSTring(p, headers,cb);
                    double R2, CV,F,sig;
                    CalR2CVFsig(xdata1, ydata, p,out R2,out CV,out F,out sig);
                    line += "," + R2.ToString("0.#####");
                    line += "," + CV.ToString("0.#####");
                    resLines.Add(line);
                }
            }

            return resLines;

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
            xdata = new double[row, col - 1];
            ydata = new double[row];
            //读取数据源赋值到矩阵
            for (int r = 0; r < row; r++)
            {
                var curLine = fileLines[r + 1].Split(',');      //第一行默认为header
                //xdata[r, 0] = 1d;       //第一列赋值为1
                //for (int c = 1; c < col; c++)
                //{
                //    xdata[r, c] = Convert.ToDouble(curLine[c - 1]);
                //}
                //直接获取数据
                for (int c = 0; c < col - 1; c++)
                {
                    xdata[r, c] = Convert.ToDouble(curLine[c]);
                }

                ydata[r] = Convert.ToDouble(curLine[col - 1]);     //获取到实际结果,y
            }
        }

        /// <summary>
        /// 从cvs文件中读取数据
        /// </summary>
        /// <param name="fileLines"></param>
        /// <param name="headers"></param>
        /// <param name="xdata"></param>
        /// <param name="ydata"></param>
        private static void ReadDataOld(string[] fileLines, out string[] headers, out double[,] xdata, out double[] ydata)
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
        /// 生成结果的公式
        /// </summary>
        /// <param name="p"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private static string GeneResultSTring(Vector<double> p, string[] headers,int[] cols)
        {
            StringBuilder sbRes = new StringBuilder();
            sbRes.Append("F: ");
            for (int i = 0; i < p.Count; i++)
            {
                if (i > 0)
                {
                    if (p[i] > 0)
                        sbRes.Append(" + ");
                    else
                    {
                        sbRes.Append(" - ");
                    }
                    sbRes.Append(string.Format("{0:0.##} * {1}", Math.Abs(p[i]), headers[cols[i - 1]]));        //header从第二个数值开始
                }
                else
                {
                    sbRes.Append(p[i].ToString("0.##"));       //第一个数值是常量
                }
            }
            //sbRes.Append("\"");
            return sbRes.ToString();
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

    }
}
