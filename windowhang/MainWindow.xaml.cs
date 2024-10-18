using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace windowhang
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
        // ui스레드에서 파일을 생성하고 10억줄짜리 텍스트틀 쓰고, 저장한다. 살짝 걸림 
        // 병렬처리 안했음 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LargeFile();
            //LargeFileStream();
        }

        private void LargeFile()
        {
            string filePath = "largeFile.txt";

            // 큰 파일 생성 (테스트용, 이미 큰 파일이 있다면 이 과정 생략 가능)
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))// 파일 생성 스트림 라이터 객체가 종료되면 파일이 저장된다. 
                {
                    for (int j = 0; j < 10; j++)
                    {
                        for (int i = 0; i < 1000000000; i++) // 1000만 줄의 큰 파일 쓰기
                        {
                            writer.WriteLine("This is line number " + i);
                            //파일 쓰기
                        }
                    }
                }
            }

            // 파일 전체를 한 번에 읽어오는 예제 - 대규모 파일로 인해 window hang 유발 가능
            try
            {
                for (int j = 0; j < 10; j++)
                {
                    string[] allLines = File.ReadAllLines(filePath); // 모든 줄을 메모리에 로드 
                    textblock0.Text += allLines.Length + " 줄";
                    //Console.WriteLine("파일 읽기 완료: " + allLines.Length + " 줄");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("오류 발생: " + ex.Message);
                MessageBox.Show("오류 발생: " + ex.Message);
            }
            textblock0.Text += "작업 끝";
            //Console.WriteLine("작업 완료");
        }


        private void LargeFileStream()
            // 스트림 사용한 파일처리예제
            // 이거하니까 메모리 사용량이 증가하지않네 
        {
            string filePath = "largeFile.txt";

            // 큰 파일 생성 (테스트용, 이미 큰 파일이 있다면 이 과정 생략 가능)
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int i = 0; i < 10000000; i++) // 1000만 줄의 큰 파일 생성
                    {
                        writer.WriteLine("This is line number " + i);
                    }
                }
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    int lineCount = 0;

                    // 파일의 끝까지 한 줄씩 읽기
                    while ((line = reader.ReadLine()) != null)
                    {
                        // 여기서 각 줄을 처리할 수 있음
                        lineCount++;
                        if (lineCount % 100000 == 0) // 진행 상황 출력
                        {
                            Console.WriteLine($"{lineCount} 줄 읽음");
                        }
                    }

                    Console.WriteLine($"파일 읽기 완료: 총 {lineCount} 줄");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("오류 발생: " + ex.Message);
            }

            Console.WriteLine("작업 완료");

        }
    }
}
