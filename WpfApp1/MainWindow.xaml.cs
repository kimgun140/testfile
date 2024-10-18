using System;
using System.Collections.Generic;
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
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Start button click event
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "largeFileeeeeee.txt";

            // 큰 파일을 생성합니다. (파일이 이미 존재하면 생략 가능)
            if (!File.Exists(filePath))
            {
                GenerateLargeFile(filePath);
            }

            // 파일을 병렬로 처리합니다.
            ProcessFileInParallel(filePath);
        }
        private void GenerateLargeFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < 100000; i++) // 100,000 줄의 파일을 생성합니다.
                {
                    writer.WriteLine($"This is line number {i}");
                }
            }
            AppendTextToOutput("Large file generated.\n");
        }
        private void ProcessFileInParallel(string filePath)
        {
            const int numberOfTasks = 4; // 병렬로 처리할 태스크 수
            FileInfo fileInfo = new FileInfo(filePath);
            long fileSize = fileInfo.Length; // 파일 크기
            long chunkSize = fileSize / numberOfTasks; // 각 태스크가 처리할 파일 청크 크기

            // 병렬 처리
            Parallel.For(0, numberOfTasks, i =>
            {
                long offset = i * chunkSize; // 시작할 파일 내부의 위치
                long end = (i == numberOfTasks - 1) ? fileSize : offset + chunkSize; // 각 태스크가 끝날 위치 설정

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader reader = new StreamReader(fs))
                {
                    fs.Seek(offset, SeekOrigin.Begin); // 파일의 시작 위치로 이동

                    // 읽기 작업을 수행하면서 파일의 끝까지 처리
                    while (fs.Position < end)
                    {
                        string line = reader.ReadLine();
                        if (line == null) break;

                        // 처리할 작업 (여기서는 TextBox에 출력)
                        AppendTextToOutput($"Task {i} - Read line: {line}\n");
                    }
                }
            });

            AppendTextToOutput("File processing complete.\n");
        }
        private void AppendTextToOutput(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                outputTextBox.AppendText(text);
                outputTextBox.ScrollToEnd(); // 새 텍스트가 추가될 때 자동으로 스크롤
            }));
        }
    }

}
