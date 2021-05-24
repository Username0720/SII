using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SII
{
    public partial class Form1 : Form
    {
        public int points_count = 0;
        public int count;
        Point[] points = new Point[11];
        public bool use_points = true;
        Graphics g;
        public Pen brush = new Pen(Color.Black, 2);
        public Pen brush2 = new Pen(Color.Chocolate, 1);
        public Pen brush3 = new Pen(Color.Blue, 1);
        public double t, k, short_way;

        
        //кол-во точек на полотне, которое вы задаете
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)        
        {
            count = Convert.ToInt32(numericUpDown1.Value);
        }

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            count = Convert.ToInt32(numericUpDown1.Value);
        }

        private void button2_Click(object sender, EventArgs e)      //кнокпка очистить все
        {
            pictureBox1.Refresh();
            use_points = true;
            points_count = 0;
            textBox3.Text = "";
            Array.Clear(points, 0, points.Length);
        }
        
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)     //расстановка точек на полотне
        {
            if (use_points == true)
            {
                Point(e.X, e.Y);
                points[points_count].X = e.X;
                points[points_count].Y = e.Y;
                points_count++;
                if (points_count >= count) 
                    use_points = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            t = Convert.ToDouble(textBox1.Text);
            k = Convert.ToDouble(textBox2.Text);
            List<int[]> arList = new List<int[]>();
            int[] track = Make_Track((int)numericUpDown1.Value);

            points[track.Length - 1] = points[0];
            track = change_Track(track);
            arList.Add(track);
            textBox3.Text += t + Environment.NewLine;
            while (t > 1)
            {
                short_way = Milky_Way(track, points);
                int[] new_track = new int[track.Length];
                track.CopyTo(new_track, 0);
                while (!check_List(arList, new_track))
                    new_track = change_Track(new_track);
                arList.Add(new_track);
                if (Milky_Way(new_track, points) <= short_way)
                {
                    short_way = Milky_Way(new_track, points);
                    track = new_track;
                }
                else
                {
                    if (checkTemper(Milky_Way(new_track, points) - short_way, t))
                    {
                        short_way = Milky_Way(new_track, points);
                        track = new_track;
                    }

                }
                t = k * t;
                textBox3.Text += t + Environment.NewLine;
            }

            for (int i = 1; i < track.Length; i++)
            {
                g.DrawLine(brush2, points[track[i - 1]].X, points[track[i - 1]].Y, points[track[i]].X, points[track[i]].Y);
            }

            textBox3.Text += ("Метод отжига: " + Milky_Way(track, points) + Environment.NewLine);
        }

        //функции
        #region functions
        public static double Track(float x1, float y1, float x2, float y2)      //расстояниe м/у 2мя точками
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private void Point(float x, float y)
        {
            g.DrawEllipse(brush, x, y, brush.Width, brush.Width);
        }

        public static int[] change_Track(int[] track)    //изменение последовательности точек
        {
            Random rnd = new Random();
            int[] new_track = new int[track.Length];
            track.CopyTo(new_track, 0);
            int first = 0, second = 0;
            while (first == second)
            {
                first = rnd.Next(1, track.Length - 1);
                second = rnd.Next(1, track.Length - 1);
            }
            int cont = new_track[first];
            new_track[first] = new_track[second];
            new_track[second] = cont;
            return new_track;
        }

        public static double Milky_Way(int[] track, Point[] points)      //весь путь
        {
            double milky_way = 0;
            for (int i = 1; i < track.Length; i++)
            {
                milky_way += Track(points[track[i]].X, points[track[i]].Y, points[track[i - 1]].X, points[track[i - 1]].Y);
            }
            return milky_way;
        }
        
        //проверка повторяющихся последовательностей точек
        public static bool check_List(List<int[]> arList, int[] track)
        {
            bool check = true;
            for (int i = 0; i < arList.Count; i++)
            {
                if (track.SequenceEqual(arList[i]))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }

        //сравнение P* и случайного числа
        public static bool checkTemper(double s, double t)
        {
            Random rnd = new Random();
            double prob = 100 * Math.Pow(Math.E, -(s / t));
            if (prob > rnd.Next(1, 100)) return true;
            else return false;
        }

        public static int[] Make_Track(int count)   //мейк трек
        {
            int[] track = new int[count + 1];
            for (int i = 0; i < count; i++)
                track[i] = i;
            track[0] = 0;
            track[track.Length - 1] = track[0];
            return track;
        }
        #endregion
    }
}
