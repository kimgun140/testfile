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
//using System.Windows.Shapes;

namespace testfile
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
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true // 여러 파일 선택 가능
            };

            if (openFileDialog.ShowDialog() == true)
            {
                List<string> filePaths = new List<string>(openFileDialog.FileNames);
                List<string> fileContents = new List<string>();

                // 파일 읽기 병렬 처리
                Parallel.ForEach(filePaths, filePath =>
                {
                    string content = File.ReadAllText(filePath);
                    lock (fileContents) // 멀티 스레드 환경에서 안전하게 리스트에 추가
                    {
                        fileContents.Add(content);
                    }
                });

                // 모든 파일 내용을 TextBox에 표시
                fileContentTextBox.Text = string.Join(Environment.NewLine + "----- 파일 구분선 -----" + Environment.NewLine, fileContents);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string[] fileContents = fileContentTextBox.Text.Split(new[] { "----- 파일 구분선 -----" }, StringSplitOptions.None);
                List<string> outputFilePaths = new List<string>();

                // 파일 저장 위치 여러 개 생성
                for (int i = 0; i < fileContents.Length; i++)
                {
                    outputFilePaths.Add(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), $"파일_{i + 1}.txt"));
                }

                // 파일 쓰기 병렬 처리
                Parallel.ForEach(outputFilePaths, (filePath, state, index) =>
                {
                    File.WriteAllText(filePath, fileContents[index]);
                });

                MessageBox.Show("모든 파일이 성공적으로 저장되었습니다!", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
