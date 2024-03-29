﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Input;


namespace Radar_Analysis_Program
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : System.Windows.Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        public List<MyDataModel> dataList = new List<MyDataModel>();
        public CheckBox[] checkBoxes;
        public String[] checkbox_name;

        public const int MAX_NODE = 200;

        TextBox[] textBoxes = new TextBox[MAX_NODE];
        Rectangle[] rectangles = new Rectangle[MAX_NODE];
        Polyline[] merge_line = new Polyline[MAX_NODE];
        Polyline[] dist_lines = new Polyline[20];
        Grid[] dist_line_texts = new Grid[20];
        Polyline[] car_lanes = new Polyline[MAX_NODE];
        Mat LUT_img = new Mat(new OpenCvSharp.Size(400, 800), MatType.CV_8UC1, 0);

        DateTime _starttime;
        DateTime _checktime;
        DateTime total_time;
        DateTime dbcompareDT;
        DateTime dbcompareDT2; // 영상 시간 비교하기 위해
        TimeSpan diff;
        TimeSpan diff2;

        private double max_lat = 20;
        private double max_long = 200;
        private System.Windows.Point shift_pos;

        int number = 0; // DB  n 번째 

        double _previousValue_check = 0; //슬라이더 값 +, -  체크
        int speed_check = 0;

        string dbcomparetime;
        string text_str = "";
        string textblock1;
        string textblock2;
        string textblock3;
        string textblock4; //number
        string textblock5; //distance 
        string textblock6;  // media_time 영상 시간
        string textblock7; //dura 영상 시간 차

        string firsttime;
        string secondtime;

        double duration = 0;
        private MySqlConnection conn;

        private double[] Lane_width = new double[6] { 3.3f, 3.3f, 3.3f, 3.3f, 3.3f, 3.3f };
        private double[] Lane_shift = new double[9] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        private double[] Lane_Point = new double[6 + 1] { -9.9f, -6.6f, -3.3f, 0.0f, 3.3f, 6.6f, 9.9f };
        private double Dist_Lane_gap = 25.0f;

        private MyDataModel[] this_frame_data = new MyDataModel[MAX_NODE];
        private bool[] exist = new bool[MAX_NODE];
        private int[] LUT_ID = new int[MAX_NODE];
        public LinkedList<MyDataModel>[] Obj_inf = new LinkedList<MyDataModel>[MAX_NODE];

        public double Shift = 17.40;
        public int Angle = 20;

        private int MINID = 0;
        private int MAXID = 99;

        #region ACTIVE
        public static bool Filter_NofObj_ACTIVE = false;
        public static bool Filter_Distance_ACTIVE = false;
        public static bool Filter_Azimuth_ACTIVE = false;
        public static bool Filter_VrelOncome_ACTIVE = false;
        public static bool Filter_VrelDepart_ACTIVE = false;
        public static bool Filter_RCS_ACTIVE = false;
        public static bool Filter_Lifetime_ACTIVE = false;
        public static bool Filter_Size_ACTIVE = false;
        public static bool Filter_ProbExists_ACTIVE = false;
        public static bool Filter_Y_ACTIVE = false;
        public static bool Filter_X_ACTIVE = false;
        public static bool Filter_VYRightLeft_ACTIVE = false;
        public static bool Filter_VXOncome_ACTIVE = false;
        public static bool Filter_VYLeftRight_ACTIVE = false;
        public static bool Filter_VXDepart_ACTIVE = false;
        #endregion
        #region MIN
        public static int Filter_NofObj_MIN = 0;
        public static double Filter_Distance_MIN = 0.0;
        public static double Filter_Azimuth_MIN = 0.0;
        public static double Filter_VrelOncome_MIN = 0.0;
        public static double Filter_VrelDepart_MIN = 0.0;
        public static double Filter_RCS_MIN = -50.0;
        public static double Filter_Lifetime_MIN = 0.0;
        public static double Filter_Size_MIN = 0.0;
        public static int Filter_ProbExists_MIN = 0;
        public static double Filter_Y_MIN = -409.5;
        public static double Filter_X_MIN = -500;
        public static double Filter_VYRightLeft_MIN = 0.0;
        public static double Filter_VXOncome_MIN = 0.0;
        public static double Filter_VYLeftRight_MIN = 0.0;
        public static double Filter_VXDepart_MIN = 0.0;
        #endregion
        #region MAX
        public static int Filter_NofObj_MAX = 4095;
        public static double Filter_Distance_MAX = 409.5;
        public static double Filter_Azimuth_MAX = 50.375;
        public static double Filter_VrelOncome_MAX = 128.993;
        public static double Filter_VrelDepart_MAX = 128.993;
        public static double Filter_RCS_MAX = 52.375;
        public static double Filter_Lifetime_MAX = 409.5;
        public static double Filter_Size_MAX = 102.375;
        public static int Filter_ProbExists_MAX = 7;
        public static double Filter_Y_MAX = 409.5;
        public static double Filter_X_MAX = 1138.2;
        public static double Filter_VYRightLeft_MAX = 128.993;
        public static double Filter_VXOncome_MAX = 128.993;
        public static double Filter_VYLeftRight_MAX = 128.993;
        public static double Filter_VXDepart_MAX = 128.993;
        #endregion

        public class MyDataModel
        {
            // General      
            public int ID;
            public double DistLong;
            public double DistLat;
            public double VrelLong;
            public int DynProp;
            public double VrelLat;
            public double RCS;

            // Quality
            public int DistLat_rms;
            public int DistLong_rms;
            public int VrelLat_rms;
            public int VrelLong_rms;
            public int ArelLat_rms;
            public int ArelLong_rms;
            public int Orientation_rms;
            public int MirrorProb;
            public int MeasState;
            public int ProbOfExist;

            // Extended
            public double ArelLat;
            public double ArelLong;
            public int Class;
            public double OrientationAngle;
            public double Length;
            public double Width;

            // Other
            public DateTime Timestamp;
            public double Distance;
            public double Velocity;
            public double Size;
            public int Zone;
            public bool Noise;
            public bool Virtual;
            public bool Merging;
            public int Merged;

            public bool Finish_Analyzing;

            public object Clone()
            {
                return this.MemberwiseClone();
            }

        }

        public Window1(MySqlConnection connection)
        {
            conn = connection;
            InitializeComponent();


            for (int NODE = 0; NODE < MAX_NODE; NODE++)
            {
                Obj_inf[NODE] = new LinkedList<MyDataModel>();
                LUT_ID[NODE] = -1;
                exist[NODE] = false;
                Obj_inf[NODE].Clear();
            }

            Load_Setting_Value();
            Set_Obj_TextBox();
            Set_map_value();
            AngleShiftText();

            draw_play_btn();
        }

        #region Draw Func
        private void Set_Obj_TextBox()
        {
            checkBoxes = new CheckBox[] { text_time, text_id, text_distlat, text_distlong, text_vrellat, text_vrellong, text_velocity, text_rsc, text_probofexist, text_class, text_zone, text_length, text_width, text_DynProp, text_ObjSize, text_ObjCount };
            checkbox_name = new String[] { "Time", "ID", "DistLat", "DistLong", "VrelLat", "VrelLong", "Velocity", "RCS", "ProbOfExist", "Class", "Zone", "Length", "Width", "DynProp", "Size", "Count" };

            for (int i = 0; i < MAX_NODE; i++)
            {
                TextBox textBox = new TextBox();
                textBox.Text = "";
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Margin = new Thickness(10, 0, 0, 0);

                textBoxes[i] = textBox;

                textBoxes[i].Visibility = Visibility.Hidden;
                Data_Draw.Children.Add(textBoxes[i]);
            }
        }
        private void Set_map_value()
        {
            InitializeLanePoint();

            int point_num = (int)(max_long / Dist_Lane_gap) + 1;

            #region Dist Line
            int dist_line_index = 0;
            for (int line_n = 0; line_n < point_num; line_n++)
            {
                Polyline distline = new Polyline();
                distline.Points = new PointCollection()
                {
                    new System.Windows.Point(0, 0),
                    new System.Windows.Point(Data_Draw.ActualWidth, 0)
                };
                distline.Stroke = Brushes.Yellow;
                distline.StrokeThickness = 1;
                distline.StrokeDashArray = new DoubleCollection() { 15, 15 };

                Grid panel = new Grid(); //사각형을 감싸줄 Panel 생성
                TextBlock textBlock = new TextBlock();//Text 생성
                textBlock.Text = ((int)(Dist_Lane_gap * line_n)).ToString() + "m";
                textBlock.Foreground = Brushes.White;
                panel.Children.Add(textBlock);//Panel에 Text 추가

                int Y = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * line_n) + 1)) / (2 * (max_long + Dist_Lane_gap))));

                Canvas.SetLeft(distline, 0);//Panel 위치 조정
                Canvas.SetTop(distline, Y);
                Canvas.SetLeft(panel, Data_Draw.ActualWidth - (textBlock.Text.Length + 2) * 5);//Panel 위치 조정
                Canvas.SetTop(panel, Y - 9);

                dist_lines[dist_line_index] = distline;
                dist_line_texts[dist_line_index] = panel;
                dist_line_index++;
            }
            #endregion

            #region Lane
            int car_lane_index = 0;

            for (int l = 0; l < 6 + 1; l++)
            {
                for (int p = 0; p < point_num - 1; p++)
                {
                    int X1 = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (Lane_Point[l] + Lane_shift[p]));
                    int X2 = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (Lane_Point[l] + Lane_shift[p + 1]));
                    int Y1 = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * p) + 1)) / (2 * (max_long + Dist_Lane_gap))));
                    int Y2 = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * (p + 1)) + 1)) / (2 * (max_long + Dist_Lane_gap))));

                    Polyline lane = new Polyline();
                    if (l == 3)
                        lane.Stroke = Brushes.Yellow;
                    else
                        lane.Stroke = Brushes.White;
                    lane.StrokeThickness = 3;

                    lane.Points = new PointCollection()
                                {
                                    new System.Windows.Point(X1, Y1),
                                    new System.Windows.Point(X2, Y2)
                                };
                    Canvas.SetLeft(lane, 0);//Panel 위치 조정
                    Canvas.SetTop(lane, 0);
                    car_lanes[car_lane_index] = lane;
                    car_lane_index++;
                }
            }
            #endregion

            Update_LaneWidthText();

            Draw_map();
        }
        private void Draw_map()
        {
            int point_num = (int)(max_long / Dist_Lane_gap) + 1;
            for (int i = 0; i < point_num; i++)
            {
                Data_Draw.Children.Add(dist_lines[i]);
                Data_Draw.Children.Add(dist_line_texts[i]);
            }
            for (int i = 0; i < (6 + 1) * (point_num - 1); i++)
            {
                Data_Draw.Children.Add(car_lanes[i]);
            }
        }
        private void Update_map()
        {
            InitializeLanePoint();

            int point_num = (int)(max_long / Dist_Lane_gap) + 1;

            #region Dist Line
            for (int dist_line_index = 0; dist_line_index < point_num; dist_line_index++)
            {
                TextBlock textBlock = new TextBlock();//Text 생성
                textBlock.Text = ((int)(Dist_Lane_gap * dist_line_index)).ToString() + "m";

                int Y = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * dist_line_index) + 1)) / (2 * (max_long + Dist_Lane_gap))));
                dist_lines[dist_line_index].Points[0] = new System.Windows.Point(0, Y);
                dist_lines[dist_line_index].Points[1] = new System.Windows.Point(Data_Draw.ActualWidth, Y);
                Canvas.SetLeft(dist_lines[dist_line_index], 0);//Panel 위치 조정
                Canvas.SetTop(dist_lines[dist_line_index], 0);
                Canvas.SetLeft(dist_line_texts[dist_line_index], Data_Draw.ActualWidth - (textBlock.Text.Length + 2) * 5);//Panel 위치 조정
                Canvas.SetTop(dist_line_texts[dist_line_index], Y - 9);
            }
            #endregion

            #region Lane
            int car_lane_index = 0;
            for (int l = 0; l < 6 + 1; l++)
            {
                for (int p = 0; p < point_num - 1; p++)
                {
                    int X1 = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (Lane_Point[l] + Lane_shift[p]));
                    int X2 = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (Lane_Point[l] + Lane_shift[p + 1]));
                    int Y1 = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * p) + 1)) / (2 * (max_long + Dist_Lane_gap))));
                    int Y2 = (int)(Data_Draw.ActualHeight - ((Data_Draw.ActualHeight * Dist_Lane_gap * ((2 * (p + 1)) + 1)) / (2 * (max_long + Dist_Lane_gap))));

                    car_lanes[car_lane_index].Points[0] = new System.Windows.Point(X1, Y1);
                    car_lanes[car_lane_index].Points[1] = new System.Windows.Point(X2, Y2);
                    Canvas.SetLeft(car_lanes[car_lane_index], 0);
                    Canvas.SetTop(car_lanes[car_lane_index], 0);
                    car_lane_index++;
                }
            }
            #endregion

            #region Lane Labeling
            int label_index = 0;
            LUT_img = Mat.Zeros(LUT_img.Size(), MatType.CV_8UC1);
            for (int n_lane = 0; n_lane < 6; n_lane++)
            {
                for (int zone = 0; zone < point_num - 1; zone++)
                {
                    label_index++;

                    int POINTB = (int)((LUT_img.Size().Height / max_long) * ((zone) * Dist_Lane_gap));
                    int POINTT = (int)((LUT_img.Size().Height / max_long) * ((zone + 1) * Dist_Lane_gap));
                    int POINTLB = (int)((LUT_img.Size().Width / (max_lat * 2)) * ((Lane_Point[n_lane] + Lane_shift[zone]) + max_lat));
                    int POINTLT = (int)((LUT_img.Size().Width / (max_lat * 2)) * ((Lane_Point[n_lane] + Lane_shift[zone + 1]) + max_lat));
                    int POINTRT = (int)((LUT_img.Size().Width / (max_lat * 2)) * ((Lane_Point[n_lane + 1] + Lane_shift[zone + 1]) + max_lat));
                    int POINTRB = (int)((LUT_img.Size().Width / (max_lat * 2)) * ((Lane_Point[n_lane + 1] + Lane_shift[zone]) + max_lat));

                    OpenCvSharp.Point[] pt = new OpenCvSharp.Point[4];
                    pt[0] = new OpenCvSharp.Point(POINTLB, POINTB);
                    pt[1] = new OpenCvSharp.Point(POINTLT, POINTT);
                    pt[2] = new OpenCvSharp.Point(POINTRT, POINTT);
                    pt[3] = new OpenCvSharp.Point(POINTRB, POINTB);
                    LUT_img.FillConvexPoly(pt, label_index);
                }
            }
            /*
                        Cv2.ImShow("test", LUT_img);
                        Cv2.WaitKey(0);
            */
            #endregion
        }
        #endregion

        #region obj info process

        private void Read()
        {
            if (number + 1 < dataList.Count)
            {
                if (dataList[number].Timestamp <= dbcompareDT)
                {
                    while (!((number + 1 >= dataList.Count) || (dataList[number].Timestamp != dataList[number + 1].Timestamp)))
                    {
                        this_frame_data[dataList[number].ID] = (MyDataModel)dataList[number].Clone();
                        exist[dataList[number].ID] = true;
                        number++;
                    }
                    this_frame_data[dataList[number].ID] = (MyDataModel)dataList[number].Clone();
                    exist[dataList[number].ID] = true;
                    number++;

                    Radar_Filter_Setting();
                    Radar_RotateShift();
                    test_code();
                    Check_New_Obj();
                    if((bool)apply_algorithm.IsChecked)
                        Object_Kalman();

                    Check_zone_index();

                    Merge_Obj();

                    Save_this_frame_obj_data();
                    Draw_this_frame_obj_data();
                    Clear_this_frame_obj_data();
                }
            }
        }
        private void insert_Obj_data(int index)
        {
            Rectangle rect = new Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromRgb(244, 143, 61)),
                StrokeThickness = 0
            };
            rect.Tag = index;
            rectangles[index] = rect;
            Data_Draw.Children.Add(rectangles[index]);

            Polyline line = new Polyline();
            line.Points = new PointCollection()
            {
                new System.Windows.Point(0, 0),
                new System.Windows.Point(0, 0)
            };
            line.Stroke = Brushes.Yellow;
            line.StrokeThickness = 1;
            line.Visibility = Visibility.Hidden;
            merge_line[index] = line;
            Data_Draw.Children.Add(merge_line[index]);

            if (CheckBox.IsChecked == true)
                textBoxes[index].Visibility = Visibility.Visible;
            else
                textBoxes[index].Visibility = Visibility.Hidden;
        }
        private void remove_Obj_data(int index)
        {
            Obj_inf[index].Clear();
            if (Data_Draw.Children.Contains(rectangles[index]))
            {
                Data_Draw.Children.Remove(rectangles[index]);
                textBoxes[index].Visibility = Visibility.Hidden;
            }
            if (Data_Draw.Children.Contains(merge_line[index]))
                Data_Draw.Children.Remove(merge_line[index]);
        }
        private void clear_Obj_data()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (Data_Draw.Children.Contains(rectangles[i]))
                {
                    Data_Draw.Children.Remove(rectangles[i]);
                    textBoxes[i].Visibility = Visibility.Hidden;
                }
                if (Data_Draw.Children.Contains(merge_line[i]))
                    Data_Draw.Children.Remove(merge_line[i]);
                Obj_inf[i].Clear();
            }
            Clear_this_frame_obj_data();
        }
        private void Radar_Filter_Setting()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    Filter_Nofobj(i);
                    Filter_Distance(i);
                    Filter_Azimuth(i);
                    Filter_VrelOncome(i);
                    Filter_VrelDepart(i);
                    Filter_RCS(i);
                    Filter_Lifetime(i);
                    Filter_Size(i);
                    Filter_ProbExists(i);
                    Filter_X(i);
                    Filter_Y(i);
                    Filter_VYRightLeft(i);
                    Filter_VXOncome(i);
                    Filter_VYLeftRight(i);
                    Filter_VXDepart(i);
                }
            }
        }
        private void Radar_RotateShift()
        {
            double Radian = Angle * (Math.PI / 180);
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    double fTempX = this_frame_data[i].DistLat * Math.Cos(Radian) - this_frame_data[i].DistLong * Math.Sin(Radian);
                    double fTempY = this_frame_data[i].DistLat * Math.Sin(Radian) + this_frame_data[i].DistLong * Math.Cos(Radian);
                    this_frame_data[i].DistLat = fTempX + Shift;
                    this_frame_data[i].DistLong = fTempY;
                }
            }
        }
        private void Check_New_Obj()
        {
            for (int i = 0; i < MAX_NODE / 2; i++)
            {
                if (exist[i])
                {
                    if (Obj_inf[i].Count >= 10)
                    {
                        this_frame_data[i].Finish_Analyzing = true;
                        MyDataModel First_obj = Obj_inf[i].First.Value;
                        float dif_Dist = (float)Math.Sqrt(Math.Pow(this_frame_data[i].DistLong - First_obj.DistLong, 2) + Math.Pow(this_frame_data[i].DistLat - First_obj.DistLat, 2));
                        if (dif_Dist < 5)
                        {
                            this_frame_data[i].Noise = true;
                        }
                    }
                    if(Obj_inf[i].Count != 0)
                    {
                        MyDataModel Last_obj = Obj_inf[i].Last.Value;
                        float dif_Dist = (float)Math.Sqrt(Math.Pow(this_frame_data[i].DistLong - Last_obj.DistLong, 2) + Math.Pow(this_frame_data[i].DistLat - Last_obj.DistLat, 2));
                        if (dif_Dist > 10)
                        {
                            this_frame_data[i].Finish_Analyzing = false;
                            int merge_id = 100;
                            while (merge_id < MAX_NODE)
                            {
                                if (Obj_inf[merge_id].Count == 0)
                                {
                                    insert_Obj_data(merge_id);
                                    Obj_inf[merge_id] = new LinkedList<MyDataModel>(Obj_inf[i]);
                                    remove_Obj_data(i);
                                    break;
                                }
                                merge_id++;
                            }
                        }
                    }
                }
            }
        }
        private void test_code()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    if (this_frame_data[i].Velocity <= 3.0 || this_frame_data[i].VrelLat > 13 || this_frame_data[i].VrelLat < -13) //가로 방향 큰 값. 
                        this_frame_data[i].Noise = true;

                    if (this_frame_data[i].Length < 0.8)
                        this_frame_data[i].Noise = true;

                    if (this_frame_data[i].Class == 7 && this_frame_data[i].ProbOfExist <= 3)  //확률 낮고 모르는 객체
                        this_frame_data[i].Noise = true;

                    if (this_frame_data[i].Class == 7 && (this_frame_data[i].VrelLat_rms >= 28 || this_frame_data[i].VrelLong_rms >= 28))  //표준편차 큰 값 제거.
                        this_frame_data[i].Noise = true;

                    if (this_frame_data[i].DistLat > 0 && this_frame_data[i].VrelLong > 0)  //역주행
                        this_frame_data[i].Noise = true;

                    if (this_frame_data[i].DistLat < 0 && this_frame_data[i].VrelLong < 0)
                        this_frame_data[i].Noise = true;


                    if (this_frame_data[i].DistLong > 100.0)
                        this_frame_data[i].Noise = true;
                }
            }
        }
        double kalmanFilter(double X, double Z, double P_value)
        {

            double x_next, P_next, x, P, K, Q, R;

            P = P_value;
            //Q = 0.002;
            //R = 0.03;
            Q = 0.022;
            R = 0.617;  // 측정 잡음 공분산 높을 수록  보정

            x_next = X;
            P_next = P + Q;
            K = P_next / (P_next + R);
            x = x_next + K * (Z - x_next);
            P = (1 - K) * P_next;

            return x;
        }
        private void Object_Kalman()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])  //부드럽게 하는 보정 부분
                {
                    if (this_frame_data[i].Finish_Analyzing)
                    {
                        double last_data_DistLat = Obj_inf[i].Last.Value.DistLat;
                        double last_data_DistLong = Obj_inf[i].Last.Value.DistLong;

                        this_frame_data[i].DistLat = kalmanFilter(last_data_DistLat, this_frame_data[i].DistLat, 0.0);
                        this_frame_data[i].DistLong = kalmanFilter(last_data_DistLong, this_frame_data[i].DistLong, 1.0);
                    }
                }
                else
                {
                    if (Obj_inf[i].Count >= 2)
                    {
                        if ((Obj_inf[i].Last.Value.Zone != 0) && !Obj_inf[i].Last.Value.Noise && Obj_inf[i].Last.Value.Finish_Analyzing)
                        {
                            TimeSpan difTime = dbcompareDT - Obj_inf[i].Last.Value.Timestamp;
                            if (difTime.Seconds < 3)
                            {
                                if (i < MAX_NODE / 2)
                                {
                                    int merge_id = 100;
                                    while (merge_id < MAX_NODE)
                                    {
                                        if (Obj_inf[merge_id].Count == 0)
                                        {
                                            insert_Obj_data(merge_id);
                                            Obj_inf[merge_id] = new LinkedList<MyDataModel>(Obj_inf[i]);
                                            remove_Obj_data(i);
                                            break;
                                        }
                                        merge_id++;
                                    }
                                }
                                else
                                {
                                    double last_data_DistLat = Obj_inf[i].Last.Value.DistLat;
                                    double last_data_DistLong = Obj_inf[i].Last.Value.DistLong;
                                    double last_last_data_DistLat = Obj_inf[i].Last.Previous.Value.DistLat;
                                    double last_last_data_DistLong = Obj_inf[i].Last.Previous.Value.DistLong;

                                    exist[i] = true;
                                    this_frame_data[i] = (MyDataModel)Obj_inf[i].Last.Value.Clone();
                                    this_frame_data[i].DistLat = last_data_DistLat;// kalmanFilter(last_last_data_DistLat, last_data_DistLat, 0.0);
                                    this_frame_data[i].DistLong = kalmanFilter(last_last_data_DistLong, 3.2 * last_data_DistLong - 2.2 * last_last_data_DistLong, 1.0);
                                    this_frame_data[i].Virtual = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Check_zone_index()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    int x = (int)(((this_frame_data[i].DistLat * (-1.0)) + max_lat) * 10);
                    int y = (int)(this_frame_data[i].DistLong * 4);
                    if ((x >= 0) && (x < LUT_img.Width) && (y >= 0) && (y < LUT_img.Height))
                    {
                        this_frame_data[i].Zone = LUT_img.At<char>(y, x);
                    }
                    else
                        this_frame_data[i].Zone = 0;

                    if (this_frame_data[i].Zone == 0)
                    {
                        //exist[i] = false;
                    }
                }
            }
        }
        private void Merge_Obj()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i] && (Obj_inf[i].Count > 0))
                {
                    if (Obj_inf[i].Last.Value.Merged != 0)
                    {
                        if (Obj_inf[Obj_inf[i].Last.Value.Merged - 1].Count != 0)
                        {
                            this_frame_data[i].Merged = Obj_inf[i].Last.Value.Merged;
                        }
                        else
                        {
                            this_frame_data[i].Merged = i + 1;
                        }
                    }
                }
            }
            for (int i = 0; i < MAX_NODE - 1; i++)
            {
                if (exist[i] && (this_frame_data[i].Merged == 0) && !this_frame_data[i].Noise && this_frame_data[i].Finish_Analyzing)
                {
                    double DistLatgap, DistLonggap, Distgap, Velocity_gap;
                    double min_Distgap = 1000;

                    for (int j = i + 1; j < MAX_NODE; j++)
                    {
                        if (exist[j] && (this_frame_data[j].Merged == 0) && !this_frame_data[j].Noise && this_frame_data[j].Finish_Analyzing)
                        {
                            DistLatgap = Math.Abs(this_frame_data[i].DistLat - this_frame_data[j].DistLat);
                            DistLonggap = Math.Abs(this_frame_data[i].DistLong - this_frame_data[j].DistLong);
                            Distgap = Math.Sqrt(Math.Pow(DistLatgap, 2) + Math.Pow(DistLonggap, 2));
                            Velocity_gap = Math.Abs(this_frame_data[i].Velocity - this_frame_data[j].Velocity);

                            if ((DistLatgap <= 3) && (DistLonggap <= 12.0)  && (Velocity_gap <= 3.0))
                            {
                                if ((DistLatgap <= 1.5) && (DistLonggap <= 10.0) && (Velocity_gap <= 1.0))
                                {
                                    if (this_frame_data[i].Virtual != this_frame_data[j].Virtual)
                                    {
                                        if (this_frame_data[i].Virtual)
                                        {
                                            remove_Obj_data(j);
                                            insert_Obj_data(j);
                                            Obj_inf[j] = new LinkedList<MyDataModel>(Obj_inf[i]);
                                            remove_Obj_data(i);
                                            exist[i] = false;
                                        }
                                        else
                                        {
                                            remove_Obj_data(i);
                                            insert_Obj_data(i);
                                            Obj_inf[i] = new LinkedList<MyDataModel>(Obj_inf[j]);
                                            remove_Obj_data(j);
                                            exist[j] = false;
                                        }
                                    }
                                    else if (this_frame_data[i].Virtual && this_frame_data[j].Virtual)
                                    {

                                    }
                                    else if ((this_frame_data[i].Length <= 4.4) || (this_frame_data[j].Length <= 4.4))
                                    {
                                        if (Obj_inf[i].Count <= Obj_inf[j].Count)
                                        {
                                            if (min_Distgap > Distgap)
                                            {
                                                this_frame_data[i].Merged = j + 1;
                                                this_frame_data[j].Merging = true;
                                                min_Distgap = Distgap;
                                            }
                                        }
                                        else if (Obj_inf[j].Count <= Obj_inf[i].Count)
                                        {
                                            if (min_Distgap > Distgap)
                                            {
                                                this_frame_data[j].Merged = i + 1;
                                                this_frame_data[i].Merging = true;
                                                min_Distgap = Distgap;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (this_frame_data[i].Virtual != this_frame_data[j].Virtual)
                                    {
                                        if (this_frame_data[i].Virtual)
                                        {
                                            remove_Obj_data(j);
                                            insert_Obj_data(j);
                                            Obj_inf[j] = new LinkedList<MyDataModel>(Obj_inf[i]);
                                            remove_Obj_data(i);
                                            exist[i] = false;
                                        }
                                        else
                                        {
                                            remove_Obj_data(i);
                                            insert_Obj_data(i);
                                            Obj_inf[i] = new LinkedList<MyDataModel>(Obj_inf[j]);
                                            remove_Obj_data(j);
                                            exist[j] = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Save_this_frame_obj_data()
        {
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    if (Obj_inf[i].Count == 0)
                    {
                        insert_Obj_data(i);
                    }
                    Obj_inf[i].AddLast(this_frame_data[i]);
                    if (Obj_inf[i].Count >= 100)
                        Obj_inf[i].RemoveFirst();
                }
                else
                {
                    if (Obj_inf[i].Count != 0)
                    {
                        TimeSpan difTime = dbcompareDT - Obj_inf[i].Last.Value.Timestamp;
                        if ((difTime.Seconds > 0) || (difTime.Milliseconds > 300))
                        {
                            remove_Obj_data(i);
                        }
                    }
                }

            }
        }

        private void Draw_this_frame_obj_data()
        {
            int size_XL = 23;
            int size_L = 15;
            int size_M = 8;
            int size_S = 3;
            for (int i = 0; i < MAX_NODE; i++)
            {
                if (exist[i])
                {
                    int X = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (-1 * this_frame_data[i].DistLat));
                    int Y = (int)(Data_Draw.ActualHeight * ((max_long + Dist_Lane_gap - this_frame_data[i].DistLong - (Dist_Lane_gap / 2)) / (max_long + Dist_Lane_gap)));

                    textBoxes[i].Text = CheckBox_print(i);
                    if ((i >= MINID) && (i <= MAXID) && (this_frame_data[i].DistLat <= max_lat) && (this_frame_data[i].DistLat >= (-1 * max_lat)))
                    {
                        if (!this_frame_data[i].Finish_Analyzing)
                        {
                            rectangles[i].StrokeThickness = size_S;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromRgb(100, 100, 100));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Hidden;
                        }
                        else if (this_frame_data[i].Noise)
                        {
                            rectangles[i].StrokeThickness = size_S;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromRgb(50, 50, 50));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Hidden;
                        }
                        else if (this_frame_data[i].Merged != 0)
                        {
                            rectangles[i].StrokeThickness = size_M;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 0));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Visible;
                        }
                        else if (this_frame_data[i].Virtual)
                        {
                            rectangles[i].StrokeThickness = size_L;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromArgb(100, 244, 143, 61));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Visible;
                        }
                        else if (this_frame_data[i].Zone == 0)
                        {
                            rectangles[i].StrokeThickness = size_M;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromRgb(244, 143, 61));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            rectangles[i].StrokeThickness = size_L;
                            rectangles[i].Stroke = new SolidColorBrush(Color.FromRgb(244, 143, 61));

                            rectangles[i].Visibility = Visibility.Visible;
                            textBoxes[i].Visibility = Visibility.Visible;
                        }
                        Canvas.SetLeft(rectangles[i], X - (rectangles[i].StrokeThickness / 2));
                        Canvas.SetTop(rectangles[i], Y - rectangles[i].StrokeThickness);
                        Canvas.SetLeft(textBoxes[i], X + 10);
                        Canvas.SetTop(textBoxes[i], Y - 18);

                        if (this_frame_data[i].Merged != 0)
                        {
                            if (exist[this_frame_data[i].Merged - 1])
                            {
                                int X2 = (int)((Data_Draw.ActualWidth / 2) + (Data_Draw.ActualWidth / (max_lat * 2)) * (-1 * this_frame_data[this_frame_data[i].Merged - 1].DistLat));
                                int Y2 = (int)(Data_Draw.ActualHeight * ((max_long + Dist_Lane_gap - this_frame_data[this_frame_data[i].Merged - 1].DistLong - (Dist_Lane_gap / 2)) / (max_long + Dist_Lane_gap)));


                                merge_line[i].Points[0] = new System.Windows.Point(X, Y);
                                merge_line[i].Points[1] = new System.Windows.Point(X2, Y2);
                                Canvas.SetLeft(merge_line[i], 0);
                                Canvas.SetTop(merge_line[i], 0);
                                merge_line[i].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                merge_line[i].Visibility = Visibility.Hidden;
                            }
                        }
                        else
                        {
                            merge_line[i].Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        rectangles[i].Visibility = Visibility.Hidden;
                        textBoxes[i].Visibility = Visibility.Hidden;
                    }
                    if (textBoxes[i].Text == "")
                        textBoxes[i].Visibility = Visibility.Hidden;
                }
                else
                {
                    textBoxes[i].Visibility = Visibility.Hidden;
                }
            }
        }
        private void Clear_this_frame_obj_data()
        {
            for (int i = 0; i < MAX_NODE; i++)
                this_frame_data[i] = default(MyDataModel);
            System.Array.Clear(exist, 0, sizeof(bool) * MAX_NODE);
        }

        #region Filter Setting
        void Filter_Nofobj(int index)
        {
            if (Filter_NofObj_ACTIVE)
            {

            }
        }
        void Filter_Distance(int index)
        {
            if (Filter_Distance_ACTIVE)
            {
                //if ((this_frame_data[index].Distance >= Filter_Distance_MAX) || (this_frame_data[index].Distance <= Filter_Distance_MIN))
                //{
                //    this_frame_data[index] = default(MyDataModel);
                //    exist[index] = false;
                //}
            }
        }
        void Filter_Azimuth(int index)
        {
            if (Filter_Azimuth_ACTIVE)
            {

            }
        }
        void Filter_VrelOncome(int index)
        {
            if (Filter_VrelOncome_ACTIVE)
            {

            }
        }
        void Filter_VrelDepart(int index)
        {
            if (Filter_VrelDepart_ACTIVE)
            {

            }
        }
        void Filter_RCS(int index)
        {
            if (Filter_RCS_ACTIVE)
            {

            }
        }
        void Filter_Lifetime(int index)
        {
            if (Filter_Lifetime_ACTIVE)
            {

            }
        }
        void Filter_Size(int index)
        {
            if (Filter_Size_ACTIVE)
            {

            }
        }
        void Filter_ProbExists(int index)
        {
            if (Filter_ProbExists_ACTIVE)
            {

            }
        }
        void Filter_Y(int index)
        {
            if (Filter_Y_ACTIVE)
            {

            }
        }
        void Filter_X(int index)
        {
            if (Filter_X_ACTIVE)
            {

            }
        }
        void Filter_VYRightLeft(int index)
        {
            if (Filter_VYRightLeft_ACTIVE)
            {

            }
        }
        void Filter_VXOncome(int index)
        {
            if (Filter_VXOncome_ACTIVE)
            {

            }
        }
        void Filter_VYLeftRight(int index)
        {
            if (Filter_VYLeftRight_ACTIVE)
            {

            }
        }
        void Filter_VXDepart(int index)
        {
            if (Filter_VXDepart_ACTIVE)
            {

            }
        }
        #endregion

        #endregion

        void TimerTickHandler(object sender, EventArgs e)
        {
            double positionMs = mediaElement.Position.TotalMilliseconds;
            slider.Value = slider.Minimum + positionMs;

            diff = TimeSpan.FromMilliseconds(1);
            diff2 += diff;


            TimeSpan dura = mediaElement.Position; //영상 시간 계산

            dbcompareDT = _starttime.Add(dura);
            Read();


            dbcompareDT2 = _starttime.Add(dura);
            // textblock6 = dbcompareDT2.ToString("yyyy-MM-dd HH:mm:ss.fff"); //영상 시간 

            TimeSpan ts = new TimeSpan(0, 0, 0, 0, 300);
            double at = dura.TotalMilliseconds - diff2.TotalMilliseconds;


            textblock2 = dbcompareDT.ToString("yyyy-MM-dd HH:mm:ss.fff"); //db 시간
            textblock1 = number.ToString();
            text_str = textblock1 + "\n" + textblock2;

            Data_Text.Text = text_str;
        }

        #region setting
        private void db_connect(MySqlConnection connection, string first, string second)
        {
            try
            {
                connection.Open();
                string query = "SELECT * FROM test_1128_gc where time BETWEEN" + "'" + first + "'" + "AND" + "'" + second + "'" + ";";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MyDataModel data = new MyDataModel();

                            // General      
                            data.ID = (int)reader["ID"];
                            data.DistLat = (double)reader["DISTLAT"];
                            data.DistLong = (double)reader["DISTLONG"];
                            data.VrelLat = (double)reader["VRELLAT"];
                            data.VrelLong = (double)reader["VRELLONG"];
                            data.DynProp = (int)reader["DYNPROP"];
                            data.RCS = (double)reader["RCS"];

                            // Quality
                            data.DistLat_rms = (int)reader["DISTLAT_RMS"];
                            data.DistLong_rms = (int)reader["DISTLONG_RMS"];
                            data.VrelLat_rms = (int)reader["VRELLAT_RMS"];
                            data.VrelLong_rms = (int)reader["VRELLONG_RMS"];
                            data.ArelLat_rms = (int)reader["ARELLAT_RMS"];
                            data.ArelLong_rms = (int)reader["ARELLONG_RMS"];
                            data.Orientation_rms = (int)reader["ORIENTATION_RMS"];
                            data.MirrorProb = (int)reader["MIRRORPROB"];
                            data.MeasState = (int)reader["MEASSTATE"];
                            data.ProbOfExist = (int)reader["PROBOFEXIST"];

                            // Extended
                            data.ArelLong = (double)reader["ARELLAT"] * 0.01;
                            data.ArelLat = (double)reader["ARELLONG"] * 0.01;
                            data.Class = (int)reader["CLASS"];
                            data.OrientationAngle = (double)reader["ORIEMTATIONANGLE"] * 0.4;
                            data.Length = (double)reader["LENGTH"] * 0.2;
                            data.Width = (double)reader["WIDTH"] * 0.2;

                            // Other
                            data.Timestamp = (DateTime)reader["TIME"];
                            data.Distance = Math.Sqrt(Math.Pow(data.DistLat, 2) + Math.Pow(data.DistLong, 2));
                            data.Velocity = Math.Sqrt(Math.Pow(data.VrelLat, 2) + Math.Pow(data.VrelLong, 2));
                            data.Size = data.Length * data.Width;
                            data.Zone = 0;

                            dataList.Add(data);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("DB Read ERROR");
            }
        }

        #region Set Lane Info
        #region Set Lane Width
        private void LaneWidth1Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[0] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth1Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[0] -= 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth2Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[1] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth2Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[1] -= 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth3Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[2] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth3Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[2] -= 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth4Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[3] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth4Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[3] -= 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth5Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[4] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth5Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[4] -= 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth6Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[5] += 0.1f;
            Update_LaneWidthText();
        }
        private void LaneWidth6Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_width[5] -= 0.1f;
            Update_LaneWidthText();
        }
        private void Update_LaneWidthText()
        {
            LaneWidth1.Text = Lane_width[0].ToString("F1");
            LaneWidth2.Text = Lane_width[1].ToString("F1");
            LaneWidth3.Text = Lane_width[2].ToString("F1");
            LaneWidth4.Text = Lane_width[3].ToString("F1");
            LaneWidth5.Text = Lane_width[4].ToString("F1");
            LaneWidth6.Text = Lane_width[5].ToString("F1");

            Update_map();
        }
        private void InitializeLanePoint()
        {
            Lane_Point[0] = 0 - (Lane_width[0] + Lane_width[1] + Lane_width[2]);
            Lane_Point[1] = 0 - (Lane_width[1] + Lane_width[2]);
            Lane_Point[2] = 0 - (Lane_width[2]);
            Lane_Point[3] = 0;
            Lane_Point[4] = 0 + (Lane_width[3]);
            Lane_Point[5] = 0 + (Lane_width[3] + Lane_width[4]);
            Lane_Point[6] = 0 + (Lane_width[3] + Lane_width[4] + Lane_width[5]);
        }

        #endregion

        #region Set Lane Point
        private void LanePoint0Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[0] += 0.1f;
            Update_map();
        }
        private void LanePoint0Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[0] -= 0.1f;
            Update_map();
        }
        private void LanePoint25Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[1] += 0.1f;
            Update_map();
        }
        private void LanePoint25Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[1] -= 0.1f;
            Update_map();
        }
        private void LanePoint50Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[2] += 0.1f;
            Update_map();
        }
        private void LanePoint50Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[2] -= 0.1f;
            Update_map();
        }
        private void LanePoint75Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[3] += 0.1f;
            Update_map();
        }
        private void LanePoint75Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[3] -= 0.1f;
            Update_map();
        }
        private void LanePoint100Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[4] += 0.1f;
            Update_map();
        }
        private void LanePoint100Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[4] -= 0.1f;
            Update_map();
        }
        private void LanePoint125Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[5] += 0.1f;
            Update_map();
        }
        private void LanePoint125Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[5] -= 0.1f;
            Update_map();
        }
        private void LanePoint150Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[6] += 0.1f;
            Update_map();
        }
        private void LanePoint150Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[6] -= 0.1f;
            Update_map();
        }
        private void LanePoint175Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[7] += 0.1f;
            Update_map();
        }
        private void LanePoint175Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[7] -= 0.1f;
            Update_map();
        }
        private void LanePoint200Up_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[8] += 0.1f;
            Update_map();
        }
        private void LanePoint200Down_Click(object sender, RoutedEventArgs e)
        {
            Lane_shift[8] -= 0.1f;
            Update_map();
        }
        #endregion

        #region Set MIN & MAX ID
        private void MINID_Value_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                MINID = Convert.ToInt32(MINIDText.Text);
                if ((MINID < 0) || (MINID >= MAX_NODE))
                {
                    MINID = 0;
                    MINIDText.Text = MINID.ToString();
                }
            }
            catch
            {
                MINIDText.Text = (0).ToString();
            }
        }
        private void MAXID_Value_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                MAXID = Convert.ToInt32(MAXIDText.Text);
                if ((MAXID < 0) || (MAXID >= MAX_NODE))
                {
                    MAXID = MAX_NODE - 1;
                    MAXIDText.Text = MAXID.ToString();
                }
            }
            catch
            {
                MAXIDText.Text = (MAX_NODE - 1).ToString();
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #endregion

        #region Set Angle & Shift
        private void AngleUp_Click(object sender, RoutedEventArgs e)
        {
            Angle += 1;
            AngleShiftText();
        }
        private void AngleDown_Click(object sender, RoutedEventArgs e)
        {
            Angle -= 1;
            AngleShiftText();
        }
        private void ShiftUp_Click(object sender, RoutedEventArgs e)
        {
            Shift += 0.1;
            AngleShiftText();
        }
        private void ShiftDown_Click(object sender, RoutedEventArgs e)
        {
            Shift -= 0.1;
            AngleShiftText();
        }
        private void AngleShiftText()
        {
            AngleText.Text = Angle.ToString();
            ShiftText.Text = Shift.ToString("F1");
        }
        #endregion
        #endregion

        #region form_Click
        private void Form_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Form_Resize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
        }
        private void Form_Hide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            fSetting testWindow2 = new fSetting();
            testWindow2.Set_Filter_Form();
            testWindow2.Show();
        }
        #endregion

        #region mediaElement
        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            //총 재생시간 가져옴  
            mediaElement.LoadedBehavior = MediaState.Pause;
            double durationMs = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;  // 영상 길이를 tick으로 가져옴. 

            duration = durationMs;
            total_time = _starttime.AddMilliseconds(duration);
            secondtime = total_time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            db_connect(conn, firsttime, secondtime);

            slider.Maximum = slider.Minimum + durationMs;

            clear_Obj_data();
        }
        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Stop;
            timer.Stop();
            number = 0;
            draw_play_btn();
        }
        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // 미디어 파일 실행 오류시
            MessageBox.Show("동영상 재생 실패 : " + e.ErrorException.Message.ToString());
        }
        #endregion

        #region btn
        private void draw_play_btn()
        {
            Polygon polygon = new Polygon();
            polygon.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            System.Windows.Point Point1 = new System.Windows.Point(5, 5);
            System.Windows.Point Point2 = new System.Windows.Point(5, 25);
            System.Windows.Point Point3 = new System.Windows.Point(25, 15);
            PointCollection myPointCollection = new PointCollection();
            myPointCollection.Add(Point1);
            myPointCollection.Add(Point2);
            myPointCollection.Add(Point3);
            polygon.Points = myPointCollection;

            btnPlay_canvas.Children.Clear();
            btnPlay_canvas.Children.Add(polygon);
        }
        private void draw_pause_btn()
        {
            Polygon polygon1 = new Polygon();
            polygon1.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            System.Windows.Point Point1_1 = new System.Windows.Point(6, 5);
            System.Windows.Point Point1_2 = new System.Windows.Point(13, 5);
            System.Windows.Point Point1_3 = new System.Windows.Point(13, 25);
            System.Windows.Point Point1_4 = new System.Windows.Point(6, 25);
            PointCollection myPointCollection1 = new PointCollection();
            myPointCollection1.Add(Point1_1);
            myPointCollection1.Add(Point1_2);
            myPointCollection1.Add(Point1_3);
            myPointCollection1.Add(Point1_4);
            polygon1.Points = myPointCollection1;


            Polygon polygon2 = new Polygon();
            polygon2.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            System.Windows.Point Point2_1 = new System.Windows.Point(17, 5);
            System.Windows.Point Point2_2 = new System.Windows.Point(24, 5);
            System.Windows.Point Point2_3 = new System.Windows.Point(24, 25);
            System.Windows.Point Point2_4 = new System.Windows.Point(17, 25);
            PointCollection myPointCollection2 = new PointCollection();
            myPointCollection2.Add(Point2_1);
            myPointCollection2.Add(Point2_2);
            myPointCollection2.Add(Point2_3);
            myPointCollection2.Add(Point2_4);
            polygon2.Points = myPointCollection2;

            btnPlay_canvas.Children.Clear();
            btnPlay_canvas.Children.Add(polygon1);
            btnPlay_canvas.Children.Add(polygon2);
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.HasVideo)
            {
                if (mediaElement.LoadedBehavior == MediaState.Pause || mediaElement.LoadedBehavior == MediaState.Stop)
                {
                    draw_pause_btn();
                    if (number == 0)
                        clear_Obj_data();

                    if (speed_check == 0)
                    {
                        timer.Interval = TimeSpan.FromMilliseconds(0.1);
                        timer.Tick += TimerTickHandler;
                        speed_check++;
                    }
                    mediaElement.LoadedBehavior = MediaState.Play;
                    timer.Start();
                }
                else if (mediaElement.LoadedBehavior == MediaState.Play)
                {
                    draw_play_btn();
                    mediaElement.LoadedBehavior = MediaState.Pause;
                    timer.Stop();
                }
            }
        }
        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".mkv",
                //Filter = "All files (*.*)|*.*",
                Filter = "Video Files (*.mp4, *.avi, *.wmv, *.mkv)|*.mp4;*.avi;*.wmv;*.mkv|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                // 선택한 파일 경로 가져오기
                string filePath = dlg.FileName;
                // MediaElement에 선택한 파일 설정
                mediaElement.Source = new Uri(filePath);
            }
            mediaElement.LoadedBehavior = MediaState.Pause;

            string filepp = dlg.FileName;

            string _start = System.IO.Path.GetFileNameWithoutExtension(filepp);

            if (DateTime.TryParseExact(_start, "yyyy-MM-dd HH-mm-ss", null, System.Globalization.DateTimeStyles.None, out _starttime))
            {
                // System.Console.WriteLine("time: {0}", _starttime.ToString());
            }
            else
            {
                // System.Console.WriteLine("tttttttttime: {0}", _starttime.ToString());
            }

            slider.Minimum = _starttime.Ticks;

            firsttime = _starttime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            secondtime = total_time.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        private void slow_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.SpeedRatio >= 0.2)
                mediaElement.SpeedRatio -= 0.1;
            Play_Speed.Content = mediaElement.SpeedRatio.ToString("x 0.0");
        }
        private void normal_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.SpeedRatio = 1.0;
            Play_Speed.Content = mediaElement.SpeedRatio.ToString("x 0.0");
        }
        private void quick_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.SpeedRatio <= 2.0)
                mediaElement.SpeedRatio += 0.1;
            Play_Speed.Content = mediaElement.SpeedRatio.ToString("x 0.0");
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Stop;
            timer.Stop();

            clear_Obj_data();

            int a = 0;
            TimeSpan value_time = TimeSpan.FromMilliseconds(slider.Value - slider.Minimum); // 현재 슬라이드 바 시간

            TimeSpan back_time = TimeSpan.FromMilliseconds(slider.Value - slider.Minimum - 3000);

            TimeSpan dura = mediaElement.Position; //영상 시간 계산

            dura = dura.Subtract(TimeSpan.FromSeconds(3));


            while (a < dataList.Count)
            {
                try   // number 값이 없을 때 
                {
                    _checktime = dataList[number].Timestamp;
                }
                catch
                {
                    number = 0;
                    break;
                }
                if (_checktime <= _starttime.Add(back_time))  //  _starttime.add(value_time) = 현재 시간  
                {
                    if (number == -1)
                    {
                        number = 0;
                    }
                    break;
                }
                else
                {
                    number--;
                    if (number >= 0)
                    {
                        while (dataList[number].Timestamp != dataList[number + 1].Timestamp)
                        {
                            number--;
                        }
                    }
                }
                a++;
            }

            dura = back_time;
            dbcomparetime = _starttime.Add(dura).ToString("yyyy-MM-dd HH:mm:ss.fff");
            dbcompareDT = _starttime.Add(dura);


            mediaElement.Position = TimeSpan.FromMilliseconds(slider.Value - slider.Minimum - 3000);
            mediaElement.LoadedBehavior = MediaState.Play;
            timer.Start();
        }
        #endregion

        #region slider
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Slider의 값이 변경될 때 MediaElement의 재생 위치를 변경
            //마우스로 움직였을 때가 아니라 그냥 움직여도 변하는 ...
            double currentValue = e.NewValue;
            double difference = currentValue - _previousValue_check;

            _previousValue_check = currentValue;
        }
        private void slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Pause;
            timer.Stop();
        }
        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            TimeSpan dura = mediaElement.Position; //영상 시간 계산

            TimeSpan value_time = TimeSpan.FromMilliseconds(slider.Value - slider.Minimum);

            clear_Obj_data();

            if (e.HorizontalChange > 0)   //앞으로 갔을 때 
            {
                int a = 0;
                while (a < dataList.Count)
                {
                    try  // number 값이 없을 때   > 왜 number
                    {
                        _checktime = dataList[number].Timestamp;             //시작 시간 
                    }
                    catch
                    {
                        number = 0;
                        break;
                    }

                    if (_starttime.Add(value_time) < _checktime)    //  _starttime.add(value_time) = 현재 시간  
                    {                                               //   _check_time = db 데이터의 시간 값
                        break;
                    }
                    else
                    {
                        number++;   // ++할수록 checktime 도 앞으로 커짐   
                    }
                    a++;
                }
                dura = value_time;
                dbcomparetime = _starttime.Add(dura).ToString("yyyy-MM-dd HH:mm:ss.fff");
                dbcompareDT = _starttime.Add(dura);
            }
            else if (e.HorizontalChange < 0)    // 뒤로 갔을 때
            {
                int a = 0;
                while (a < dataList.Count)
                {
                    try   // number 값이 없을 때 
                    {
                        _checktime = dataList[number].Timestamp;
                    }
                    catch
                    {
                        number = 0;
                        break;
                    }
                    if (_checktime <= _starttime.Add(value_time))  //  _starttime.add(value_time) = 현재 시간  
                    {
                        if (number == -1)
                        {
                            number = 0;
                        }
                        break;
                    }
                    else
                    {
                        number--;
                        if (number >= 0)
                        {
                            while (dataList[number].Timestamp != dataList[number + 1].Timestamp)
                            {
                                number--;
                            }
                        }
                    }
                    a++;
                }
                dura = value_time;
                dbcomparetime = _starttime.Add(dura).ToString("yyyy-MM-dd HH:mm:ss.fff");
                dbcompareDT = _starttime.Add(dura);
            }
            else
            {
                dbcomparetime = _starttime.Add(dura).ToString("yyyy-MM-dd HH:mm:ss.fff");
                dbcompareDT = _starttime.Add(dura);
            }
            mediaElement.Position = TimeSpan.FromMilliseconds(slider.Value - slider.Minimum);
            mediaElement.LoadedBehavior = MediaState.Play;
            draw_pause_btn();
            timer.Start();
        }
        #endregion

        #region checkBox
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 200; i++)
            {
                if (textBoxes[i] != null)
                    textBoxes[i].Visibility = Visibility.Visible;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 200; i++)
            {
                if (textBoxes[i] != null)
                    textBoxes[i].Visibility = Visibility.Hidden;
            }
        }
        private string CheckBox_print(int index)
        {
            string pprint = "";
            for (int i = 0; i < 16; i++)
            {
                string db_data = "";
                if (checkBoxes[i].IsChecked == true)
                {
                    if (i == 0) db_data = this_frame_data[index].Timestamp.ToString();
                    else if (i == 1) db_data = index.ToString();//this_frame_data[index].ID.ToString();
                    else if (i == 2) db_data = this_frame_data[index].DistLat.ToString("0.0");
                    else if (i == 3) db_data = this_frame_data[index].DistLong.ToString("0.0");
                    else if (i == 4) db_data = this_frame_data[index].VrelLat.ToString("0.0");
                    else if (i == 5) db_data = this_frame_data[index].VrelLong.ToString("0.0");
                    else if (i == 6) db_data = this_frame_data[index].Velocity.ToString("0.0");
                    else if (i == 7) db_data = this_frame_data[index].RCS.ToString("0.0");
                    else if (i == 8) db_data = this_frame_data[index].ProbOfExist.ToString();
                    else if (i == 9) db_data = this_frame_data[index].Class.ToString();
                    else if (i == 10) db_data = this_frame_data[index].Zone.ToString();
                    else if (i == 11) db_data = this_frame_data[index].Length.ToString("0.0");
                    else if (i == 12) db_data = this_frame_data[index].Width.ToString("0.0");
                    //else if (i == 13) db_data = this_frame_data[index].DynProp.ToString();
                    else if (i == 13) db_data = this_frame_data[index].Merged.ToString();
                    else if (i == 14) db_data = (this_frame_data[index].Length * this_frame_data[index].Width).ToString("0.0");
                    else if (i == 15) db_data = Obj_inf[index].Count.ToString();
                    pprint += checkbox_name[i] + " = " + db_data + '\n';
                }
            }
            pprint = pprint.TrimEnd('\n');
            return pprint;
        }
        #endregion

        #region Save Setting

        #region ini 입력 메소드
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        private void Save_Setting_Value_Click(object sender, RoutedEventArgs e)
        {
            WritePrivateProfileString("RADAR", "ANGLE", Angle.ToString(), "./Setting.ini");
            WritePrivateProfileString("RADAR", "SHIFT", Shift.ToString("F2"), "./Setting.ini");
            for (int i = 0; i < (int)((max_long / Dist_Lane_gap) + 1); i++)
                WritePrivateProfileString("RADAR", "LANE_SHIFT" + (i + 1).ToString(), Lane_shift[i].ToString("F2"), "./Setting.ini");
            for (int i = 0; i < 6; i++)
                WritePrivateProfileString("RADAR", "LANE_WIDTH" + (i + 1).ToString(), Lane_width[i].ToString("F2"), "./Setting.ini");
        }
        private void Load_Setting_Value()
        {
            StringBuilder str_value = new StringBuilder();
            GetPrivateProfileString("RADAR", "ANGLE", "", str_value, 32, "./Setting.ini");
            if (str_value.Length != 0)
                Angle = Convert.ToInt32(str_value.ToString());
            GetPrivateProfileString("RADAR", "SHIFT", "", str_value, 32, "./Setting.ini");
            if (str_value.Length != 0)
                Shift = Convert.ToDouble(str_value.ToString());
            for (int i = 0; i < (int)((max_long / Dist_Lane_gap) + 1); i++)
            {
                GetPrivateProfileString("RADAR", "LANE_SHIFT" + (i + 1).ToString(), "", str_value, 32, "./Setting.ini");
                if (str_value.Length != 0)
                    Lane_shift[i] = Convert.ToDouble(str_value.ToString());
            }
            for (int i = 0; i < 6; i++)
            {
                GetPrivateProfileString("RADAR", "LANE_WIDTH" + (i + 1).ToString(), "", str_value, 32, "./Setting.ini");
                if (str_value.Length != 0)
                    Lane_width[i] = Convert.ToDouble(str_value.ToString());
            }
        }
        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update_map();
        }
    }
}