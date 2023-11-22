using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace _100HperWeekTimer
{
    //하루 최대 몇 시간 소비했는 지 기록을 남기는 클래스
    //log는 날짜|초단위 시간|시간,분,초로 나타낸 시간 로 표기 (시간은 모두 0으로 채워서 자릿수 맞추기
    //한 줄의 바이트 크기 45
    public static class DailyLogger
    {
        private const int LINELENGTH = -45;
        private const int INITIALCAPACITY = 64;
        private static uint passedTime = 0;
        private static string[] lines = null; 
        private static readonly string directory = Directory.GetCurrentDirectory() + "\\log.txt";
        private static readonly char[] seperator = { '|'};
        private static readonly char[] dateSeperator = { '/', ' ' };
        public static void init()
        {
            if (File.Exists(directory))
            {
                lines = File.ReadAllLines(directory);
                //마지막 줄을 그냥 읽어온다고 생각
                string[] strings = lines[lines.Length - 1].Split(seperator);
                string[] strings1 = strings[0].Split(dateSeperator);
                DateTime date = new DateTime(int.Parse(strings1[0]), int.Parse(strings1[1]), int.Parse(strings1[2]));
                if (date.Equals(DateTime.Today))
                {
                    passedTime = UInt32.Parse(strings[1]);
                }
            }
            else
            {
                FileStream fileStream = File.Create(directory);
                fileStream.Close();
            }
        }
        public static void dispose()
        {
            using (FileStream fileStream = File.Open(directory, FileMode.Open))
            {
                byte[] temp;
                if (lines != null)
                {
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        temp = Encoding.UTF8.GetBytes(lines[i] + "\n");
                    }
                }
                temp = Encoding.UTF8.GetBytes(DateTime.Today.ToString() + seperator[0]);
                fileStream.Write(temp, 0, temp.Length);
                StringBuilder stringBuilder = new StringBuilder();
                string time = passedTime.ToString();
                for (int i = 0; i < 6 - time.Length; ++i)
                {
                    stringBuilder.Append('0');
                }
                stringBuilder.Append(time);
                stringBuilder.Append(seperator[0]);
                temp = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                fileStream.Write(temp, 0, temp.Length);
                stringBuilder.Clear();
                time = (passedTime / 3600).ToString();
                if (time.Length != 2)
                {
                    stringBuilder.Append('0');
                }
                stringBuilder.Append(time);
                stringBuilder.Append("시간");
                time = ((passedTime % 3600) / 60).ToString();
                if (time.Length != 2)
                {
                    stringBuilder.Append('0');
                }
                stringBuilder.Append(time);
                stringBuilder.Append("분");
                time = ((passedTime % 3600) % 60).ToString();
                if (time.Length != 2)
                {
                    stringBuilder.Append('0');
                }
                stringBuilder.Append(time);
                stringBuilder.Append("초\n");
                temp = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                fileStream.Write(temp, 0, temp.Length);
            }
        }
        public static void increasePassedTime()
        {
            passedTime++;
        }
    }
}
