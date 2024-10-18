using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace fileparallel
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

        private void LargeFile()
        {
            string filePath = "largeFile.txt";

            // 큰 파일 생성 (테스트용, 이미 큰 파일이 있다면 이 과정 생략 가능)
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))// 파일 생성
                {

                    for (int i = 0; i < 100; i++) // 1000만 줄의 큰 파일 생성
                    {
                        writer.WriteLine("This is line number " + i);// 파일 쓰기
                        //파일 쓰기
                    }

                }
            }
            ParalleFile(filePath);

        }

        private void ParalleFile(string filePath)
        {
            // 파일의 라인을 병렬로 처리
            IEnumerable<string> lines = File.ReadLines(filePath); // 파일을 줄 단위로 읽음 (스트리밍 방식)

            // 병렬로 각 줄을 처리
            Parallel.ForEach(lines, line =>
            {
                // 읽은 데이터 처리 (UI 업데이트)
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    textblock0.Text += line + Environment.NewLine;
                }));
            });

            // 병렬 작업이 모두 완료되었을 때 출력
            Dispatcher.BeginInvoke(new Action(() =>
            {
                textblock0.Text += "모든 스레드가 파일을 처리했습니다." + Environment.NewLine;
            }));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => LargeFile());// 새로운 스레드 생성해서 처리 
            thread.Start();
 
        }
        //private void ParalleFile(string filePath)
        //{
        //    const int numberOfTasks = 5; // 병렬로 실행할 태스크 수
        //    FileInfo fileInfo = new FileInfo(filePath);
        //    long fileSize = fileInfo.Length;
        //    long chunkSize = fileSize / numberOfTasks; // 각 태스크가 처리할 파일 크기

        //    // 병렬로 파일을 읽기
        //    Parallel.For(0, numberOfTasks, i =>
        //    {
        //        long offset = i * chunkSize; // 시작할 파일 내부의 위치 
        //        long end = (i == numberOfTasks - 1) ? fileSize : offset + chunkSize; // 각 태스크가 끝날 파일의 위치 

        //        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        using (StreamReader reader = new StreamReader(fs))
        //        {
        //            fs.Seek(offset, SeekOrigin.Begin); // 파일의 시작 위치로 이동 (구간 시작점 설정)

        //            // 할당된 구간을 처리 (파일의 현재 위치가 구간 끝을 넘지 않도록)
        //            while (reader.BaseStream.Position < end)
        //            {
        //                string line = reader.ReadLine();
        //                if (line == null) break; // 파일 끝에 도달했을 때 종료

        //                // 읽은 데이터 처리 (UI 업데이트)
        //                Dispatcher.BeginInvoke(new Action(() =>
        //                {
        //                    textblock0.Text += line + Environment.NewLine;
        //                }));

        //                // 만약 한 줄 읽은 후 스트림 위치가 끝 구간을 넘었으면 종료
        //                if (reader.BaseStream.Position >= end)
        //                {
        //                    //MessageBox.Show("끝");
        //                    break;
        //                }
        //            }
        //            // 각 스레드가 작업을 완료했을 때 출력
        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                textblock0.Text += $"스레드 {i}가 파일 구간을 모두 처리했습니다." + Environment.NewLine;
        //            }));
        //        }
        //    });
        //}


        //private void ParalleFile(string filePath)
        //{
        //    const int numberOfTasks = 2; // 병렬로 실행할 태스크 수
        //    FileInfo fileInfo = new FileInfo(filePath);
        //    long fileSize = fileInfo.Length;
        //    long chunkSize = fileSize / numberOfTasks; // 각 태스크가 처리할 파일 크기

        //    // 병렬로 파일을 읽기
        //    Parallel.For(0, numberOfTasks, i =>
        //    // parallel 병렬로 작업을 할 수 있게 0번째 부터 numberOfTasks-1까지의 개수로 진행 
        //    {
        //        long offset = i * chunkSize;// 시작할 파일 내부의 위치 
        //        long end = (i == numberOfTasks - 1) ? fileSize : offset + chunkSize;// 각 태스크가 끝날 파일의 위치 

        //        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        // FileStream(String, FileMode, FileAccess, FileShare)	
        //        // 지정된 경로, 생성 모드, 읽기 / 쓰기 권한 및 공유 권한을 사용하여 FileStream 클래스의 새 인스턴스를 초기화합니다.
        //        // 공유권한 공유 권한 
        //        // access는 현재 스레드에서의 권한을이야기하는거고 , share는 다른 스레드나, 프로세스에서 파일에 대한 권한을 말하는거네
        //        // // 여기서는 병렬 작업을 위한 옵션들이네 
        //        using (StreamReader reader = new StreamReader(fs))
        //        //파일스트림은 바이트단위로 파일을 읽고, 스트림리더는 텍스트파일을 읽는거네 문자단위 줄 단위로
        //        // 사용할 때는 파일스트림을 스트림리더로 감싸서 사용한다. 
        //        {

        //            fs.Seek(offset, SeekOrigin.Begin); // 파일의 시작 위치로 이동 (구간 시작점 설정)

        //            int lineCount = 0; // 처리한 라인 수를 추적
        //            int maxLinesToRead = 10000; // 예시로, 각 스레드가 읽을 최대 라인 수를 설정
        //            long currentPosition = reader.BaseStream.Position;
        //            // 구간을 처리할 때, 라인 수로 구간을 제한
        //            int count = 0;
        //            string line = reader.ReadLine();


        //            while (lineCount < maxLinesToRead)

        //            // 여기서 현재 위치를 넣어서 
        //            {
        //                count++;
        //                //string line = reader.ReadLine();
        //                if (line == null) break; // 파일 끝에 도달했을 때 종료

        //                lineCount++;

        //                Dispatcher.BeginInvoke(new Action(() =>
        //                {
        //                    textblock0.Text += line + Environment.NewLine;
        //                    textblock0.Text += count + Environment.NewLine;
        //                }));
        //            }

        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                textblock0.Text += $"스레드 {i}가 파일 구간을 모두 처리했습니다." + Environment.NewLine;
        //            }));



        //        }
        //    });
        //}




    }
}
