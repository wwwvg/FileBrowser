using MainModule.Events;
using MainModule.Helpers;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.ViewModels.Content
{
    public class HexViewModel : BindableBase, INavigationAware
    {
        #region СВОЙСТВА
        private FileInfoModel _fileInfoModel;

        private GetYieldText _hexText;

        IEventAggregator _eventAggregator; // будет посылать сообщения об ошибках

        private int _bufferPos = 0;

        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        #endregion

        #region КОМАНДА СКРОЛЛА - ПОКА НЕ ИСПОЛЬЗУЕТСЯ
        private DelegateCommand _scrollChanged;
        public DelegateCommand ScrollChanged =>
            _scrollChanged ?? (_scrollChanged = new DelegateCommand(ExecuteScrollChanged));

        void ExecuteScrollChanged()
        {
            Text += _hexText.AllLinesFromFile(10);
            //StringBuilder text = new StringBuilder();
            //int counter = 0;
            //foreach (var line in _hexText.AllLinesFromFile(_fileInfoModel.FullPath))
            //{
            //    text.Append(line);
            //    if (counter++ == 2)
            //    {
            //        counter = 0;
            //        break;
            //    }
            //}
            //Text += text;
        }
        #endregion

        #region РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА НАВИГАЦИИ (INavigationAware)
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
            {
                _fileInfoModel = navigationContext.Parameters.GetValue<FileInfoModel>("FileInfoModel");
                _hexText = new GetYieldText(_fileInfoModel.FullPath);
                Text = _hexText.AllLinesFromFile(50);
                //StringBuilder text = new StringBuilder();
                //int counter = 0;
                //foreach (var line in _hexText.AllLinesFromFile(_fileInfoModel.FullPath))
                //{
                //    text.Append(line);
                //    if(counter++ == 50)
                //        break;
                //}
                //Text = text.ToString();
            }
        }
        #endregion

        #region ВЫВОД ТЕКСТА В НЕХ ФОРМАТЕ НА ЭКРАН
        void SetText()
        {
            StringBuilder text = new StringBuilder();
            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            int counter = 0;
            string hexNumber = string.Empty;
            try
            {
                using (System.IO.BinaryReader br = new System.IO.BinaryReader
                    (new System.IO.FileStream
                            (
                            _fileInfoModel.FullPath,
                            System.IO.FileMode.Open,
                            System.IO.FileAccess.Read,
                            System.IO.FileShare.None,
                            1024)
                        )
                        )
                {
                    if (counter++ % 16 == 0)
                        hexNumber = counter.ToString("X");
                    //br.BaseStream.Seek(100, SeekOrigin.Begin);
                    byte[] inbuff = new byte[0];
                    int b = 0;
                    while ((inbuff = br.ReadBytes(16)).Length > 0)
                    {
                        for (b = 0; b < inbuff.Length - 1; b++)
                        {
                            text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + " ");
                        }
                        text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + "\n");
                        // if (_bufferPos++ % 100 == 0)
                        //   break;
                    }
                }
                //Text = text.ToString();
            }
            catch(Exception ex)
            {
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            }
        }

        private IEnumerable<string> AllLinesFromFile()
        {
            LinkedList<string> Lines = new LinkedList<string>();
            StringBuilder text = new StringBuilder();
            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
///dddddddddddddddddddd
            using (BinaryReader br = new BinaryReader(new System.IO.FileStream
                            (
                            _fileInfoModel.FullPath,
                            System.IO.FileMode.Open,
                            System.IO.FileAccess.Read,
                            System.IO.FileShare.None,
                            32)
                        ))
            {

                byte[] inbuff = new byte[0];
                int b = 0;
                int count = 0;
                int countRow = 0;
                while ((inbuff = br.ReadBytes(16)).Length > 0)
                {
                    for (b = 0; b < inbuff.Length - 1; b++)
                    {
                        text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + " ");
                        count++;
                    }
                    text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + "\n");
                    //Lines.AddLast(text.ToString());
                    yield return text.ToString();
                    text.Clear();
                }
            }
        }
        //async Task SetTextAsync()
        //{
        //    // асинхронное чтение
        //    using (StreamReader reader = new StreamReader(_fileInfoModel.FullPath))
        //    {
        //        string? line;
        //        while ((line = await reader.ReadLineAsync()) != null)
        //        {
        //            Text += line + "\n";
        //        }
        //    }
        //}
        #endregion

        #region КОНСТРУКТОР
        public HexViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion
    }
}
