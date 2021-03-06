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

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private void SetHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"          ");
            foreach (var item in GetYieldText.Digits)
            {
                sb.Append($" 0{item}");
            }

            Header = sb.ToString();
        }

        IEventAggregator _eventAggregator; // будет посылать сообщения об ошибках

        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        #endregion

        #region КОМАНДА СКРОЛЛА
        private DelegateCommand _scrollChanged;
        public DelegateCommand ScrollChanged =>
            _scrollChanged ?? (_scrollChanged = new DelegateCommand(ExecuteScrollChanged));

        void ExecuteScrollChanged()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            while (_enumerator.MoveNext())
            {
                sb.AppendLine(_enumerator.Current);
                if (count++ > 100)
                    break;
            }
            Text += sb.ToString();
        }
        #endregion

        #region РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА НАВИГАЦИИ (INavigationAware)
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;  // создается новый << HexView >> каждый раз при выборе файла, за счет этого освобождается << BinaryReader >> и не блокирует файлы
        }

        private IEnumerator<string> _enumerator;
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose(); // освободили предыдущий enumerator
            }
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
            {
                _fileInfoModel = navigationContext.Parameters.GetValue<FileInfoModel>("FileInfoModel");
                GetYieldText.CloseBinaryReader(); // освободили предыдущий ресурс
                try
                {
                    StringBuilder sb = new StringBuilder();
                    _enumerator = GetYieldText.GetHexLinesFromFile(_fileInfoModel.FullPath); // получаем первые 100 линий
                    int count = 0;
                    while (_enumerator.MoveNext())
                    {
                        sb.AppendLine(_enumerator.Current);  // добавляется очередная линия
                        if (count++ > 100)
                            break;
                    }
                    Text = sb.ToString();  // устанавливаюся первые 100 строк
                }
                catch (Exception ex)
                {
                    _eventAggregator.GetEvent<Error>().Publish(ex.Message);
                }
            }
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion

        #region КОНСТРУКТОР
        public HexViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SetHeader();
        }
        #endregion
    }
}
