using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Timers;
using System.IO.Pipes;
/*to do: 하루에 몇시간 하는 지 log 남기는 코드 짜기*/
namespace _100HperWeekTimer
{
    public static class Program
    {
        private const uint goalTime = 360000;
        private static readonly char[] sepreator = { '/', ' ' };
        private static readonly string directory = Directory.GetCurrentDirectory() + "\\data.txt";
        private static System.Timers.Timer timer;
        private static uint restTime;
        private static DateTime fileDate;
        public static void Main(string[] args)
        {
            string[] lines;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
            DailyLogger.init();
            /*시간 양식
            날짜 (일주일 지났는지 체크용)
            남은 시간 (초단위)
            */
            if (File.Exists(directory))
            {
                lines = File.ReadAllLines(directory);
                string[] strings = lines[0].Split(sepreator);
                fileDate = new DateTime(Int32.Parse(strings[0]), Int32.Parse(strings[1]), Int32.Parse(strings[2]));
                //남은 시간은 끌 떄 작성
                if (fileDate.DayOfWeek == DateTime.Now.DayOfWeek && !fileDate.Equals(DateTime.Today))
                {
                    fileDate = DateTime.Now;
                    restTime = goalTime;
                }
                else
                {
                    uint fileRestTime = UInt32.Parse(lines[1]);
                    if (fileRestTime == 0)
                    {
                        Console.WriteLine("이미 100시간 채웠다 놀아라.");
                        Environment.Exit(0);
                    }
                    else
                    {
                        restTime = fileRestTime;
                    }
                }
            }
            else
            {
                fileDate = DateTime.Now;
                restTime = goalTime;
            }
            SetTimer();
            uint restHour = restTime / 3600;
            uint restMinute = (restTime % 3600) / 60;
            uint restSecond = (restTime % 3600) % 60;
            Console.WriteLine("남은시간");
            Console.WriteLine($"{restHour.ToString()}:{restMinute.ToString()}:{restSecond.ToString()}");
            Console.ReadLine();
            timer.Stop();
            timer.Dispose();
            Console.WriteLine("종료 중....");
            using (FileStream fileStream = File.Open(directory, FileMode.Create))
            {
                byte[] temp = Encoding.UTF8.GetBytes(fileDate.ToString() + "\n");
                fileStream.Write(temp, 0, temp.Length);
                temp = Encoding.UTF8.GetBytes(restTime.ToString() + "\n");
                fileStream.Write(temp, 0, temp.Length);
            }
            DailyLogger.dispose();
        }
        private static void SetTimer()
        {
            timer = new System.Timers.Timer(1000.0);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        private static void OnTimedEvent(Object Source, ElapsedEventArgs e)
        {
            DailyLogger.increasePassedTime();
            if (restTime-- == 0)
            {
                restTime++;
                Console.WriteLine("이번 주 100시간 도전 성공!");
                //To do: 0 으로 숫자 종료하는 코드 작성;
                using (FileStream fileStream = File.Open(directory, FileMode.Create))
                {
                    byte[] temp = Encoding.UTF8.GetBytes(fileDate.ToString() + "\n");
                    fileStream.Write(temp, 0, temp.Length);
                    temp = Encoding.UTF8.GetBytes(restTime.ToString() + "\n");
                    fileStream.Write(temp, 0, temp.Length);
                }
                Environment.Exit(0);
            }
            uint restHour = restTime / 3600;
            uint restMinute = (restTime % 3600) / 60;
            uint restSecond = (restTime % 3600) % 60;
            Console.Clear();
            Console.WriteLine("남은시간");
            Console.WriteLine($"{restHour.ToString()}:{restMinute.ToString()}:{restSecond.ToString()}");
        }
    }
}
