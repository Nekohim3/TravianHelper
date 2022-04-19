using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.Settings
{
    public class Proxy : DbEntity
    {
        private string _ip;

        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                RaisePropertyChanged(() => Ip);
            }
        }

        private int _port;

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                RaisePropertyChanged(() => Port);
            }
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        private string _note;

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                RaisePropertyChanged(() => Note);
            }
        }

        public Proxy()
        {

        }

        public Proxy(int id, string ip, int port, string userName, string password, string note)
        {
            Id       = id;
            Ip       = ip;
            Port     = port;
            UserName = userName;
            Password = password;
            Note     = note;
        }

        public override string ToString()
        {
            return $"{Ip}{(Port == 0 ? "" : ":")}{(Port == 0 ? "" : Port.ToString())} {(string.IsNullOrEmpty(Note) ? "" : "(")}{Note}{(string.IsNullOrEmpty(Note) ? "" : ")")}";
        }
    }
}
